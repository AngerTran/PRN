using CapstoneReviewTool.Entities;
using CapstoneReviewTool.Repositories.Interfaces;
using CapstoneReviewTool.Services.Common;
using CapstoneReviewTool.Services.Interfaces;

namespace CapstoneReviewTool.Services.Implementations;

public class TopicService : ITopicService
{
    private readonly ITopicRepository _topicRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly ITopicStatusNotifier _topicStatusNotifier;

    public TopicService(
        ITopicRepository topicRepository,
        IGroupRepository groupRepository,
        ITopicStatusNotifier topicStatusNotifier)
    {
        _topicRepository = topicRepository;
        _groupRepository = groupRepository;
        _topicStatusNotifier = topicStatusNotifier;
    }

    public async Task<ServiceResult<Topic>> CreateTopicAsync(string userId, int groupId, string name, string description, string field)
    {
        // Validation: Check required fields
        if (string.IsNullOrWhiteSpace(name))
        {
            return ServiceResult<Topic>.Fail("Tên đề tài không được để trống.");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            return ServiceResult<Topic>.Fail("Mô tả đề tài không được để trống.");
        }

        if (name.Length > 200)
        {
            return ServiceResult<Topic>.Fail("Tên đề tài không được vượt quá 200 ký tự.");
        }

        var group = await _groupRepository.GetGroupWithMembersAsync(groupId);
        if (group == null)
        {
            return ServiceResult<Topic>.Fail("Không tìm thấy nhóm.");
        }

        if (group.LeaderId != userId)
        {
            return ServiceResult<Topic>.Fail("Chỉ nhóm trưởng mới có thể đăng ký đề tài.");
        }

        // Check if group has minimum members (optional validation)
        if (group.Members == null || group.Members.Count < 1)
        {
            return ServiceResult<Topic>.Fail("Nhóm phải có ít nhất 1 thành viên để đăng ký đề tài.");
        }

        var newTopic = new Topic
        {
            Name = name.Trim(),
            Description = description.Trim(),
            Field = field,
            GroupId = groupId,
            Status = "Draft",
            SubmittedDate = null // Only set when submitted
        };

        var topic = await _topicRepository.CreateTopicAsync(newTopic);
        return ServiceResult<Topic>.Ok(topic, "Tạo đề tài thành công! Bạn có thể chỉnh sửa và gửi để xét duyệt.");
    }

    public async Task<ServiceResult> UpdateTopicAsync(string userId, int topicId, string name, string description, string field, string? repoLink)
    {
        var topic = await _topicRepository.GetTopicWithGroupAsync(topicId);
        if (topic == null)
        {
            return ServiceResult.Fail("Không tìm thấy đề tài.");
        }

        if (topic.Group?.LeaderId != userId)
        {
            return ServiceResult.Fail("Không có quyền chỉnh sửa.");
        }

        if (topic.Status == "Approved")
        {
            return ServiceResult.Fail("Đề tài đã được duyệt, không thể chỉnh sửa.");
        }

        topic.Name = name;
        topic.Description = description;
        topic.Field = field;
        topic.RepoLink = repoLink;

        await _topicRepository.UpdateTopicAsync(topic);
        return ServiceResult.Ok("Cập nhật đề tài thành công!");
    }

    public async Task<ServiceResult> DeleteTopicAsync(string userId, int topicId)
    {
        var topic = await _topicRepository.GetTopicWithGroupAsync(topicId);
        if (topic == null)
        {
            return ServiceResult.Fail("Không tìm thấy đề tài.");
        }

        if (topic.Group?.LeaderId != userId)
        {
            return ServiceResult.Fail("Chỉ nhóm trưởng mới có thể xóa đề tài.");
        }

        // Chỉ cho phép xóa nếu đề tài ở trạng thái Draft
        if (topic.Status != "Draft")
        {
            return ServiceResult.Fail($"Không thể xóa đề tài ở trạng thái '{topic.Status}'. Chỉ có thể xóa đề tài ở trạng thái Draft.");
        }

        // Check if topic has any slot registrations
        // Note: This would require IRegistrationRepository, but for now we'll allow deletion
        // In a more complete implementation, you might want to check for active registrations

        await _topicRepository.DeleteTopicAsync(topic);
        return ServiceResult.Ok("Đã xóa đề tài thành công.");
    }

