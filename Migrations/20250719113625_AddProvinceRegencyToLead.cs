using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyAuthDemo.Migrations
{
    /// <inheritdoc />
    public partial class AddProvinceRegencyToLead : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProvinceId",
                table: "Leads",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RegencyId",
                table: "Leads",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "reg_provinces",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reg_provinces", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "reg_regencies",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    province_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reg_regencies", x => x.id);
                    table.ForeignKey(
                        name: "FK_reg_regencies_reg_provinces_province_id",
                        column: x => x.province_id,
                        principalTable: "reg_provinces",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_ProvinceId",
                table: "Leads",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_RegencyId",
                table: "Leads",
                column: "RegencyId");

            migrationBuilder.CreateIndex(
                name: "IX_reg_regencies_province_id",
                table: "reg_regencies",
                column: "province_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Leads_reg_provinces_ProvinceId",
                table: "Leads",
                column: "ProvinceId",
                principalTable: "reg_provinces",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Leads_reg_regencies_RegencyId",
                table: "Leads",
                column: "RegencyId",
                principalTable: "reg_regencies",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Leads_reg_provinces_ProvinceId",
                table: "Leads");

            migrationBuilder.DropForeignKey(
                name: "FK_Leads_reg_regencies_RegencyId",
                table: "Leads");

            migrationBuilder.DropTable(
                name: "reg_regencies");

            migrationBuilder.DropTable(
                name: "reg_provinces");

            migrationBuilder.DropIndex(
                name: "IX_Leads_ProvinceId",
                table: "Leads");

            migrationBuilder.DropIndex(
                name: "IX_Leads_RegencyId",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "ProvinceId",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "RegencyId",
                table: "Leads");
        }
    }
}
