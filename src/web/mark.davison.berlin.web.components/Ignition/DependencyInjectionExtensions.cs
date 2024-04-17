namespace mark.davison.berlin.web.components.Ignition;

public static class DependecyInjectionExtensions
{
    public static IServiceCollection UseBerlinComponents(this IServiceCollection services)
    {
        services.AddMudServices();

        // ModalViewModel<TFormViewModel, TForm> : IModalViewModel<TFormViewModel, TForm>

        services.AddTransient(typeof(ModalViewModel<,>));

        return services;
    }
}