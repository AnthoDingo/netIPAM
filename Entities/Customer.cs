using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace netIPAM.Entities;

[Table("customers")]
public class Customer
{
    [Column("id")] public int Id { get; set; }
    [Column("title"), Required, MaxLength(128)] public string Title { get; set; } = string.Empty;
    [Column("address"), MaxLength(255)] public string? Address { get; set; }
    [Column("postcode"), MaxLength(32)] public string? Postcode { get; set; }
    [Column("city"), MaxLength(255)] public string? City { get; set; }
    [Column("state"), MaxLength(255)] public string? State { get; set; }
    [Column("lat"), MaxLength(31)] public string? Lat { get; set; }
    [Column("long"), MaxLength(31)] public string? Long { get; set; }
    [Column("contact_person"), Column(TypeName = "text")] public string? ContactPerson { get; set; }
    [Column("contact_phone"), MaxLength(32)] public string? ContactPhone { get; set; }
    [Column("contact_mail"), MaxLength(254)] public string? ContactMail { get; set; }
    [Column("note"), Column(TypeName = "text")] public string? Note { get; set; }
    [Column("status"), MaxLength(16)] public string? Status { get; set; } = "Active";
}
