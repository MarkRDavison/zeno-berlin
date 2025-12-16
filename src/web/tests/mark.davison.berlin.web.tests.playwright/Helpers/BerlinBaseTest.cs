namespace mark.davison.berlin.web.tests.playwright.Helpers;

public abstract class BerlinBaseTest : LoggedInTest
{
    protected DashboardPage Dashboard => new(CurrentPage, AppSettings);
}
