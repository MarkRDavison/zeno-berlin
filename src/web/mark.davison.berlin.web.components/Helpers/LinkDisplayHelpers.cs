namespace mark.davison.berlin.web.components.Helpers;

// TODO: Move to web.services???
public static class LinkDisplayHelpers
{
    public static string FandomIdsToSummaryString(IEnumerable<Guid> fandomIds, ICollection<FandomDto> fandoms)
    {
        var fandomsToDisplay = FandomIdsToSummaryFandoms(fandomIds, fandoms);

        var names = fandomsToDisplay.Select(_ => _.Name).Distinct();

        return string.Join(", ", names);
    }

    public static FandomDto? GetFandom(Guid id, bool displayParent, ICollection<FandomDto> fandoms)
    {
        if (fandoms.FirstOrDefault(_ => _.FandomId == id) is not { } fandom)
        {
            return null;
        }

        if (displayParent)
        {
            return GetParentRecursively(fandom, fandoms);
        }

        return fandom;
    }

    public static AuthorDto? GetAuthor(Guid id, bool displayParent, ICollection<AuthorDto> authors)
    {
        if (authors.FirstOrDefault(_ => _.AuthorId == id) is not { } author)
        {
            return null;
        }

        if (displayParent)
        {
            return GetParentRecursively(author, authors);
        }

        return author;
    }

    public static IEnumerable<FandomDto> FandomIdsToSummaryFandoms(IEnumerable<Guid> fandomIds, ICollection<FandomDto> fandoms)
    {
        var fandomsToDisplay = new List<FandomDto>();

        foreach (var fid in fandomIds)
        {
            if (fandoms.FirstOrDefault(_ => _.FandomId == fid) is FandomDto fandom)
            {
                fandomsToDisplay.Add(GetParentRecursively(fandom, fandoms));
            }
        }

        return fandomsToDisplay.DistinctBy(_ => _.FandomId);
    }

    public static IEnumerable<FandomDto> FandomIdsToFandoms(IEnumerable<Guid> fandomIds, ICollection<FandomDto> fandoms)
    {
        var fandomsToDisplay = new List<FandomDto>();

        foreach (var fid in fandomIds)
        {
            if (fandoms.FirstOrDefault(_ => _.FandomId == fid) is FandomDto fandom)
            {
                fandomsToDisplay.Add(fandom);
            }
        }

        return fandomsToDisplay.DistinctBy(_ => _.FandomId);
    }

    private static FandomDto GetParentRecursively(FandomDto current, ICollection<FandomDto> fandoms)
    {
        if (current.ParentFandomId == null)
        {
            return current;
        }

        if (fandoms.FirstOrDefault(_ => _.FandomId == current.ParentFandomId) is FandomDto parentFandom)
        {
            return GetParentRecursively(parentFandom, fandoms);
        }

        return current;
    }


    public static string AuthorIdsToSummaryString(IEnumerable<Guid> authorIds, ICollection<AuthorDto> authors)
    {
        var authorsToDisplay = AuthorIdsToSummaryAuthors(authorIds, authors);

        var names = authorsToDisplay.Select(_ => _.Name).Distinct();

        return string.Join(", ", names);
    }

    public static IEnumerable<AuthorDto> AuthorIdsToSummaryAuthors(IEnumerable<Guid> authorIds, ICollection<AuthorDto> authors)
    {
        var authorsToDisplay = new List<AuthorDto>();

        foreach (var fid in authorIds)
        {
            if (authors.FirstOrDefault(_ => _.AuthorId == fid) is AuthorDto author)
            {
                authorsToDisplay.Add(GetParentRecursively(author, authors));
            }
        }

        return authorsToDisplay.DistinctBy(_ => _.AuthorId);
    }

    public static IEnumerable<AuthorDto> AuthorIdsToAuthors(IEnumerable<Guid> authorIds, ICollection<AuthorDto> authors)
    {
        var authorsToDisplay = new List<AuthorDto>();

        foreach (var fid in authorIds)
        {
            if (authors.FirstOrDefault(_ => _.AuthorId == fid) is AuthorDto author)
            {
                authorsToDisplay.Add(author);
            }
        }

        return authorsToDisplay.DistinctBy(_ => _.AuthorId);
    }

    private static AuthorDto GetParentRecursively(AuthorDto current, ICollection<AuthorDto> authors)
    {
        if (current.ParentAuthorId == null)
        {
            return current;
        }

        if (authors.FirstOrDefault(_ => _.AuthorId == current.ParentAuthorId) is AuthorDto parentAuthor)
        {
            return GetParentRecursively(parentAuthor, authors);
        }

        return current;
    }
}
