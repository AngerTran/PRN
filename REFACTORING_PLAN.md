# 📋 Kế hoạch Refactoring: Tách BE/FE và Tổ chức lại Code

## 🔍 Phân tích hiện trạng

### Cấu trúc hiện tại:
```
PRN212/
├── Controllers/          (5 files - quá nhiều business logic)
├── Models/              (9 files - entities + ViewModels lẫn lộn)
├── Data/                (DbContext + DbSeeder)
├── Views/               (Razor views - FE)
├── ViewModels/          (1 file - LoginViewModel)
└── wwwroot/            (Static files - FE)
```

### ❌ Vấn đề hiện tại:

1. **Controllers có quá nhiều business logic:**
   - `StudentController`: 370 dòng, chứa validation, data access trực tiếp
   - `CommitteeController`: 210 dòng, logic phức tạp (auto-cancel, validation)
   - Controllers đang làm việc của cả Service + Repository

2. **Không có tách biệt layers:**
   - Không có Repository pattern → Controllers truy cập DbContext trực tiếp
   - Không có Service layer → Business logic nằm trong Controllers
   - Models và ViewModels lẫn lộn trong cùng folder

3. **FE và BE chưa tách rõ:**
   - Views (Razor) vẫn phụ thuộc vào Controllers
   - Có thể tách thành API Controllers (BE) và Frontend riêng (React/Vue) sau này

---

## 🎯 Cấu trúc mới đề xuất

```
PRN212/
├── Entities/                    # Domain Models (đổi tên từ Models/)
│   ├── ApplicationUser.cs
│   ├── CapstoneGroup.cs
│   ├── Topic.cs
│   ├── DefenseSlot.cs
│   ├── DefenseSlotRegistration.cs
│   ├── Committee.cs
│   ├── CommitteeMember.cs
│   └── UserRole.cs
│
├── Repositories/                 # Data Access Layer
│   ├── Interfaces/
│   │   ├── IGroupRepository.cs
│   │   ├── ITopicRepository.cs
│   │   ├── ISlotRepository.cs
│   │   ├── ICommitteeRepository.cs
│   │   └── IRegistrationRepository.cs
│   └── Implementations/
│       ├── GroupRepository.cs
│       ├── TopicRepository.cs
│       ├── SlotRepository.cs
│       ├── CommitteeRepository.cs
│       └── RegistrationRepository.cs
│
├── Services/                     # Business Logic Layer
│   ├── Interfaces/
│   │   ├── IGroupService.cs
│   │   ├── ITopicService.cs
│   │   ├── ISlotService.cs
│   │   ├── ICommitteeService.cs
│   │   └── IRegistrationService.cs
│   └── Implementations/
│       ├── GroupService.cs
│       ├── TopicService.cs
│       ├── SlotService.cs
│       ├── CommitteeService.cs
│       └── RegistrationService.cs
│
├── Controllers/                   # API Controllers (BE)
│   ├── AccountController.cs      (giữ nguyên - authentication)
│   ├── StudentController.cs      (chỉ gọi Services)
│   ├── CommitteeController.cs    (chỉ gọi Services)
│   ├── AdminController.cs         (chỉ gọi Services)
│   └── HomeController.cs          (giữ nguyên)
│
├── ViewModels/                   # DTOs cho Views
│   ├── LoginViewModel.cs
│   ├── GroupViewModel.cs
│   ├── TopicViewModel.cs
│   ├── SlotViewModel.cs
│   └── DashboardViewModel.cs
│
├── Data/                         # Database Layer
│   ├── ApplicationDbContext.cs
│   └── DbSeeder.cs
│
├── Views/                        # Frontend (Razor Views)
│   ├── Account/
│   ├── Student/
│   ├── Committee/
│   └── Admin/
│
└── wwwroot/                      # Static Files (FE)
```

---

## 📝 Mapping các file hiện tại → Cấu trúc mới

### 1. Entities/ (đổi tên từ Models/)
```
Models/ApplicationUser.cs          → Entities/ApplicationUser.cs
Models/CapstoneGroup.cs            → Entities/CapstoneGroup.cs
Models/Topic.cs                    → Entities/Topic.cs
Models/DefenseSlot.cs              → Entities/DefenseSlot.cs
Models/DefenseSlotRegistration.cs  → Entities/DefenseSlotRegistration.cs
Models/Committee.cs                 → Entities/Committee.cs
Models/CommitteeMember.cs          → Entities/CommitteeMember.cs
Models/UserRole.cs                 → Entities/UserRole.cs
Models/ErrorViewModel.cs           → ViewModels/ErrorViewModel.cs (không phải entity)
```

### 2. Repositories/ (MỚI - tách từ Controllers)
**Logic cần tách:**
- Tất cả các query `_context.XXX.Include().Where().ToListAsync()`
- CRUD operations: Add, Update, Delete, Find, GetById

