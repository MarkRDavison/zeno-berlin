namespace mark.davison.berlin.api.fakesite.StoryGeneration;

public class StoryGenerationStateService : IStoryGenerationStateService
{
    private readonly Faker _faker;
    private readonly Dictionary<int, int> _storyRequests;
    private readonly HashSet<int> _chapterIds;

    public StoryGenerationStateService()
    {
        _faker = new();
        _storyRequests = [];
        _chapterIds = [];
    }

    public StoryGenerationInfo RecordGeneration(int externalId, int? chapterId)
    {
        _chapterIds.Clear();
        if (!_storyRequests.ContainsKey(externalId))
        {
            _storyRequests.Add(externalId, 0);
        }

        var info = externalId switch
        {
            FakeStoryConstants.CompleteStoryExternalId => CreateCompleteStoryInfo(externalId),
            FakeStoryConstants.PerpetuallyIncompleteButContinuesStoryExternalId => CreatePerpetuallyIncompleteButContinuesStoryInfo(externalId),
            _ => CreateRandomStoryInfo(externalId),
        };

        _storyRequests[externalId]++;

        return info;
    }

    private StoryGenerationInfo CreatePerpetuallyIncompleteButContinuesStoryInfo(int externalId)
    {
        var chapters = _storyRequests[externalId] + 1;
        return new StoryGenerationInfo
        {
            Title = "The never finished tales of Avalon",
            Summary = "Once upon a time, a long time ago, in a land without time... things continued to keep happening!",
            Notes = "This is the current iteration of the tales of Avalon, it will never end",
            Authors = [FakeStoryConstants.Avalon_Author1, FakeStoryConstants.Avalon_Author2],
            Fandoms = [FakeStoryConstants.Avalon_Fandom1, FakeStoryConstants.Avalon_Fandom2],
            Chapters = Enumerable.Range(1, chapters).Select(_ => $"Chapter {_}: {_faker.Lorem.Word()}").ToList(),
            ChapterIds = Enumerable.Range(0, chapters).Select(_ => CreateChapterId(externalId)).ToList(),
            Published = DateOnly.FromDateTime(DateTime.Today).AddDays(-500),
            Updated = DateOnly.FromDateTime(DateTime.Today).AddDays(chapters - 500),
            Bookmarks = 145 + chapters * 50,
            Hits = 27534 + chapters * 50,
            Kudos = 22 + chapters * 5,
            Words = 3_753 * chapters,
            Comments = 1234 + chapters * 10,
            CurrentChapters = chapters,
            TotalChapters = null
        };
    }

    private StoryGenerationInfo CreateRandomStoryInfo(int externalId)
    {
        var random = new Random(externalId);
        var chapters = random.Next(2, 30);
        int? totalChapters = random.Next(chapters, 40);

        if (random.Next(1, 100) < 60)
        {
            totalChapters = null;
        }

        return new StoryGenerationInfo
        {
            Title = _faker.Hacker.Phrase(),
            Summary = _faker.Rant.Review("story"),
            Notes = _faker.Rant.Review("story"),
            Authors = Enumerable.Range(1, 3).Select(_ => $"{_faker.Name.FirstName()}_{_faker.Name.LastName()}").ToList(),
            Fandoms = Enumerable.Range(1, 5).Select(_ => _faker.Commerce.ProductName()).ToList(),
            Chapters = Enumerable.Range(1, chapters).Select(_ => $"Chapter {_}: {_faker.Lorem.Word()}").ToList(),
            ChapterIds = Enumerable.Range(0, chapters).Select(_ => CreateChapterId(externalId)).ToList(),
            Published = DateOnly.FromDateTime(DateTime.Today).AddDays(-125),
            Updated = DateOnly.FromDateTime(DateTime.Today),
            Bookmarks = 145,
            Hits = 27534,
            Kudos = 22,
            Words = 306_275,
            Comments = 1234,
            CurrentChapters = chapters,
            TotalChapters = totalChapters
        };
    }
    private StoryGenerationInfo CreateCompleteStoryInfo(int externalId)
    {
        var chapters = 10;
        return new StoryGenerationInfo
        {
            Title = "The complete tales of Avalon",
            Summary = "Once upon a time, a long time ago, in a land without time...",
            Notes = "This is the first iteration of the tales of Avalon",
            Authors = ["The first prophet", "A random person"],
            Fandoms = ["Stories of Avalon", "Avalon (2024)"],
            Chapters = Enumerable.Range(1, chapters).Select(_ => $"Chapter {_}: {_faker.Lorem.Word}").ToList(),
            ChapterIds = Enumerable.Range(0, chapters).Select(_ => CreateChapterId(externalId)).ToList(),
            Published = DateOnly.FromDateTime(DateTime.Today).AddDays(-125),
            Updated = DateOnly.FromDateTime(DateTime.Today),
            Bookmarks = 145,
            Hits = 27534,
            Kudos = 22,
            Words = 306_275,
            Comments = 1234,
            CurrentChapters = chapters,
            TotalChapters = chapters
        };
    }

    public async Task ResetAsync()
    {
        _storyRequests.Clear();
        _chapterIds.Clear();
        await Task.CompletedTask;
    }

    private int CreateChapterId(int seed)
    {
        var random = new Random(seed);
        while (true)
        {
            var digits = random.Next(1_111_111, 9_999_999);

            if (!_chapterIds.Contains(digits))
            {
                _chapterIds.Add(digits);
                return digits;
            }
        }
    }
}
