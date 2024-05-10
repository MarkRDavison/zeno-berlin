﻿using mark.davison.berlin.shared.models.dtos.Scenarios.Commands.SendNotification;
using mark.davison.berlin.web.services.CommonCandidates;

namespace mark.davison.berlin.web.components.Pages.Settings.User;

public partial class UserSettingsPage
{
    private bool _inProgress;

    [Inject]
    public required IClientHttpRepository ClientHttpRepository { get; set; }
    [Inject]
    public required IClientJobHttpRepository ClientJobHttpRepository { get; set; }

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
            UseJob = true,
            TriggerImmediateJob = true
        };

        var response = await ClientJobHttpRepository.PostLongPolling<ExportCommandResponse, ExportCommandRequest, SerialisedtDataDto>(
            request,
            TimeSpan.FromSeconds(2),
            CancellationToken.None);

        if (response.SuccessWithValue)
        {
            var content = JsonSerializer.Serialize(response.Value, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            var filename = $"fanfic_export_{DateTime.Today.ToShortDateString()}_{DateTime.Now.ToLocalTime().ToLongTimeString()}.json".Replace(" ", "_").Replace(":", "_");
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
        return await ClientJobHttpRepository.PostSetupBackgroundJob<ImportCommandResponse, ImportCommandRequest, ImportSummary>(
            new ImportCommandRequest
            {
                UseJob = true,
                TriggerImmediateJob = true,
                Data = data
            },
            (ImportCommandResponse response) =>
            {
                if (response.SuccessWithValue)
                {
                    if (response.Value.Imported == 0)
                    {
                        Snackbar.Add("No new stories imported", Severity.Normal);
                    }
                    else
                    {
                        Snackbar.Add($"Imported {response.Value.Imported} stories", Severity.Success);
                    }
                }
                else
                {
                    Snackbar.Add("Failed to import stories", Severity.Error);
                    Console.Error.WriteLine(string.Join(", ", response.Errors));
                }

                return Task.CompletedTask;
            },
            CancellationToken.None);


    }
}
