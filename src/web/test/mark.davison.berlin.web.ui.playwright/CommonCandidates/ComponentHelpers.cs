namespace mark.davison.berlin.web.ui.playwright.CommonCandidates;

public static class ComponentHelpers
{
    public static async Task SetAutoComplete(IPage page, string label, string value)
    {
        var parentFandomAutocomplete = page.GetByLabel(label);

        await parentFandomAutocomplete.ClickAsync();
        await parentFandomAutocomplete.PressSequentiallyAsync(value);

        var popupOptions = await page.Locator(".mud-popover-open p").AllAsync();

        foreach (var option in popupOptions)
        {
            var text = await option.TextContentAsync();

            if (text == value)
            {
                await option.ClickAsync();
            }
        }
    }
}
