﻿namespace mark.davison.berlin.shared.commands.test.Scenarios.UpdateStories;

[TestClass]
public class UpdateStoriesCommandProcessorTests
{
    private readonly IRepository _repository;
    private readonly IDateService _dateService;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly UpdateStoriesCommandProcessor _processor;

    private readonly IStoryInfoProcessor _site1StoryInfoProcessor;
    private readonly IStoryInfoProcessor _site2StoryInfoProcessor;

    private readonly User _user;
    private readonly Site _site1;
    private readonly Site _site2;
    private readonly Story _story1Site1;
    private readonly Story _story1Site2;
    private readonly Story _story2Site1;
    private readonly Story _story2Site2;

    public UpdateStoriesCommandProcessorTests()
    {

        _repository = Substitute.For<IRepository>();
        _dateService = Substitute.For<IDateService>();
        _site1StoryInfoProcessor = Substitute.For<IStoryInfoProcessor>();
        _site2StoryInfoProcessor = Substitute.For<IStoryInfoProcessor>();
        _currentUserContext = Substitute.For<ICurrentUserContext>();

        _dateService.Now.Returns(DateTime.Now);

        _user = new User
        {
            Id = Guid.NewGuid()
        };
        _site1 = new()
        {
            Id = Guid.NewGuid(),
            ShortName = "SOMESITE1",
            Address = "https://somesite1.org"
        };
        _site2 = new()
        {
            Id = Guid.NewGuid(),
            ShortName = "SOMESITE2",
            Address = "https://somesite2.org"
        };
        _story1Site1 = new Story
        {
            Id = Guid.NewGuid(),
            SiteId = _site1.Id,
            LastModified = _dateService.Now.AddDays(-2)
        };
        _story2Site1 = new Story
        {
            Id = Guid.NewGuid(),
            SiteId = _site1.Id,
            LastModified = _dateService.Now.AddDays(-1.5)
        };
        _story1Site2 = new Story
        {
            Id = Guid.NewGuid(),
            SiteId = _site2.Id,
            LastModified = _dateService.Now
        };
        _story2Site2 = new Story
        {
            Id = Guid.NewGuid(),
            SiteId = _site2.Id,
            LastModified = _dateService.Now
        };

        _repository
            .BeginTransaction()
            .Returns(new StubAsyncDisposable());

        _currentUserContext
            .CurrentUser
            .Returns(_ => _user);

        var services = new ServiceCollection();
        services.AddKeyedTransient(_site1.ShortName, (_, __) => _site1StoryInfoProcessor);
        services.AddKeyedTransient(_site2.ShortName, (_, __) => _site2StoryInfoProcessor);

        _repository
            .GetEntityAsync<Site>(
                Arg.Any<Guid>(),
                Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromResult(new List<Site>
            {
                _site1,
                _site2
            }.FirstOrDefault(__ => __.Id == _.Arg<Guid>())));

        _processor = new(_repository, _dateService, services.BuildServiceProvider());
    }

    [TestMethod]
    public async Task ProcessAsync_WhereNoStoryIdsProvided_QueriesStoriesBasedOnLastModified()
    {
        var stories = new List<Story>
        {
            _story1Site1,
            _story1Site2,
            _story2Site1,
            _story2Site2
        };

        stories[0].LastModified = _dateService.Now.AddDays(-3);
        stories[1].LastModified = _dateService.Now.AddDays(-2);
        stories[2].LastModified = _dateService.Now.AddDays(-1);
        stories[3].LastModified = _dateService.Now.AddDays(-0);

        _repository
            .GetEntitiesAsync<Story>(
                Arg.Any<Expression<Func<Story, bool>>>(),
                Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                var predicate = _.Arg<Expression<Func<Story, bool>>>().Compile();

                var s = stories.Where(predicate).ToList();

                Assert.AreEqual(3, s.Count);

                return Task.FromResult(new List<Story>());
            });

        var request = new UpdateStoriesRequest();

        var response = await _processor.ProcessAsync(request, _currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.Success);

        await _repository
            .Received(1)
            .GetEntitiesAsync<Story>(
                Arg.Any<Expression<Func<Story, bool>>>(),
                Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task ProcessAsync_WhereStoryIdsProvided_QueriesStoriesBasedOnProvidedIds()
    {
        var stories = new List<Story>
        {
            _story1Site1,
            _story1Site2,
            _story2Site1,
            _story2Site2
        };

        var request = new UpdateStoriesRequest
        {
            StoryIds = [.. stories.Select(_ => _.Id)]
        };

        _repository
            .GetEntitiesAsync<Story>(
                Arg.Any<Expression<Func<Story, bool>>>(),
                Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                var predicate = _.Arg<Expression<Func<Story, bool>>>().Compile();


                var count = stories.Count(predicate);

                Assert.AreEqual(stories.Count, count);

                return Task.FromResult(new List<Story>());
            });

        var response = await _processor.ProcessAsync(request, _currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.Success);
        Assert.IsTrue(response.Warnings.Any(_ => _ == ValidationMessages.NO_ITEMS));

        await _repository
            .Received(1)
            .GetEntitiesAsync<Story>(
                Arg.Any<Expression<Func<Story, bool>>>(),
                Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task ProcessAsync_InvokesStoryInfoProcessor_PerSiteGrouping()
    {
        var stories = new List<Story>
        {
            _story1Site1,
            _story1Site2,
            _story2Site1,
            _story2Site2
        };

        var request = new UpdateStoriesRequest
        {
            StoryIds = [.. stories.Select(_ => _.Id)]
        };

        _repository
            .GetEntitiesAsync<Story>(
                Arg.Any<Expression<Func<Story, bool>>>(),
                Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromResult(stories));

        _site1StoryInfoProcessor
            .ExtractStoryInfo(
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(new StoryInfoModel());

        _site2StoryInfoProcessor
            .ExtractStoryInfo(
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(new StoryInfoModel());

        var response = await _processor.ProcessAsync(request, _currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.Success);

        await _site1StoryInfoProcessor
            .Received(2)
            .ExtractStoryInfo(
                Arg.Any<string>(),
                Arg.Any<CancellationToken>());

        await _site2StoryInfoProcessor
            .Received(2)
            .ExtractStoryInfo(
                Arg.Any<string>(),
                Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task ProcessAsync_UpdatesStoryInfoOnStories()
    {

        var request = new UpdateStoriesRequest
        {
            StoryIds = [_story1Site1.Id]
        };

        _repository
            .GetEntitiesAsync<Story>(
                Arg.Any<Expression<Func<Story, bool>>>(),
                Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromResult<List<Story>>([_story1Site1]));

        var info = new StoryInfoModel { };

        _site1StoryInfoProcessor
            .ExtractStoryInfo(
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(info);

        _repository
            .UpsertEntitiesAsync<Story>(
                Arg.Is<List<Story>>(_ =>
                    _.Count == 1 &&
                    _[0].Id == _story1Site1.Id &&
                    _[0].TotalChapters == info.TotalChapters &&
                    _[0].CurrentChapters == info.CurrentChapters &&
                    _[0].Complete == info.IsCompleted &&
                    _[0].Name == info.Name &&
                    _[0].LastModified == _dateService.Now),
                Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromResult(_.Arg<List<Story>>()));

        var response = await _processor.ProcessAsync(request, _currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.Success);

        await _repository
            .Received(1)
            .UpsertEntitiesAsync<Story>(
                Arg.Any<List<Story>>(),
                Arg.Any<CancellationToken>());
    }
}
