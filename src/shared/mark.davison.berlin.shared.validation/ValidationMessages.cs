namespace mark.davison.berlin.shared.validation;

public static class ValidationMessages
{
    public const string ERROR_SAVING = nameof(ERROR_SAVING);
    public const string ERROR_DELETING = nameof(ERROR_DELETING);
    public const string FAILED_TO_FIND_ENTITY = nameof(FAILED_TO_FIND_ENTITY);
    public const string INVALID_PROPERTY = nameof(INVALID_PROPERTY);
    public const string DUPLICATE_ENTITY = nameof(DUPLICATE_ENTITY);
    public const string UNSUPPORTED_SITE = nameof(UNSUPPORTED_SITE);
    public const string SITE_STORY_MISMATCH = nameof(SITE_STORY_MISMATCH);
    public const string NO_ITEMS = nameof(NO_ITEMS);
    public const string FAILED_TO_IMPORT = nameof(FAILED_TO_IMPORT);
    public const string OWNERSHIP_MISMATCH = nameof(OWNERSHIP_MISMATCH);
    public const string FAILED_RETRIEVE = nameof(FAILED_RETRIEVE);


    public static string FormatMessageParameters(string message, params string[] parameters)
    {
        if (parameters.Length == 0)
        {
            return message;
        }
        var parametersSegment = string.Join('&', parameters);
        return message + '&' + parametersSegment;
    }

    public static T CreateErrorResponse<T>(string message, params string[] parameters) where T : Response, new()
    {
        return new T()
        {
            Errors = [
                FormatMessageParameters(message, parameters)
            ]
        };
    }
}
