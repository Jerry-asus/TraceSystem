using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TraceSystem.Migrations
{
    public partial class his : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ItemNamestb",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CrateTime = table.Column<DateTime>(nullable: false),
                    ItemName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemNamestb", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ItemValuestb",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CrateTime = table.Column<DateTime>(nullable: false),
                    DataType = table.Column<string>(nullable: true),
                    DataVaue = table.Column<string>(nullable: true),
                    ItemNamesID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemValuestb", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ItemValuestb_ItemNamestb_ItemNamesID",
                        column: x => x.ItemNamesID,
                        principalTable: "ItemNamestb",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemValuestb_ItemNamesID",
                table: "ItemValuestb",
                column: "ItemNamesID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemValuestb");

            migrationBuilder.DropTable(
                name: "ItemNamestb");
        }
    }
}
