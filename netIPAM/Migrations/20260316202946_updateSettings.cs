using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cIPAM.Migrations
{
    /// <inheritdoc />
    public partial class updateSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Settings",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "8b5abfdb-2cfe-405d-aca2-4d884fa29b9b", 0, "55ddd299-2b34-4665-aea6-d3a5b6f339f6", "admin@local.lan", true, false, null, "ADMIN@LOCAL.LAN", "ADMIN", "AQAAAAIAAYagAAAAEGM/ll7G62x2i5K84HUeA1LRTrMucZ6zPbAIMxnNEgg2KCVy9remI9mlliHU2tpHrw==", null, false, "9c78a916-aa9e-4862-b40b-8563a3ae874a", false, "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Settings_Name",
                table: "Settings",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Settings_Name",
                table: "Settings");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "8b5abfdb-2cfe-405d-aca2-4d884fa29b9b");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
