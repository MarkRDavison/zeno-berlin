namespace mark.davison.berlin.web.components.Pages.Settings.User;

public partial class UserSettingsPage
{
    private bool _inProgress;

    [Inject]
    public required IClientHttpRepository ClientHttpRepository { get; set; }

    [Inject]
    public required IJSRuntime JSRuntime { get; set; }

    [Inject]
    public required ISnackbar Snackbar { get; set; }

    private async Task Export()
    {
        _inProgress = true;
        var request = new ExportCommandRequest();

        var response = await ClientHttpRepository.Post<ExportCommandResponse, ExportCommandRequest>(request, CancellationToken.None);

        if (response.SuccessWithValue)
        {
            var content = JsonSerializer.Serialize(response.Value, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await DownloadContent(content, $"fanfic-export-{DateTime.Today.ToShortDateString()}.json");
        }
        else
        {
            Snackbar.Add(string.Join(" - ", response.Errors), Severity.Error);
        }

        _inProgress = false;
    }

    private async Task DownloadContent(string content, string fileName)
    {
        var fileStream = new MemoryStream(new UTF8Encoding(true).GetBytes(content));
        using var streamRef = new DotNetStreamReference(fileStream);
        await JSRuntime.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
    }
}