    public async Task<ServiceResult> SubmitTopicAsync(string userId, int topicId)
    {
        var topic = await _topicRepository.GetTopicWithGroupAsync(topicId);
        if (topic == null || topic.Group?.LeaderId != userId)
        {
            return ServiceResult.Fail("Không có quyền gửi đề tài.");
        }

        // Validation: Cannot submit if already approved
        if (topic.Status == "Approved")
        {
            return ServiceResult.Fail("Đề tài đã được gửi và duyệt rồi.");
        }

        // Validation: Ensure required fields are filled
        if (string.IsNullOrWhiteSpace(topic.Name) || string.IsNullOrWhiteSpace(topic.Description))
        {
            return ServiceResult.Fail("Vui lòng điền đầy đủ thông tin đề tài trước khi gửi.");
        }

        // Tự động approve khi submit (không cần hội đồng duyệt)
        topic.Status = "Approved";
        topic.SubmittedDate = DateTime.Now;

        await _topicRepository.UpdateTopicAsync(topic);
        await _topicStatusNotifier.NotifyGroupTopicStatusChangedAsync(
            topic.GroupId, topic.Id, topic.Name, topic.Status, "Đề tài đã được gửi và phê duyệt.");
        return ServiceResult.Ok("Đã gửi đề tài thành công! Bạn có thể đăng ký slot bảo vệ ngay bây giờ.");
    }

    public async Task<ServiceResult> ApproveTopicAsync(int topicId, string? comment = null)
    {
        var topic = await _topicRepository.GetTopicWithGroupAsync(topicId);
        if (topic == null)
        {
            return ServiceResult.Fail("Không tìm thấy đề tài.");
        }

        if (topic.Status != "Submitted" && topic.Status != "Revision")
        {
            return ServiceResult.Fail($"Không thể duyệt đề tài ở trạng thái '{topic.Status}'. Chỉ có thể duyệt đề tài đã được gửi.");
        }

        topic.Status = "Approved";
        await _topicRepository.UpdateTopicAsync(topic);
        await _topicStatusNotifier.NotifyGroupTopicStatusChangedAsync(
            topic.GroupId, topic.Id, topic.Name, topic.Status, comment);
        return ServiceResult.Ok($"Đã duyệt đề tài '{topic.Name}' thành công!" + (string.IsNullOrWhiteSpace(comment) ? "" : $" Ghi chú: {comment}"));
    }

    public async Task<ServiceResult> RejectTopicAsync(int topicId, string? rejectionReason = null)
    {
        var topic = await _topicRepository.GetTopicWithGroupAsync(topicId);
        if (topic == null)
        {
            return ServiceResult.Fail("Không tìm thấy đề tài.");
        }

        if (topic.Status != "Submitted" && topic.Status != "Revision")
        {
            return ServiceResult.Fail($"Không thể từ chối đề tài ở trạng thái '{topic.Status}'.");
        }

        if (string.IsNullOrWhiteSpace(rejectionReason))
        {
            rejectionReason = "Không đáp ứng yêu cầu đề tài.";
        }

        topic.Status = "Rejected";
        topic.Description += $"\n\n[Lý do từ chối: {rejectionReason}]";
        await _topicRepository.UpdateTopicAsync(topic);
        await _topicStatusNotifier.NotifyGroupTopicStatusChangedAsync(
            topic.GroupId, topic.Id, topic.Name, topic.Status, rejectionReason);
        return ServiceResult.Ok($"Đã từ chối đề tài '{topic.Name}'. Lý do: {rejectionReason}");
    }

    public async Task<ServiceResult> RequestRevisionAsync(int topicId, string? comment = null)
    {
        var topic = await _topicRepository.GetTopicWithGroupAsync(topicId);
        if (topic == null)
        {
            return ServiceResult.Fail("Không tìm thấy đề tài.");
        }

        if (topic.Status != "Submitted")
        {
            return ServiceResult.Fail("Chỉ có thể yêu cầu chỉnh sửa đề tài đã được gửi.");
        }

        topic.Status = "Revision";
        if (!string.IsNullOrWhiteSpace(comment))
        {
            topic.Description += $"\n\n[Yêu cầu chỉnh sửa: {comment}]";
        }
        await _topicRepository.UpdateTopicAsync(topic);
        await _topicStatusNotifier.NotifyGroupTopicStatusChangedAsync(
            topic.GroupId, topic.Id, topic.Name, topic.Status, comment);
        return ServiceResult.Ok($"Đã yêu cầu chỉnh sửa đề tài '{topic.Name}'." + (string.IsNullOrWhiteSpace(comment) ? "" : $" Ghi chú: {comment}"));
    }

    public async Task<Topic?> GetTopicByGroupIdAsync(int groupId)
    {
        return await _topicRepository.GetTopicByGroupIdAsync(groupId);
    }

    public async Task<List<Topic>> GetTopicsByGroupIdAsync(int groupId)
    {
        return await _topicRepository.GetTopicsByGroupIdAsync(groupId);
    }

    public async Task<Topic?> GetTopicByIdAsync(int topicId)
    {
        return await _topicRepository.GetTopicByIdAsync(topicId);
    }

    public async Task<List<Topic>> GetAllTopicsAsync()
    {
        return await _topicRepository.GetAllTopicsAsync();
    }

    public async Task<List<Topic>> GetTopicsByStatusAsync(string status)
    {
        return await _topicRepository.GetTopicsByStatusAsync(status);
    }

    public async Task<List<Topic>> GetPendingTopicsAsync()
    {
        return await _topicRepository.GetPendingTopicsAsync();
    }
}
