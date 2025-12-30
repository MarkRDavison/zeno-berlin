using mark.davison.berlin.shared.models.dto.Scenarios.Commands.UpdateStories;

namespace mark.davison.berlin.web.components.Pages;

public partial class AdminSettings
{
    private bool _inProgress;
    private AuthenticationState? _authState;

    [Inject]
    public required IClientHttpRepository ClientHttpRepository { get; set; }

    [Inject]
    public required ISnackbar Snackbar { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _authState = await _authStateProvider.GetAuthenticationStateAsync();
    }

    public async Task TriggerUpdateStories()
    {
        var request = new UpdateStoriesRequest();
        var response = await ClientHttpRepository.Post<UpdateStoriesRequest, UpdateStoriesResponse>(request, CancellationToken.None);



        if (response.SuccessWithValue)
        {
            Snackbar.Add($"Updated {response.Value.Count} stories", Severity.Success);
        }
        else if (response.Warnings.Contains("NO_ITEMS"))
        {
            Snackbar.Add("Update successful but no stories need updating", Severity.Info);
        }
        else
        {
            Snackbar.Add($"Failed to update stories: {string.Join(", ", response.Errors)}", Severity.Error);
        }
    }
}
