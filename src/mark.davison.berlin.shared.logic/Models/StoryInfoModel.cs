namespace mark.davison.berlin.shared.logic.Models;

public class StoryInfoModel
{
    public bool IsCompleted { get; set; }
    public string Author { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int CurrentChapters { get; set; }
    public int? TotalChapters { get; set; }
    public DateOnly Published { get; set; }
    public DateOnly Updated { get; set; }
    public List<string> Fandoms { get; set; } = [];
}
