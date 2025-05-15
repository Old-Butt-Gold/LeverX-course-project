using EER.Domain.Exceptions;

namespace EER.Domain.ValueObjects;

public sealed class RoleName
{
    public static readonly RoleName Admin = new(nameof(Admin));
    public static readonly RoleName Owner = new(nameof(Owner));
    public static readonly RoleName Customer = new(nameof(Customer));

    private static readonly List<RoleName> _all = [Admin, Owner, Customer];

    public static IReadOnlyCollection<RoleName> All => _all;
    
    public string Value { get; set; }
    
    private RoleName(string value)
    {
        Value = value;
    }

    public static RoleName FromString(string? roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            throw new DomainRuleException("Role name cannot be empty");
        }
        
        var role = roleName.Trim();

        var result = _all.FirstOrDefault(r => r.Value.Equals(role, StringComparison.OrdinalIgnoreCase));

        if (result is null)
        {
            throw new DomainRuleException($"Invalid role: {roleName}");
        }

        return result;
    }
}