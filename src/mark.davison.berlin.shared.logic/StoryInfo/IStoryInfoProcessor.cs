﻿namespace mark.davison.berlin.shared.logic.StoryInfo;

public interface IStoryInfoProcessor
{
    string ExtractExternalStoryId(string storyAddress, string siteAddress);
    string GenerateBaseStoryAddress(string storyAddress, string siteAddress);
    Task<StoryInfoModel> ExtractStoryInfo(string storyAddress, string siteAddress, CancellationToken cancellationToken);
}
