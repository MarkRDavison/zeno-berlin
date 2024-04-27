﻿using mark.davison.berlin.web.features.Store.SharedStoryUseCase;

namespace mark.davison.berlin.web.features.Store.StoryListUseCase;

public static class StoryListReducers
{
    [ReducerMethod]
    public static StoryListState FetchStoryListAction(StoryListState state, FetchStoryListAction action)
    {
        return new StoryListState(
            true,
            state.Stories,
            state.LastLoaded);
    }

    [ReducerMethod]
    public static StoryListState FetchStoryListActionResult(StoryListState state, FetchStoryListActionResponse action)
    {
        if (action.SuccessWithValue)
        {
            var stories = action.Value.ToDictionary(_ => _.StoryId, _ => _);

            foreach (var existingStory in state.Stories.Where(_ => !stories.ContainsKey(_.StoryId)))
            {
                stories.Add(existingStory.StoryId, existingStory);
            }

            return new StoryListState(false, stories.Values, DateTime.Now); // TODO: IDateService.Now???
        }

        return new StoryListState(false, state.Stories, DateTime.Now);
    }

    [ReducerMethod]
    public static StoryListState DeleteStoryListActionResponse(StoryListState state, DeleteStoryListActionResponse response)
    {
        if (response.Success)
        {
            return new StoryListState(
                state.IsLoading,
                [.. state.Stories.Where(_ => _.StoryId != response.StoryId)],
                state.LastLoaded);
        }

        return state;
    }

    [ReducerMethod]
    public static StoryListState SetFavouriteStoryListAction(StoryListState state, SetFavouriteStoryListAction action)
    {
        var story = state.Stories.FirstOrDefault(_ => _.StoryId == action.StoryId);

        if (story is not null)
        {
            story.IsFavourite = action.IsFavourite;
        }

        return new StoryListState(
            state.IsLoading,
            state.Stories,
            state.LastLoaded);
    }

    [ReducerMethod]
    public static StoryListState SetFavouriteStoryListActionResponse(StoryListState state, SetFavouriteStoryListActionResponse response)
    {
        var story = state.Stories.FirstOrDefault(_ => _.StoryId == response.StoryId);

        if (story is not null)
        {
            story.IsFavourite = response.IsFavourite;
        }

        return new StoryListState(
            state.IsLoading,
            state.Stories,
            state.LastLoaded);
    }

    [ReducerMethod]
    public static StoryListState AddStoryActionResponse(StoryListState state, AddStoryActionResponse response)
    {
        if (response.SuccessWithValue)
        {
            var newStory = new StoryRowDto
            {
                StoryId = response.Value.Id,
                Name = response.Value.Name,
                Author = "TODO",
                IsFavourite = response.Value.Favourite,
                IsComplete = response.Value.Complete,
                CurrentChapters = response.Value.CurrentChapters,
                TotalChapters = response.Value.TotalChapters,
                Fandoms = response.Value.Fandoms
            }; // TODO: Helper??? if you ever cannot make one from the other need to fetch
            return new StoryListState(
                state.IsLoading,
                [.. state.Stories.Where(_ => _.StoryId != newStory.StoryId), newStory],
                state.LastLoaded);
        }

        return state;
    }
}
