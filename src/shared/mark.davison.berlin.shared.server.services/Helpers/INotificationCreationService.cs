namespace mark.davison.berlin.shared.server.services.Helpers;

public interface INotificationCreationService
{
    string CreateNotification(Story story, List<StoryInfoModel> updates, Site site);
    string CreateNotification(Site site, Story story, StoryInfoModel info);
}
