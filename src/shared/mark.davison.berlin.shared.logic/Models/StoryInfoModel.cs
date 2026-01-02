namespace mark.davison.berlin.shared.logic.Models;

public sealed record ChapterInfoModel(int Number, string Address, string Title);

public sealed class StoryInfoModel
{
    public bool IsCompleted
    { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public int CurrentChapters { get; set; }
    public int? TotalChapters { get; set; }
    public DateOnly Published { get; set; }
    public DateOnly Updated { get; set; }
    public Dictionary<int, ChapterInfoModel> ChapterInfo { get; set; } = [];
    public List<string> Fandoms { get; set; } = [];
    public List<string> Authors { get; set; } = [];
}
