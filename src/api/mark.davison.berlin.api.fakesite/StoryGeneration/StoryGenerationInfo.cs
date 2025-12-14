namespace mark.davison.berlin.api.fakesite.StoryGeneration;

public class StoryGenerationInfo
{
    public required string Title { get; set; }
    public required List<string> Summary { get; set; }
    public required string Notes { get; set; }
    public required List<string> Authors { get; set; }
    public required List<string> Fandoms { get; set; }
    public required List<string> Chapters { get; set; }
    public required List<int> ChapterIds { get; set; }
    public required DateOnly Published { get; set; }
    public required DateOnly Updated { get; set; }
    public required int Words { get; set; }
    public required int Comments { get; set; }
    public required int Hits { get; set; }
    public required int Kudos { get; set; }
    public required int Bookmarks { get; set; }
    public required int CurrentChapters { get; set; }
    public required int? TotalChapters { get; set; }
}
