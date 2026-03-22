using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CapstoneReviewTool.Entities;
using CapstoneReviewTool.Services.Interfaces;
using CapstoneReviewTool.ViewModels;

namespace CapstoneReviewTool.Controllers
{
    [Authorize]
    [Route("Student")]
    public class StudentController : Controller
    {
        private readonly IGroupService _groupService;
        private readonly ITopicService _topicService;
        private readonly ISlotService _slotService;
        private readonly IRegistrationService _registrationService;
        private readonly IChatService _chatService;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentController(
            IGroupService groupService,
            ITopicService topicService,
            ISlotService slotService,
            IRegistrationService registrationService,
            IChatService chatService,
            UserManager<ApplicationUser> userManager)
        {
            _groupService = groupService;
            _topicService = topicService;
            _slotService = slotService;
            _registrationService = registrationService;
            _chatService = chatService;
            _userManager = userManager;
        }

        [HttpGet("Chat/History/Group")]
        public async Task<IActionResult> GroupChatHistory(int take = 100, int skip = 0)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var group = await _groupService.GetGroupByUserIdAsync(user.Id);
            if (group == null) return Json(new object[] { });

            var items = await _chatService.GetGroupHistoryAsync(group.Id, take, skip);
            return Json(items);
        }

        [HttpGet("Chat/History/Public")]
        public async Task<IActionResult> PublicChatHistory(int take = 100, int skip = 0)
        {
            if (!User.IsInRole("Student"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { error = "Chỉ sinh viên xem kênh cộng đồng." });
            }

            var items = await _chatService.GetPublicHistoryAsync(take, skip);
            return Json(items);
        }

        [HttpGet("Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var group = await _groupService.GetGroupByUserIdAsync(user.Id);

            var topics = group != null ? await _topicService.GetTopicsByGroupIdAsync(group.Id) : new List<Entities.Topic>();
            var latestTopic = topics.OrderByDescending(t => t.SubmittedDate ?? DateTime.MinValue).ThenByDescending(t => t.Id).FirstOrDefault();

            var regCount = group != null
                ? await _registrationService.CountActiveRegistrationsByGroupAsync(group.Id)
                : 0;
            var hasAccepted = group != null && await _registrationService.GroupHasAcceptedRegistrationAsync(group.Id);
            var defenseSummary = group != null
                ? await _registrationService.GetDefenseScheduleSummaryForGroupAsync(group.Id)
                : "Chưa xác định";

            var vm = new StudentDashboardViewModel
            {
                Group = group,
                TopicStatusDisplay = latestTopic?.Status ?? "Chưa đăng ký",
                RegisteredSlotsCount = regCount,
                DefenseScheduleSummary = defenseSummary,
                ProgressSteps = BuildStudentProgressSteps(group, topics, latestTopic, regCount, hasAccepted)
            };

            return View(vm);
        }

        private static List<ProgressStepViewModel> BuildStudentProgressSteps(
            CapstoneGroup? group,
            List<Entities.Topic> topics,
            Entities.Topic? latest,
            int regCount,
            bool hasAcceptedSlot)
        {
            var steps = new List<ProgressStepViewModel>
            {
                new()
                {
                    Title = "Đăng ký nhóm",
                    Status = group != null ? "Hoàn thành" : "Chưa thực hiện",
                    Detail = group?.Name,
                    IsMuted = group == null
                },
                new()
                {
                    Title = "Đề tài",
                    Status = topics.Count == 0
                        ? "Chưa đăng ký"
                        : latest?.Status == "Approved"
                            ? "Đã duyệt"
                            : latest?.Status ?? "—",
                    Detail = latest?.Name,
                    IsMuted = topics.Count == 0
                },
                new()
                {
                    Title = "Đăng ký slot bảo vệ",
                    Status = regCount == 0
                        ? "Chưa đăng ký"
                        : hasAcceptedSlot
                            ? "Đã được phân lịch"
                            : $"Đã đăng ký {regCount} slot (chờ hội đồng)",
                    IsMuted = regCount == 0 && !hasAcceptedSlot
                },
                new()
                {
                    Title = "Bảo vệ đồ án",
                    Status = hasAcceptedSlot ? "Đã có lịch" : "Chưa có lịch",
                    IsMuted = !hasAcceptedSlot
                }
            };
            return steps;
        }

        [HttpGet("MyGroup")]
        public async Task<IActionResult> MyGroup()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var group = await _groupService.GetGroupByUserIdAsync(user.Id);
            return View(group);
        }

        [HttpPost("CreateGroup")]
        public async Task<IActionResult> CreateGroup(string groupName)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("MyGroup");

            var result = await _groupService.CreateGroupAsync(user.Id, groupName);
            
