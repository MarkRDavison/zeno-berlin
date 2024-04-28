namespace mark.davison.berlin.web.components;

public class Routes
{
    public const string Root = "/";
    public const string Dashboard = Root;
    public const string UserSettings = "/settings/user";
    public const string Stories = "/stories";
    public const string Story = "/stories/{id:guid}";
    public const string Fandoms = "/fandoms";
    public const string Fandom = "/fandoms/{id:guid}";
    public const string Authors = "/authors";
    public const string Author = "/authors/{id:guid}";
}

public class RouteHelpers
{
    public static string Story(Guid id) => Routes.Story.Replace("{id:guid}", id.ToString());
    public static string Fandom(Guid id) => Routes.Fandom.Replace("{id:guid}", id.ToString());
    public static string Author(Guid id) => Routes.Author.Replace("{id:guid}", id.ToString());
}