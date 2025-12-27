namespace mark.davison.berlin.web.components.Pages.Story;

public partial class Story : IDisposable
{
    private bool disposedValue;

    [Parameter]
    public required Guid Id { get; set; }

    [Inject]
    public required IStoryViewModel ViewModel { get; set; }

    protected override void OnParametersSet()
    {
        if (ViewModel.Initialise(Id))
        {
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged();
    }

    private void OnPropertyChanged()
    {
        _ = InvokeAsync(StateHasChanged);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
                ViewModel.Dispose(); // Or will DI do this?
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