            if (result.Success)
            {
                TempData["Success"] = result.Message;
            }
            else
            {
                TempData["Error"] = result.Message;
            }

            return RedirectToAction("MyGroup");
        }

        [HttpPost("AddMember")]
        public async Task<IActionResult> AddMember(string email)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("MyGroup");

            var result = await _groupService.AddMemberAsync(user.Id, email);
            
            if (result.Success)
            {
                TempData["Success"] = result.Message;
            }
            else
            {
                TempData["Error"] = result.Message;
            }

            return RedirectToAction("MyGroup");
        }
        
        [HttpPost("RemoveMember")]
        public async Task<IActionResult> RemoveMember(string userId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("MyGroup");

            var result = await _groupService.RemoveMemberAsync(user.Id, userId);
            
            if (result.Success)
            {
                TempData["Success"] = result.Message;
            }
            else
            {
                TempData["Error"] = result.Message;
            }

            return RedirectToAction("MyGroup");
        }

    [HttpGet("Topic")]
        public async Task<IActionResult> Topic(int? topicId = null)
        {
             var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var group = await _groupService.GetGroupByUserIdAsync(user.Id);
            List<Topic> topics = new List<Topic>();
            Topic? selectedTopic = null;

            if (group != null)
            {
                topics = await _topicService.GetTopicsByGroupIdAsync(group.Id);
                
                if (topicId.HasValue)
                {
                    selectedTopic = topics.FirstOrDefault(t => t.Id == topicId.Value);
                }
                else if (topics.Any())
                {
                    selectedTopic = topics.OrderByDescending(t => t.SubmittedDate ?? DateTime.MinValue).ThenByDescending(t => t.Id).FirstOrDefault();
                }
            }
             
            ViewBag.Topics = topics;
            ViewBag.SelectedTopic = selectedTopic;
            return View(group);
        }

        [HttpPost("CreateTopic")]
        public async Task<IActionResult> CreateTopic(string name, string description, string field)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Topic");

            var group = await _groupService.GetGroupByUserIdAsync(user.Id);
            if (group == null) return RedirectToAction("Topic");

            var result = await _topicService.CreateTopicAsync(user.Id, group.Id, name, description, field);
            
            if (result.Success)
            {
                TempData["Success"] = result.Message;
            }
            else
            {
                TempData["Error"] = result.Message;
            }

            return RedirectToAction("Topic");
        }

        [HttpPost("EditTopic")]
        public async Task<IActionResult> EditTopic(int topicId, string name, string description, string field, string repoLink)
        {
             var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Topic");

            var result = await _topicService.UpdateTopicAsync(user.Id, topicId, name, description, field, repoLink);
            
            if (result.Success)
            {
                TempData["Success"] = result.Message;
            }
            else
            {
                TempData["Error"] = result.Message;
            }

             return RedirectToAction("Topic");
        }
        
        [HttpPost("DeleteTopic")]
        public async Task<IActionResult> DeleteTopic(int topicId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Topic");

            var result = await _topicService.DeleteTopicAsync(user.Id, topicId);
            
            if (result.Success)
            {
                TempData["Success"] = result.Message;
            }
            else
            {
                TempData["Error"] = result.Message;
            }

            return RedirectToAction("Topic");
        }

        [HttpPost("SubmitTopic")]
        public async Task<IActionResult> SubmitTopic(int topicId)
        {
             var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Topic");

            var result = await _topicService.SubmitTopicAsync(user.Id, topicId);
            
            if (result.Success)
            {
                TempData["Success"] = result.Message;
            }
            else
            {
                TempData["Error"] = result.Message;
            }

             return RedirectToAction("Topic");
        }

    [HttpGet("SlotRegistration")]
        public async Task<IActionResult> SlotRegistration()
        {
             var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var group = await _groupService.GetGroupByUserIdAsync(user.Id);
            var slots = await _slotService.GetAvailableSlotsAsync();

             ViewData["UserGroup"] = group;
             return View(slots);
        }

        [HttpPost("BookSlot")]
        public async Task<IActionResult> BookSlot(int slotId)
        {
             var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("SlotRegistration");

            var result = await _registrationService.BookSlotAsync(user.Id, slotId);
            
            if (result.Success)
             {
                TempData["Success"] = result.Message;
             }
            else
             {
                TempData["Error"] = result.Message;
            }

             return RedirectToAction("SlotRegistration");
        }

        [HttpPost("CancelSlot")]
        public async Task<IActionResult> CancelSlot(int slotId)
        {
             var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("SlotRegistration");

            var result = await _registrationService.CancelSlotAsync(user.Id, slotId);
            
            if (result.Success)
            {
                TempData["Success"] = result.Message;
            }
            else
             {
                TempData["Error"] = result.Message;
            }

             return RedirectToAction("SlotRegistration");
        }
    }
}
