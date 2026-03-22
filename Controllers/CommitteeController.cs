using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CapstoneReviewTool.Services.Interfaces;
using CapstoneReviewTool.Services.Common;

namespace CapstoneReviewTool.Controllers
{
    [Authorize]
    [Route("Committee")]
    public class CommitteeController : Controller
    {
    private readonly ISlotService _slotService;
    private readonly ICommitteeService _committeeService;
    private readonly IRegistrationService _registrationService;
    private readonly IDashboardStatsService _dashboardStats;

    public CommitteeController(
        ISlotService slotService,
        ICommitteeService committeeService,
        IRegistrationService registrationService,
        IDashboardStatsService dashboardStats)
    {
        _slotService = slotService;
        _committeeService = committeeService;
        _registrationService = registrationService;
        _dashboardStats = dashboardStats;
    }

        [HttpGet("")]
        [HttpGet("Index")]
        public IActionResult Index() => RedirectToAction(nameof(Dashboard));

        [HttpGet("Dashboard")]
        public async Task<IActionResult> Dashboard(CancellationToken cancellationToken)
        {
            var stats = await _dashboardStats.GetSystemOverviewAsync(cancellationToken);
            return View(stats);
        }

        [HttpGet("Slots")]
        public async Task<IActionResult> Slots()
        {
            var slots = await _slotService.GetAvailableSlotsAsync();
            return View(slots);
        }

        [HttpGet("CreateSlot")]
        public async Task<IActionResult> CreateSlot()
        {
            ViewBag.Committees = await _committeeService.GetAllCommitteesAsync();
            return View();
        }

        [HttpPost("CreateSlot")]
        public async Task<IActionResult> CreateSlot(DateTime date, TimeSpan startTime, TimeSpan endTime, string room, string mode, int maxGroups, int? committeeId)
        {
            var result = await _slotService.CreateSlotAsync(date, startTime, endTime, room, mode, maxGroups, committeeId);
            
            if (result.Success)
            {
                TempData["Success"] = result.Message;
                return RedirectToAction("Slots");
            }

            ModelState.AddModelError("", result.Message);
            ViewBag.Committees = await _committeeService.GetAllCommitteesAsync();
            return View();
        }

        [HttpGet("SlotDetails/{id}")]
        public async Task<IActionResult> SlotDetails(int id)
        {
            var slot = await _slotService.GetSlotDetailsAsync(id);
            if (slot == null) return NotFound();

            return View(slot);
        }

        [HttpPost("RemoveGroupFromSlot")]
        public async Task<IActionResult> RemoveGroupFromSlot(int slotId, int groupId)
        {
            var result = await _slotService.RemoveGroupFromSlotAsync(slotId, groupId);
            
            if (result.Success)
            {
                TempData["Success"] = result.Message;
            }
            else
            {
                TempData["Error"] = result.Message;
            }

            return RedirectToAction("SlotDetails", new { id = slotId });
        }

        [HttpGet("ManageCommittees")]
        public async Task<IActionResult> ManageCommittees()
        {
            var committees = await _committeeService.GetAllCommitteesAsync();
            ViewBag.Users = await _committeeService.GetAllUsersAsync();
            return View(committees);
        }

        [HttpPost("CreateCommittee")]
        public async Task<IActionResult> CreateCommittee(string name)
        {
            var result = await _committeeService.CreateCommitteeAsync(name);
            
            if (result.Success)
            {
                TempData["Success"] = "Tạo hội đồng thành công!";
            }
            else
            {
                TempData["Error"] = result.Message;
            }

            return RedirectToAction("ManageCommittees");
        }

        [HttpPost("AddMember")]
        public async Task<IActionResult> AddMember(int committeeId, string userId)
        {
            var result = await _committeeService.AddMemberToCommitteeAsync(committeeId, userId);
            
            if (result.Success)
            {
                TempData["Success"] = result.Message;
            }
            else
            {
                TempData["Error"] = result.Message;
            }

            return RedirectToAction("ManageCommittees");
        }

        [HttpPost("UpdateRegistrationStatus")]
        public async Task<IActionResult> UpdateRegistrationStatus(int slotId, int registrationId, string status)
        {
            ServiceResult result;
            
            if (status == "Accepted")
            {
                result = await _registrationService.ApproveRegistrationAsync(slotId, registrationId);
            }
            else if (status == "Rejected")
            {
                result = await _registrationService.RejectRegistrationAsync(slotId, registrationId);
            }
            else
            {
                result = ServiceResult.Fail("Trạng thái không hợp lệ.");
            }

            if (result.Success)
            {
                TempData["Success"] = result.Message;
            }
            else
            {
                TempData["Error"] = result.Message;
            }

            return RedirectToAction("SlotDetails", new { id = slotId });
        }

    }
}
