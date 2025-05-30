namespace EER.API.CustomAttributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class AddHeaderAttribute : Attribute
{
    public string Name { get; }
    public string Value { get; }
    public bool Overwrite { get; }

    public AddHeaderAttribute(string name, string value, bool overwrite = false)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Value = value ?? throw new ArgumentNullException(nameof(value));
        Overwrite = overwrite;
    }
}
