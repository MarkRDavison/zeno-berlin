namespace mark.davison.berlin.api.jobs.Services;

public class JobCurrentUserContext : ICurrentUserContext
{
    private readonly HashSet<string> _roles = [];

    public bool IsAuthenticated { get; private set; }
    public Guid UserId { get; private set; }
    public Guid TenantId { get; private set; }
    public bool HasRole(string role) => true;

    public Task<ClaimsPrincipal> PopulateFromPrincipal(ClaimsPrincipal principal, string provider)
    {
        _roles.Clear();

        IsAuthenticated = principal.Identity!.IsAuthenticated;
        UserId = Guid.Parse(principal.FindFirstValue(AuthConstants.InternalUserId) ?? throw new InvalidOperationException());
        TenantId = Guid.Parse(principal.FindFirstValue(AuthConstants.TenantId) ?? throw new InvalidOperationException());

        foreach (var r in principal.FindAll(ClaimTypes.Role))
        {
            _roles.Add(r.Value);
        }

        return Task.FromResult(principal);
    }
}
