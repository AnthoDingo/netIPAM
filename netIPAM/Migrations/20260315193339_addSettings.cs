using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace cIPAM.Migrations
{
    /// <inheritdoc />
    public partial class addSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Id", "Name", "Type", "Value" },
                values: new object[,]
                {
                    { 1, "siteTitle", 0, "netIPAM address management" },
                    { 2, "siteDomain", 0, "domain.local" },
                    { 3, "siteURL", 0, "http://yourpublicurl.com" },
                    { 4, "siteLoginText", 0, "" },
                    { 5, "permissionPropagate", 2, "True" },
                    { 6, "vlanMax", 1, "4096" },
                    { 7, "maintaneanceMode", 2, "False" },
                    { 8, "siteAdminName", 0, "Sysadmin" },
                    { 9, "siteAdminMail", 0, "admin@domain.local" },
                    { 10, "api", 2, "False" },
                    { 11, "enableIPrequests", 2, "False" },
                    { 12, "enableVRF", 2, "False" },
                    { 13, "enableNAT", 2, "True" },
                    { 14, "enablePowerDNS", 2, "False" },
                    { 15, "enableDHCP", 2, "False" },
                    { 16, "enableFirewallZones", 2, "False" },
                    { 17, "enableDNSresolving", 2, "False" },
                    { 18, "tempShare", 2, "False" },
                    { 19, "enableChangelog", 2, "True" },
                    { 20, "enableMulticast", 2, "False" },
                    { 21, "enableThreshold", 2, "True" },
                    { 22, "enableRACK", 2, "True" },
                    { 23, "enableCircuits", 2, "True" },
                    { 24, "enableLocations", 2, "True" },
                    { 25, "enableSNMP", 2, "False" },
                    { 26, "enablePSNT", 2, "False" },
                    { 27, "enableCustomers", 2, "True" },
                    { 28, "enableRouting", 2, "False" },
                    { 29, "updateTags", 2, "False" },
                    { 30, "enforceUnique", 2, "True" },
                    { 31, "vlanDuplicate", 2, "True" },
                    { 32, "decodeMAC", 2, "True" },
                    { 33, "enableVaults", 2, "True" },
                    { 34, "passkeys", 2, "True" }
                });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings");
        }
    }
}
