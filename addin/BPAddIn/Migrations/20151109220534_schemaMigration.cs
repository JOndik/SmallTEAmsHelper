using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace BPAddIn.Migrations
{
    public partial class schemaMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModelChange",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelChange", x => x.id);
                });
            migrationBuilder.CreateTable(
                name: "PropertyChange",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    elementGUID = table.Column<string>(nullable: true),
                    propertyBody = table.Column<string>(nullable: true),
                    propertyType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyChange", x => x.id);
                });
            migrationBuilder.CreateTable(
                name: "dictionary",
                columns: table => new
                {
                    word = table.Column<string>(nullable: false),
                    flags = table.Column<string>(nullable: true),
                    @base = table.Column<string>(name: "base", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Word", x => x.word);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("ModelChange");
            migrationBuilder.DropTable("PropertyChange");
            migrationBuilder.DropTable("dictionary");
        }
    }
}
