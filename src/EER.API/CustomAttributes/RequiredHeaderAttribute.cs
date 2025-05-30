namespace EER.API.CustomAttributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequiredHeaderAttribute : Attribute
{
    public string HeaderName { get; }
    public string[]? AllowedValues { get; }
    public bool IgnoreCase { get; set; }

    public RequiredHeaderAttribute(string headerName, bool ignoreCase = true, params string[] allowedValues)
    {
        HeaderName = headerName;
        AllowedValues = allowedValues;
        IgnoreCase = ignoreCase;
    }
}
