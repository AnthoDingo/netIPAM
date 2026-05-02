using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhpIpamNet.Domain.Entities;

[Table("devices")]
public class Device
{
    [Column("id")] public int Id { get; set; }
    [Column("hostname"), MaxLength(255)] public string? Hostname { get; set; }
    [Column("ip_addr"), MaxLength(100)] public string? IpAddr { get; set; }
    [Column("type")] public int? Type { get; set; } = 0;
    [Column("description"), MaxLength(256)] public string? Description { get; set; }
    [Column("sections"), MaxLength(1024)] public string? Sections { get; set; }
    [Column("snmp_community"), MaxLength(100)] public string? SnmpCommunity { get; set; }
    [Column("snmp_version"), MaxLength(2)] public string? SnmpVersion { get; set; } = "0";
    [Column("snmp_port")] public int? SnmpPort { get; set; } = 161;
    [Column("snmp_timeout")] public int? SnmpTimeout { get; set; } = 1000;
    [Column("snmp_queries"), MaxLength(128)] public string? SnmpQueries { get; set; }
    [Column("snmp_v3_sec_level"), MaxLength(16)] public string? SnmpV3SecLevel { get; set; } = "none";
    [Column("snmp_v3_auth_protocol"), MaxLength(8)] public string? SnmpV3AuthProtocol { get; set; } = "none";
    [Column("snmp_v3_auth_pass"), MaxLength(64)] public string? SnmpV3AuthPass { get; set; }
    [Column("snmp_v3_priv_protocol"), MaxLength(8)] public string? SnmpV3PrivProtocol { get; set; } = "none";
    [Column("snmp_v3_priv_pass"), MaxLength(64)] public string? SnmpV3PrivPass { get; set; }
    [Column("snmp_v3_ctx_name"), MaxLength(64)] public string? SnmpV3CtxName { get; set; }
    [Column("snmp_v3_ctx_engine_id"), MaxLength(64)] public string? SnmpV3CtxEngineId { get; set; }
    [Column("rack")] public int? Rack { get; set; }
    [Column("rack_start")] public int? RackStart { get; set; }
    [Column("rack_size")] public int? RackSize { get; set; }
    [Column("location")] public int? Location { get; set; }
    [Column("editDate")] public DateTime? EditDate { get; set; }
}
