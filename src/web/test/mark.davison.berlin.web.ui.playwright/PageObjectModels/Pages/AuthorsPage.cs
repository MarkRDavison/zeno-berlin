namespace mark.davison.berlin.web.ui.playwright.PageObjectModels.Pages;


public sealed class AuthorsPage
{
    public sealed class AuthorTableRow
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string ParentName { get; set; } = string.Empty;

        public ILocator? Author { get; set; }
        public ILocator? ParentAuthor { get; set; }
    }

    private readonly IPage _page;

    public AuthorsPage(IPage page)
    {
        _page = page;
    }

    public static async Task<AuthorsPage> GotoAsync(IPage page)
    {
        var authorsLink = page.GetByRole(AriaRole.Link, new PageGetByRoleOptions
        {
            Name = "Authors"// TODO: Constants
        });

        await authorsLink.ClickAsync();

        await Assertions.Expect(page).ToHaveURLAsync(
            new Regex("/authors"),// TODO: Constants
            new PageAssertionsToHaveURLOptions
            {
                Timeout = 10_000.0f
            });

        var pom = new AuthorsPage(page);

        return pom;
    }

    public async Task<List<AuthorTableRow>> GetAuthorTableInfo()
    {
        var table = _page.GetByTestId(DataTestIds.AuthorsTable);

        await table.WaitForAsync();

        var rows = await table.Locator(".mud-table-body > .mud-table-row").AllAsync();

        var tableRows = new List<AuthorTableRow>();

        foreach (var row in rows)
        {
            var rowData = new AuthorTableRow();
            var cells = await row.Locator(".mud-table-cell").AllAsync();

            foreach (var cell in cells)
            {
                var dataLabel = await cell.GetAttributeAsync("data-label");

                if (dataLabel == "Name") // TODO: Constants
                {
                    rowData.Author = cell;
                    rowData.Name = await cell.InnerTextAsync();
                    var nameLink = await cell.GetByRole(AriaRole.Link).GetAttributeAsync("href");

                    if (!string.IsNullOrEmpty(nameLink))
                    {
                        if (Guid.TryParse(nameLink.Split('/').Last(), out var id))
                        {
                            rowData.Id = id;
                        }
                    }
                }
                else if (dataLabel == "Parent")// TODO: Constants
                {
                    rowData.ParentAuthor = cell;
                    rowData.ParentName = await cell.InnerTextAsync();
                    if (!string.IsNullOrEmpty(rowData.ParentName))
                    {
                        var parentNameLink = await cell.GetByRole(AriaRole.Link).GetAttributeAsync("href");

                        if (!string.IsNullOrEmpty(parentNameLink))
                        {
                            if (Guid.TryParse(parentNameLink.Split('/').Last(), out var id))
                            {
                                rowData.Id = id;
                            }
                        }
                    }
                }
            }

            tableRows.Add(rowData);
        }

        return tableRows;
    }
}
