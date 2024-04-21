namespace mark.davison.berlin.web.components;

public class Routes
{
    public const string Root = "/";
    public const string Dashboard = Root;
    public const string UserSettings = "/settings/user";
    public const string Story = "/stories/{id:guid}";
}

public class RouteHelpers
{
    public static string Story(Guid id) => Routes.Story.Replace("{id:guid}", id.ToString());
}