namespace mark.davison.berlin.web.components.CommonCandidates.FileUpload;

public partial class FileUploadDialog
{

    private bool _inProgress;
    public bool _primaryDisabled => _inProgress;

    [CascadingParameter, EditorRequired]
    public required MudDialogInstance MudDialog { get; set; }

    [Parameter]
    public string PrimaryText { get; set; } = "Ok";

    [Parameter, EditorRequired]
    public required Func<Task<Response>> PrimaryCallback { get; set; }

    [Parameter, EditorRequired]
    public required Color Color { get; set; }

    private async Task Primary()
    {
        _inProgress = true;

        var response = await PrimaryCallback();

        if (response.Success)
        {
            MudDialog.Close(DialogResult.Ok(true));
        }
        else
        {
            Console.Error.WriteLine("TODO: How to handle errors in FileUploadDialog.Primary, add to message bar/toast???");
            response.Errors.ForEach(Console.Error.WriteLine);
            response.Warnings.ForEach(Console.WriteLine);
        }

        _inProgress = false;
    }
    private void Secondary() => MudDialog.Cancel();
}
