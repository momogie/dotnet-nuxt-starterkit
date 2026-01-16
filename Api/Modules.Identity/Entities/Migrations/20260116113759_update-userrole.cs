using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Identity.Entities.Migrations
{
    /// <inheritdoc />
    public partial class updateuserrole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoleId",
                schema: "Idp",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RoleId",
                schema: "Idp",
                table: "Users",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true);
        }
    }
}
