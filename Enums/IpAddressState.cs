namespace netIPAM.Enums;

/// <summary>
/// Standard IP / subnet state codes used by phpIPAM.
/// Stored as INT(3) in the source schema; default 2 = Used.
/// </summary>
public enum IpAddressState
{
    Offline = 0,
    Active = 1,
    Used = 2,
    Reserved = 3,
    Dhcp = 4
}
