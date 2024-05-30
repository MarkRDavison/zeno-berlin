namespace mark.davison.berlin.web.ui.playwright.Scenarios.Smoke.Fandom;

[TestClass]
public sealed class AddFandomTests : BerlinBaseTest
{

    [TestMethod]
    public async Task CanAddManualFandom()
    {
        var fandomName = GetSentence();

        var fandomsPage = await Dashboard
            .GoToPage<FandomsPage>();

        var addFandomModal = await fandomsPage
            .OpenAddFandomModal();

        await addFandomModal.Submit(new(fandomName));

        await fandomsPage.GetFandomLinkInfoByName(fandomName);
    }

    [TestMethod]
    public async Task AddingParentFandomWorks()
    {
        var parentFandomName = GetNoun();
        var childFandomName = GetNoun();

        var fandomsPage = await Dashboard
            .GoToPage<FandomsPage>();

        var addFandomModal = await fandomsPage
            .OpenAddFandomModal()
            .ThenAsync(_ => _.Submit(new(parentFandomName)));

        addFandomModal = await fandomsPage
            .OpenAddFandomModal()
            .ThenAsync(_ => _.Submit(new(childFandomName)));

        await fandomsPage
            .ValidateTableHasFandoms(parentFandomName, childFandomName);

        var manageFandomPage = await fandomsPage
            .NavigateToFandomByName(childFandomName);

        await manageFandomPage
            .ExpectNoParentFandom();

        var editFandomModal = await manageFandomPage
            .OpenEditFandomModal();

        await editFandomModal
            .SetParent(parentFandomName)
            .ThenAsync(_ => _.Submit());

        await manageFandomPage
            .ExpectParentFandom(parentFandomName);

        manageFandomPage = await manageFandomPage
            .GoToPage<FandomsPage>()
            .ThenAsync(_ => _.NavigateToFandomByName(parentFandomName));

        await manageFandomPage
            .ExpectChildFandomContains(childFandomName);
    }
}
