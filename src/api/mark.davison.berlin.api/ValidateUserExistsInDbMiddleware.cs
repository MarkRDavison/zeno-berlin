namespace mark.davison.berlin.api;

public sealed class ValidateUserExistsInDbMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AppSettings _appSettings;

    public ValidateUserExistsInDbMiddleware(
        RequestDelegate next,
        IOptions<AppSettings> appSettings)
    {
        _next = next;
        _appSettings = appSettings.Value;
    }

    public async Task Invoke(HttpContext context)
    {
        if (!_appSettings.PRODUCTION_MODE)
        {
            var currentUserContext = context.RequestServices.GetRequiredService<ICurrentUserContext>();
            if (currentUserContext.CurrentUser != null)
            {
                var dbContext = context.RequestServices.GetRequiredService<IDbContext>();

                var user = await dbContext.GetByIdAsync<User>(currentUserContext.CurrentUser.Id, CancellationToken.None);

                if (user == null)
                {
                    await dbContext.UpsertEntityAsync(currentUserContext.CurrentUser, CancellationToken.None);
                    await dbContext.SaveChangesAsync(CancellationToken.None);
                }
            }
        }

        await _next(context);
    }
}