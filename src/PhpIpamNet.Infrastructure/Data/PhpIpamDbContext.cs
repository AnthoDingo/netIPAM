using Microsoft.EntityFrameworkCore;
using PhpIpamNet.Domain.Entities;

namespace PhpIpamNet.Infrastructure.Data;

/// <summary>
/// DbContext unique commun aux providers SQL Server et SQLite.
/// Le choix du provider se fait au niveau de la composition (Program.cs ou design-time factory).
/// L'assemblage des migrations est ensuite fixé via UseSqlServer/UseSqlite + MigrationsAssembly.
/// </summary>
public class PhpIpamDbContext : DbContext
{
    public PhpIpamDbContext(DbContextOptions<PhpIpamDbContext> options) : base(options) { }

    public DbSet<Section> Sections => Set<Section>();
    public DbSet<Subnet> Subnets => Set<Subnet>();
    public DbSet<IpAddress> IpAddresses => Set<IpAddress>();
    public DbSet<Vlan> Vlans => Set<Vlan>();
    public DbSet<VlanDomain> VlanDomains => Set<VlanDomain>();
    public DbSet<Vrf> Vrfs => Set<Vrf>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserGroup> UserGroups => Set<UserGroup>();
    public DbSet<UserAuthMethod> UserAuthMethods => Set<UserAuthMethod>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<DeviceType> DeviceTypes => Set<DeviceType>();
    public DbSet<Nameserver> Nameservers => Set<Nameserver>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<IpTag> IpTags => Set<IpTag>();
    public DbSet<ChangeLog> ChangeLogs => Set<ChangeLog>();
    public DbSet<Setting> Settings => Set<Setting>();
    public DbSet<Log> Logs => Set<Log>();
    public DbSet<Request> Requests => Set<Request>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        // Clés primaires non standard
        mb.Entity<Section>().HasKey(x => x.Id);
        mb.Entity<Subnet>().HasKey(x => x.Id);
        mb.Entity<IpAddress>().HasKey(x => x.Id);
        mb.Entity<Vlan>().HasKey(x => x.VlanId);
        mb.Entity<VlanDomain>().HasKey(x => x.Id);
        mb.Entity<Vrf>().HasKey(x => x.VrfId);
        mb.Entity<User>().HasKey(x => x.Id);
        mb.Entity<UserGroup>().HasKey(x => x.GId);
        mb.Entity<UserAuthMethod>().HasKey(x => x.Id);
        mb.Entity<Customer>().HasKey(x => x.Id);
        mb.Entity<Device>().HasKey(x => x.Id);
        mb.Entity<DeviceType>().HasKey(x => x.Tid);
        mb.Entity<Nameserver>().HasKey(x => x.Id);
        mb.Entity<Location>().HasKey(x => x.Id);
        mb.Entity<IpTag>().HasKey(x => x.Id);
        mb.Entity<ChangeLog>().HasKey(x => x.Cid);
        mb.Entity<Setting>().HasKey(x => x.Id);
        mb.Entity<Log>().HasKey(x => x.Id);
        mb.Entity<Request>().HasKey(x => x.Id);

        // Index uniques de l'original phpIPAM
        mb.Entity<Section>().HasIndex(x => x.Name).IsUnique();
        mb.Entity<User>().HasIndex(x => x.Username).IsUnique();
        mb.Entity<Customer>().HasIndex(x => x.Title).IsUnique();
        mb.Entity<IpAddress>().HasIndex(x => new { x.IpAddr, x.SubnetId }).IsUnique();

        // Relations principales
        mb.Entity<Subnet>()
            .HasOne(s => s.Section)
            .WithMany(s => s.Subnets)
            .HasForeignKey(s => s.SectionId)
            .OnDelete(DeleteBehavior.SetNull);

        mb.Entity<Subnet>()
            .HasOne(s => s.Vlan)
            .WithMany(v => v.Subnets)
            .HasForeignKey(s => s.VlanId)
            .OnDelete(DeleteBehavior.SetNull);

        mb.Entity<Subnet>()
            .HasOne(s => s.Vrf)
            .WithMany(v => v.Subnets)
            .HasForeignKey(s => s.VrfId)
            .HasPrincipalKey(v => v.VrfId)
            .OnDelete(DeleteBehavior.SetNull);

        mb.Entity<Subnet>()
            .HasOne(s => s.Customer)
            .WithMany()
            .HasForeignKey(s => s.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);

        mb.Entity<IpAddress>()
            .HasOne(i => i.Subnet)
            .WithMany(s => s.IpAddresses)
            .HasForeignKey(i => i.SubnetId)
            .OnDelete(DeleteBehavior.Cascade);

        mb.Entity<IpAddress>()
            .HasOne(i => i.Customer)
            .WithMany()
            .HasForeignKey(i => i.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);

        mb.Entity<Vlan>()
            .HasOne(v => v.Domain)
            .WithMany(d => d.Vlans)
            .HasForeignKey(v => v.DomainId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index supplémentaires fidèles au schéma
        mb.Entity<Subnet>().HasIndex(s => s.MasterSubnetId);
        mb.Entity<Subnet>().HasIndex(s => s.SectionId);
        mb.Entity<ChangeLog>().HasIndex(c => c.Coid);
        mb.Entity<ChangeLog>().HasIndex(c => c.Ctype);
        mb.Entity<Device>().HasIndex(d => d.Hostname);
    }
}
