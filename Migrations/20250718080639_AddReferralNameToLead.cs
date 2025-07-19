using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyAuthDemo.Migrations
{
    /// <inheritdoc />
    public partial class AddReferralNameToLead : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReferralName",
                table: "Leads",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferralName",
                table: "Leads");
        }
    }
}
