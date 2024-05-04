namespace mark.davison.shared.server.services.Helpers;

public class NotificationCreationService : INotificationCreationService
{
    public string CreateNotification(Story story, List<StoryInfoModel> updates, Site site)
    {
        var builder = new StringBuilder();

        var updateChapterNumbers = updates.Select(_ => _.CurrentChapters).OrderByDescending(_ => _).ToList();

        builder.AppendLine("==================== STORY UPDATED ====================");
        builder.AppendLine("|");
        builder.AppendLine($"|  Name: {story.Name}");
        builder.AppendLine($"|  Chapters: {string.Join(", ", updateChapterNumbers)}");
        builder.AppendLine($"|  Status: {(story.Complete ? "Complete" : "In progress")}");
        builder.AppendLine($"|  Site: {site.LongName}({site.ShortName})");
        builder.AppendLine($"|  Url: {story.Address}");
        builder.AppendLine("|");
        builder.AppendLine("=======================================================");

        return builder.ToString();
    }

    public string CreateNotification(Site site, Story story, StoryInfoModel info)
    {
        var builder = new StringBuilder();

        builder.AppendLine("==================== STORY UPDATED ====================");
        builder.AppendLine("|");
        builder.AppendLine($"|  Name: {info.Name}{(info.Name == story.Name ? string.Empty : $" - previously {story.Name}")}");
        builder.AppendLine($"|  Chapters: {info.CurrentChapters}/{(info.TotalChapters?.ToString() ?? "?")} - previously {story.CurrentChapters}/{(story.TotalChapters?.ToString() ?? "?")}");
        builder.AppendLine($"|  Status: {(info.IsCompleted ? "Complete" : "In progress")} - previously {(story.Complete ? "Complete" : "In progress")}");
        builder.AppendLine($"|  Site: {site.LongName}({site.ShortName})");
        builder.AppendLine($"|  Url: {story.Address}");
        builder.AppendLine("|");
        builder.AppendLine("=======================================================");

        return builder.ToString();
    }

}
