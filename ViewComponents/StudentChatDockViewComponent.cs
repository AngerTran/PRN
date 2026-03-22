using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CapstoneReviewTool.Entities;
using CapstoneReviewTool.Services.Interfaces;
using CapstoneReviewTool.ViewModels;

namespace CapstoneReviewTool.ViewComponents;

public class StudentChatDockViewComponent : ViewComponent
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IGroupService _groupService;
    private readonly IChatService _chatService;

    public StudentChatDockViewComponent(
        UserManager<ApplicationUser> userManager,
        IGroupService groupService,
        IChatService chatService)
    {
        _userManager = userManager;
        _groupService = groupService;
        _chatService = chatService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var principal = HttpContext.User;
        if (principal.Identity?.IsAuthenticated != true)
        {
            return Content(string.Empty);
        }

        var user = await _userManager.GetUserAsync(principal);
        if (user == null)
        {
            return Content(string.Empty);
        }

        var group = await _groupService.GetGroupByUserIdAsync(user.Id);
        var termId = await _chatService.GetActiveTermIdAsync();

        var model = new StudentChatDockViewModel
        {
            TermId = termId,
            GroupId = group?.Id,
            IsStudent = HttpContext.User.IsInRole("Student")
        };

        return View(model);
    }
}