**Ví dụ:**
```csharp
// Repositories/Interfaces/IGroupRepository.cs
public interface IGroupRepository
{
    Task<CapstoneGroup?> GetGroupByUserIdAsync(string userId);
    Task<CapstoneGroup?> GetGroupWithMembersAsync(int groupId);
    Task<bool> UserHasGroupAsync(string userId);
    Task<CapstoneGroup> CreateGroupAsync(CapstoneGroup group);
    Task AddMemberToGroupAsync(string userId, int groupId);
    Task RemoveMemberFromGroupAsync(string userId, int groupId);
}

// Repositories/Implementations/GroupRepository.cs
public class GroupRepository : IGroupRepository
{
    private readonly ApplicationDbContext _context;
    public GroupRepository(ApplicationDbContext context) => _context = context;
    // Implement các methods...
}
```

### 3. Services/ (MỚI - tách business logic từ Controllers)
**Logic cần tách:**
- Validation rules (ví dụ: max 5 members, max 3 slots)
- Business rules (ví dụ: auto-cancel khi approve slot)
- Permission checks (ví dụ: chỉ leader mới được thêm member)

**Ví dụ:**
```csharp
// Services/Interfaces/IGroupService.cs
public interface IGroupService
{
    Task<ServiceResult<CapstoneGroup>> CreateGroupAsync(string userId, string groupName);
    Task<ServiceResult> AddMemberAsync(string leaderId, string memberEmail);
    Task<ServiceResult> RemoveMemberAsync(string leaderId, string memberId);
}

// Services/Implementations/GroupService.cs
public class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepo;
    private readonly UserManager<ApplicationUser> _userManager;
    
    public async Task<ServiceResult<CapstoneGroup>> CreateGroupAsync(...)
    {
        // Validation: Check if user already has group
        // Business logic: Create group, set leader
        // Call repository
    }
}
```

### 4. Controllers/ (REFACTOR - chỉ xử lý HTTP)
**Controllers mới sẽ:**
- Nhận request từ client
- Gọi Service methods
- Trả về View hoặc JSON response
- Không có business logic, không có data access

**Ví dụ:**
```csharp
// Controllers/StudentController.cs (sau refactor)
public class StudentController : Controller
{
    private readonly IGroupService _groupService;
    private readonly ITopicService _topicService;
    private readonly ISlotService _slotService;
    
    [HttpPost("CreateGroup")]
    public async Task<IActionResult> CreateGroup(string groupName)
    {
        var user = await _userManager.GetUserAsync(User);
        var result = await _groupService.CreateGroupAsync(user.Id, groupName);
        
        if (result.Success)
        {
            TempData["Success"] = result.Message;
            return RedirectToAction("MyGroup");
        }
        
        TempData["Error"] = result.Message;
        return RedirectToAction("MyGroup");
    }
}
```

---

## 🔄 Chi tiết refactoring từng Controller

### StudentController (370 dòng → ~100 dòng)

**Tách ra:**
1. **IGroupRepository + GroupRepository:**
   - `GetGroupByUserIdAsync()`
   - `GetGroupWithMembersAsync()`
   - `CreateGroupAsync()`
   - `AddMemberToGroupAsync()`
   - `RemoveMemberFromGroupAsync()`

2. **IGroupService + GroupService:**
   - `CreateGroupAsync()` - validation: user đã có nhóm chưa?
   - `AddMemberAsync()` - validation: max 5 members, chỉ leader, member chưa có nhóm
   - `RemoveMemberAsync()` - validation: chỉ leader, không tự xóa mình

3. **ITopicRepository + TopicRepository:**
   - `GetTopicByGroupIdAsync()`
   - `CreateTopicAsync()`
   - `UpdateTopicAsync()`
   - `SubmitTopicAsync()`

4. **ITopicService + TopicService:**
   - `CreateTopicAsync()` - validation: chỉ leader, group chưa có topic
   - `UpdateTopicAsync()` - validation: chỉ leader, topic chưa Approved
   - `SubmitTopicAsync()` - validation: chỉ leader

5. **ISlotRepository + SlotRepository:**
   - `GetAvailableSlotsAsync()` - filter Cancelled
   - `GetSlotWithRegistrationsAsync()`
   - `GetGroupRegistrationsAsync()`

6. **ISlotService + SlotService:**
   - `BookSlotAsync()` - validation: max 3 slots, slot chưa đầy, topic đã submit
   - `CancelSlotAsync()` - validation: chỉ leader, chưa Accepted

### CommitteeController (210 dòng → ~80 dòng)

**Tách ra:**
1. **ISlotRepository + SlotRepository:**
   - `CreateSlotAsync()`
   - `GetSlotDetailsAsync()`
   - `RemoveGroupFromSlotAsync()`

2. **ISlotService + SlotService:**
   - `CreateSlotAsync()` - validation: startTime < endTime
   - `RemoveGroupFromSlotAsync()` - validation: permission

3. **ICommitteeRepository + CommitteeRepository:**
   - `GetAllCommitteesAsync()`
   - `CreateCommitteeAsync()`
   - `AddMemberToCommitteeAsync()`

