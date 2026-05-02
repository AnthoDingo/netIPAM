using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhpIpamNet.Domain.Entities;

/// <summary>
/// Maps to phpIPAM table `settings`. Singleton row (id=1).
/// Faithful to the source schema; many fields rarely mutate.
/// </summary>
[Table("settings")]
public class Setting
{
    [Column("id")] public int Id { get; set; }
    [Column("siteTitle"), MaxLength(64)] public string? SiteTitle { get; set; }
    [Column("siteAdminName"), MaxLength(64)] public string? SiteAdminName { get; set; }
    [Column("siteAdminMail"), MaxLength(254)] public string? SiteAdminMail { get; set; }
    [Column("siteDomain"), MaxLength(32)] public string? SiteDomain { get; set; }
    [Column("siteURL"), MaxLength(64)] public string? SiteUrl { get; set; }
    [Column("siteLoginText"), MaxLength(128)] public string? SiteLoginText { get; set; }
    [Column("domainAuth")] public bool DomainAuth { get; set; }
    [Column("enableIPrequests")] public bool EnableIpRequests { get; set; }
    [Column("enableVRF")] public bool EnableVrf { get; set; } = true;
    [Column("enableDNSresolving")] public bool EnableDnsResolving { get; set; }
    [Column("enableFirewallZones")] public bool EnableFirewallZones { get; set; }
    [Column("enablePowerDNS")] public bool EnablePowerDns { get; set; }
    [Column("enableDHCP")] public bool EnableDhcp { get; set; }
    [Column("enableMulticast")] public bool EnableMulticast { get; set; }
    [Column("enableNAT")] public bool EnableNat { get; set; } = true;
    [Column("enableSNMP")] public bool EnableSnmp { get; set; }
    [Column("enableThreshold")] public bool EnableThreshold { get; set; } = true;
    [Column("enableRACK")] public bool EnableRack { get; set; } = true;
    [Column("enableLocations")] public bool EnableLocations { get; set; } = true;
    [Column("enablePSTN")] public bool EnablePstn { get; set; }
    [Column("enableChangelog")] public bool EnableChangelog { get; set; } = true;
    [Column("enableCustomers")] public bool EnableCustomers { get; set; } = true;
    [Column("enableVaults")] public bool EnableVaults { get; set; } = true;
    [Column("link_field"), MaxLength(32)] public string? LinkField { get; set; } = "0";
    [Column("version"), MaxLength(5)] public string? Version { get; set; }
    [Column("dbversion")] public int DbVersion { get; set; }
    [Column("dbverified")] public bool DbVerified { get; set; }
    [Column("donate")] public bool Donate { get; set; }
    [Column("IPfilter"), MaxLength(128)] public string? IpFilter { get; set; }
    [Column("IPrequired"), MaxLength(128)] public string? IpRequired { get; set; }
    [Column("vlanDuplicate")] public int VlanDuplicate { get; set; }
    [Column("vlanMax")] public int VlanMax { get; set; } = 4096;
    [Column("subnetOrdering"), MaxLength(16)] public string? SubnetOrdering { get; set; } = "subnet,asc";
    [Column("visualLimit")] public int VisualLimit { get; set; }
    [Column("theme"), MaxLength(32)] public string Theme { get; set; } = "dark";
    [Column("autoSuggestNetwork")] public bool AutoSuggestNetwork { get; set; }
    [Column("pingStatus"), MaxLength(32)] public string PingStatus { get; set; } = "1800;3600";
    [Column("defaultLang")] public int? DefaultLang { get; set; }
    [Column("editDate")] public DateTime? EditDate { get; set; }
    [Column("vcheckDate")] public DateTime? VcheckDate { get; set; }
    [Column("api")] public bool Api { get; set; }
    [Column("scanPingPath"), MaxLength(64)] public string? ScanPingPath { get; set; } = "/bin/ping";
    [Column("scanFPingPath"), MaxLength(64)] public string? ScanFpingPath { get; set; } = "/bin/fping";
    [Column("scanPingType"), MaxLength(8)] public string ScanPingType { get; set; } = "ping";
    [Column("scanMaxThreads")] public int ScanMaxThreads { get; set; } = 128;
    [Column("prettyLinks"), MaxLength(3)] public string PrettyLinks { get; set; } = "No";
    [Column("hiddenCustomFields"), Column(TypeName = "text")] public string? HiddenCustomFields { get; set; }
    [Column("inactivityTimeout")] public int InactivityTimeout { get; set; } = 3600;
    [Column("updateTags")] public bool UpdateTags { get; set; }
    [Column("enforceUnique")] public bool EnforceUnique { get; set; } = true;
    [Column("authmigrated")] public byte AuthMigrated { get; set; }
    [Column("maintaneanceMode")] public bool MaintenanceMode { get; set; }
    [Column("decodeMAC")] public bool DecodeMac { get; set; } = true;
    [Column("tempShare")] public bool TempShare { get; set; }
    [Column("tempAccess"), Column(TypeName = "text")] public string? TempAccess { get; set; }
    [Column("log"), MaxLength(8)] public string Log { get; set; } = "Database";
    [Column("subnetView")] public byte SubnetView { get; set; }
    [Column("enableCircuits")] public bool EnableCircuits { get; set; } = true;
    [Column("enableRouting")] public bool EnableRouting { get; set; }
    [Column("permissionPropagate")] public bool PermissionPropagate { get; set; } = true;
    [Column("passwordPolicy"), MaxLength(1024)] public string? PasswordPolicy { get; set; }
    [Column("2fa_provider"), MaxLength(24)] public string TwoFaProvider { get; set; } = "none";
    [Column("2fa_name"), MaxLength(32)] public string TwoFaName { get; set; } = "phpipam";
    [Column("2fa_length")] public int TwoFaLength { get; set; } = 26;
    [Column("2fa_userchange")] public bool TwoFaUserChange { get; set; } = true;
    [Column("passkeys")] public bool Passkeys { get; set; } = true;
}
