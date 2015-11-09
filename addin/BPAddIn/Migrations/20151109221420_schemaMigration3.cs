using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace BPAddIn.Migrations
{
    public partial class schemaMigration3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "timestamp",
                table: "PropertyChange",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "userGUID",
                table: "PropertyChange",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "timestamp",
                table: "ModelChange",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "userGUID",
                table: "ModelChange",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "timestamp", table: "PropertyChange");
            migrationBuilder.DropColumn(name: "userGUID", table: "PropertyChange");
            migrationBuilder.DropColumn(name: "timestamp", table: "ModelChange");
            migrationBuilder.DropColumn(name: "userGUID", table: "ModelChange");
        }
    }
}
