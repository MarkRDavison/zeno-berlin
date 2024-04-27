namespace mark.davison.berlin.web.components.Helpers;

// TODO: Move to web.services???
public static class FandomDisplayHelpers
{
    public static string FandomIdsToSummaryString(IEnumerable<Guid> fandomIds, ICollection<FandomDto> fandoms)
    {
        var fandomsToDisplay = FandomIdsToSummaryFandoms(fandomIds, fandoms);

        var names = fandomsToDisplay.Select(_ => _.Name).Distinct();

        return string.Join(", ", names);
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
}