4. **IRegistrationRepository + RegistrationRepository:**
   - `GetRegistrationByIdAsync()`
   - `GetPendingRegistrationsByGroupAsync()`
   - `UpdateRegistrationStatusAsync()`

5. **IRegistrationService + RegistrationService:**
   - `ApproveRegistrationAsync()` - **QUAN TRỌNG**: Logic auto-cancel
     - Check: group đã có Accepted slot chưa?
     - Auto-cancel: các Pending khác → Cancelled + CancelReason
   - `RejectRegistrationAsync()`

---

## 🎨 Tách BE và FE

### Option 1: Giữ Razor Views (Hiện tại - đơn giản)
- **BE**: Controllers trả về Views (MVC pattern)
- **FE**: Razor Views (.cshtml) trong folder Views/
- **Ưu điểm**: Đơn giản, không cần thay đổi nhiều
- **Nhược điểm**: FE và BE vẫn coupling

### Option 2: API Controllers + Frontend riêng (Tương lai)
- **BE**: Controllers trả về JSON (API)
  ```
  Controllers/
  ├── Api/
  │   ├── StudentApiController.cs    (trả về JSON)
  │   ├── CommitteeApiController.cs
  │   └── AdminApiController.cs
  ```
- **FE**: Tách ra project riêng (React/Vue/Angular)
  ```
  Frontend/ (project riêng)
  ├── src/
  │   ├── components/
  │   ├── services/ (API calls)
  │   └── views/
  ```
- **Ưu điểm**: Tách biệt hoàn toàn, dễ scale
- **Nhược điểm**: Cần refactor nhiều, cần thêm API layer

### Đề xuất: **Làm Option 1 trước, sau đó migrate sang Option 2**

---

## 📦 Các class/interface cần tạo

### 1. ServiceResult (Helper class)
```csharp
// Services/Common/ServiceResult.cs
public class ServiceResult<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();
    
    public static ServiceResult<T> Ok(T data, string message = "") 
        => new() { Success = true, Data = data, Message = message };
    
    public static ServiceResult<T> Fail(string message) 
        => new() { Success = false, Message = message };
}
```

### 2. Register Services trong Program.cs
```csharp
// Repositories
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<ITopicRepository, TopicRepository>();
builder.Services.AddScoped<ISlotRepository, SlotRepository>();
builder.Services.AddScoped<ICommitteeRepository, CommitteeRepository>();
builder.Services.AddScoped<IRegistrationRepository, RegistrationRepository>();

// Services
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<ITopicService, TopicService>();
builder.Services.AddScoped<ISlotService, SlotService>();
builder.Services.AddScoped<ICommitteeService, CommitteeService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
```

---

## ✅ Checklist Refactoring

### Phase 1: Tạo cấu trúc mới
- [ ] Tạo folder Entities/ (đổi tên Models/)
- [ ] Tạo folder Repositories/Interfaces/
- [ ] Tạo folder Repositories/Implementations/
- [ ] Tạo folder Services/Interfaces/
- [ ] Tạo folder Services/Implementations/
- [ ] Tạo Services/Common/ServiceResult.cs

### Phase 2: Tách Repositories
- [ ] IGroupRepository + GroupRepository
- [ ] ITopicRepository + TopicRepository
- [ ] ISlotRepository + SlotRepository
- [ ] ICommitteeRepository + CommitteeRepository
- [ ] IRegistrationRepository + RegistrationRepository

### Phase 3: Tách Services
- [ ] IGroupService + GroupService
- [ ] ITopicService + TopicService
- [ ] ISlotService + SlotService
- [ ] ICommitteeService + CommitteeService
- [ ] IRegistrationService + RegistrationService (quan trọng: auto-cancel logic)

### Phase 4: Refactor Controllers
- [ ] StudentController - chỉ gọi Services
- [ ] CommitteeController - chỉ gọi Services
- [ ] AdminController - chỉ gọi Services

### Phase 5: Update Program.cs
- [ ] Register tất cả Repositories
- [ ] Register tất cả Services

### Phase 6: Testing
- [ ] Test tất cả chức năng vẫn hoạt động
- [ ] Test auto-cancel logic
- [ ] Test validation rules

---

## 🚀 Lợi ích sau khi refactor

1. **Separation of Concerns**: Mỗi layer có trách nhiệm rõ ràng
2. **Testability**: Dễ unit test Services và Repositories
3. **Maintainability**: Dễ maintain, dễ thêm features mới
4. **Reusability**: Services có thể dùng lại ở nhiều nơi
5. **Scalability**: Dễ migrate sang API + Frontend riêng sau này

---

## 📌 Lưu ý

- **Giữ nguyên Views**: Không cần thay đổi Views trong Phase 1-4
- **Migration từ từ**: Có thể refactor từng Controller một, không cần làm hết cùng lúc
- **Backward compatible**: Đảm bảo chức năng hiện tại vẫn hoạt động sau mỗi bước refactor
