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

        var response = await ClientHttpRepository.Post<ExportCommandResponse, ExportCommandRequest>(CancellationToken.None);

        if (response.SuccessWithValue)
        {
            var content = JsonSerializer.Serialize(response.Value, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await DownloadContent(content, $"fanfic_export_{DateTime.Today.ToShortDateString()}.json");
        }
        else
        {
            response.Errors.ForEach(_ => Snackbar.Add(_, Severity.Error));
        }

        response.Warnings.ForEach(_ => Snackbar.Add(_, Severity.Warning));

        _inProgress = false;
    }

    private async Task DownloadContent(string content, string fileName)
    {
        var fileStream = new MemoryStream(new UTF8Encoding(true).GetBytes(content));
        using var streamRef = new DotNetStreamReference(fileStream);
        await JSRuntime.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
    }
}
