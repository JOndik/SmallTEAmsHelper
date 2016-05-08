using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace BPAddIn.Migrations
{
    public partial class schemaMigration4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "PropertyChange",
                newName: "property_changes");
            migrationBuilder.RenameTable(
                name: "ModelChange",
                newName: "model_changes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "property_changes",
                newName: "PropertyChange");
            migrationBuilder.RenameTable(
                name: "model_changes",
                newName: "ModelChange");
        }
    }
}
