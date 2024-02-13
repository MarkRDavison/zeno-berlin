namespace mark.davison.berlin.shared.logic.Models;

public class StoryInfoModel
{
    public bool IsCompleted { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CurrentChapters { get; set; }
    public int? TotalChapters { get; set; }
}
