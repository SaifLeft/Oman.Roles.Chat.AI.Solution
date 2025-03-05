
namespace MauiKit.Models;
public class DaskboardTaskModel
{
    public int TaskId { get; set; }
    public string TaskName { get; set; }
    public string TaskDescription { get; set; }
    public string Priority { get; set; }
    public string OwnerName { get; set; }
    public string OwnerAvatar { get; set; }
    public int TaskStatus { get; set; }
    public string CreatedDate { get; set; }
}

public enum TaskStatusOption
{
    NewTask,
    InProgress,
    InReview,
    Completed
}
