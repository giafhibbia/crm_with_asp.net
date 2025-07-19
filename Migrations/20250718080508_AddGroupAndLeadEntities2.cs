using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyAuthDemo.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupAndLeadEntities2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Leads_Groups_GroupId",
                table: "Leads");

            migrationBuilder.AddForeignKey(
                name: "FK_Leads_Groups_GroupId",
                table: "Leads",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Leads_Groups_GroupId",
                table: "Leads");

            migrationBuilder.AddForeignKey(
                name: "FK_Leads_Groups_GroupId",
                table: "Leads",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
