namespace mark.davison.berlin.web.tests.playwright.Scenarios.Smoke.Author;

public sealed class AddAuthorTests : BerlinBaseTest
{
    [Test]
    [Skip("Feature not added yet.")]
    public async Task CanAddManualAuthor()
    {
        var authorName = GetSentence(2);

        var authorsPage = await Dashboard
            .GoToPage<AuthorsPage>();

        var addAuthorModal = await authorsPage
            .OpenAddAuthorModal();

        await addAuthorModal.Submit(new(authorName));

        await authorsPage.GetAuthorLinkInfoByName(authorName);
    }
}
