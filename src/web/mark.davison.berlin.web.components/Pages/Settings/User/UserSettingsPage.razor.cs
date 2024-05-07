using mark.davison.berlin.shared.models.dtos.Scenarios.Commands.SendNotification;

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

    [Inject]
    public required IDialogService DialogService { get; set; }

    private async Task Export()
    {
        _inProgress = true;

        var request = new ExportCommandRequest
        {
            UseJob = true
        };


        var response = await ClientHttpRepository.Post<ExportCommandResponse, ExportCommandRequest>(request, CancellationToken.None);

        if (!response.Success || response.JobId == null)
        {
            Console.Error.WriteLine("Failed to submit the job as expected");
            return;
        }

        request.JobId = response.JobId;

        int maxTime = 500;
        const int Delay = 2;
        while (maxTime > 0)
        {
            await Task.Delay(TimeSpan.FromSeconds(Delay));
            maxTime -= Delay;

            response = await ClientHttpRepository.Post<ExportCommandResponse, ExportCommandRequest>(request, CancellationToken.None);

            if (response.JobStatus == "Complete" ||
                response.JobStatus == "Errored")
            {
                break;
            }
        }

        if (response.SuccessWithValue)
        {
            var content = JsonSerializer.Serialize(response.Value, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            var filename = $"fanfic_export_{DateTime.Today.ToShortDateString()}_{DateTime.Now.ToLocalTime().ToLongTimeString()}.json".Replace(" ", "_");
            await DownloadContent(content, filename);
            Snackbar.Add($"Exported '{filename}'", Severity.Success);
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

    private async Task TestNotifications()
    {
        var response = await ClientHttpRepository.Post<SendNotificationCommandResponse, SendNotificationCommandRequest>(new SendNotificationCommandRequest
        {
            Message = "Test notification from client side :)"
        }, CancellationToken.None);

        if (response.Errors.Any())
        {
            Console.Error.WriteLine(string.Join(", ", response.Errors));
        }
        if (response.Warnings.Any())
        {
            Console.WriteLine(string.Join(", ", response.Warnings));
        }
    }

    private async Task OpenImportDialog()
    {
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = false
        };

        var param = new DialogParameters<FileUploadDialog<SerialisedtDataDto>>
        {
            { _ => _.PrimaryText, "Import stories" },
            { _ => _.Color, Color.Primary },
            { _ => _.PrimaryCallback, ImportData },
            { _ => _.CorrectFileDescriptionCallback, _ => $"Found {_.Stories.Count} stories to import" }
        };

        var dialog = DialogService.Show<FileUploadDialog<SerialisedtDataDto>>("Import stories", param, options);

        var result = await dialog.Result;
    }

    private async Task<Response> ImportData(SerialisedtDataDto data)
    {
        var request = new ImportCommandRequest
        {
            Data = data
        };

        var response = await ClientHttpRepository.Post<ImportCommandResponse, ImportCommandRequest>(request, CancellationToken.None);

        if (response.Success)
        {
            if (response.Imported == 0)
            {
                Snackbar.Add("No new stories imported", Severity.Normal);
            }
            else
            {
                Snackbar.Add($"Imported {response.Imported} stories", Severity.Success);
            }
        }

        return response;
    }
}
