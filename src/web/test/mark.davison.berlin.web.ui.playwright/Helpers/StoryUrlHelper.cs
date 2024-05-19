namespace mark.davison.berlin.web.ui.playwright.Helpers;

public class StoryUrlHelper
{
    private readonly AppSettings _appSettings;

    public StoryUrlHelper(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    // TODO: Protection from duplicating the externalId
    public string RandomStoryUrl => $"{_appSettings.ENVIRONMENT.STORY_URL}/works/{Random.Shared.Next(10_000, 99_999)}";
    public string FinishedStoryUrl => $"{_appSettings.ENVIRONMENT.STORY_URL}/works/{FakeStoryConstants.CompleteStoryExternalId}";
    public string NeverFinishedStoryUrl => $"{_appSettings.ENVIRONMENT.STORY_URL}/works/{FakeStoryConstants.PerpetuallyIncompleteButContinuesStoryExternalId}";
}
