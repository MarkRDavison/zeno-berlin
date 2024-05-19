namespace mark.davison.berlin.web.ui.playwright.Scenarios.Smoke.Fandom;

[TestClass]
public sealed class AddFandomTests : LoggedInTest
{
    [TestMethod]
    public async Task AddingManualFandomWorks()
    {
        var fandomPage = await FandomsPage.GotoAsync(CurrentPage);

        var fandomName = GetNoun();

        var modal = await AddFandomModal.GotoAsync(CurrentPage);
        await modal.AddAsync(fandomName);

        await Task.Delay(500);
        var rows = await fandomPage.GetFandomTableInfo();

        Assert.AreEqual(1, rows.Count);
        Assert.AreEqual(fandomName, rows[0].Name);
    }

    [TestMethod]
    public async Task AddingParentFandomWorks()
    {
        var fandomsPage = await FandomsPage.GotoAsync(CurrentPage);

        var parentFandomName = GetNoun();
        var childFandomName = GetNoun();

        var modal = await AddFandomModal.GotoAsync(CurrentPage);
        await modal.AddAsync(parentFandomName);
        modal = await AddFandomModal.GotoAsync(CurrentPage);
        await modal.AddAsync(childFandomName);

        await Task.Delay(500);

        var rows = await fandomsPage.GetFandomTableInfo();

        var parentFandom = rows.First(_ => _.Name.Equals(parentFandomName));
        var childFandom = rows.First(_ => _.Name.Equals(childFandomName));

        Assert.IsNotNull(childFandom.Fandom);

        await childFandom.Fandom.GetByRole(AriaRole.Link).ClickAsync();

        var fandomPage = await ManageFandomPage.GotoAsync(CurrentPage);

        await fandomPage.ExpectNoParentFandom();

        var editFandomModal = await fandomPage.EditFandomAsync();

        await editFandomModal.SetParent(parentFandomName);

        await editFandomModal.Save();

        await fandomPage.ExpectParentFandom(parentFandomName);

        fandomsPage = await FandomsPage.GotoAsync(CurrentPage);

        await Task.Delay(500);

        rows = await fandomsPage.GetFandomTableInfo();

        parentFandom = rows.First(_ => _.Name.Equals(parentFandomName));
        childFandom = rows.First(_ => _.Name.Equals(childFandomName));

        Assert.AreEqual(childFandom.ParentName, parentFandomName);
    }
}
