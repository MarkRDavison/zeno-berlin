namespace mark.davison.berlin.shared.validation.Ignition;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseValidation(this IServiceCollection services)
    {
        services.AddScoped<IValidationContext, ValidationContext>();
        return services;
    }
}
