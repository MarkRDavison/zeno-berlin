namespace mark.davison.berlin.web.ui.playwright.PageObjectModels.Pages;

public sealed class FandomsPage
{
    public sealed class FandomTableRow
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string ParentName { get; set; } = string.Empty;
        public string ExternalName { get; set; } = string.Empty;

        public ILocator? Fandom { get; set; }
        public ILocator? ParentFandom { get; set; }
    }

    private readonly IPage _page;

    public FandomsPage(IPage page)
    {
        _page = page;
    }

    public static async Task<FandomsPage> GotoAsync(IPage page)
    {
        var fandomsLink = page.GetByRole(AriaRole.Link, new PageGetByRoleOptions
        {
            Name = "Fandoms"// TODO: Constants
        });

        await fandomsLink.ClickAsync();

        await Assertions.Expect(page).ToHaveURLAsync(
            new Regex("/fandoms"),// TODO: Constants
            new PageAssertionsToHaveURLOptions
            {
                Timeout = 10_000.0f
            });

        var pom = new FandomsPage(page);

        return pom;
    }

    public async Task<List<FandomTableRow>> GetFandomTableInfo()
    {
        var table = _page.GetByTestId(DataTestIds.FandomsTable);

        await table.WaitForAsync();

        var rows = await table.Locator(".mud-table-body > .mud-table-row").AllAsync();

        var tableRows = new List<FandomTableRow>();

        foreach (var row in rows)
        {
            var rowData = new FandomTableRow();
            var cells = await row.Locator(".mud-table-cell").AllAsync();

            foreach (var cell in cells)
            {
                var dataLabel = await cell.GetAttributeAsync("data-label");

                if (dataLabel == "Name") // TODO: Constants
                {
                    rowData.Fandom = cell;
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
                else if (dataLabel == "External name")// TODO: Constants
                {
                    rowData.ExternalName = await cell.InnerTextAsync();
                }
                else if (dataLabel == "Parent")// TODO: Constants
                {
                    rowData.ParentFandom = cell;
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
