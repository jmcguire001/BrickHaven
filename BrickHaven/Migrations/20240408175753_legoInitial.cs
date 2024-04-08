using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrickHaven.Migrations
{
    /// <inheritdoc />
    public partial class legoInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Legos",
                columns: table => new
                {
                    LegoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LegoName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProgramName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LegoType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LegoImpact = table.Column<int>(type: "int", nullable: false),
                    LegoInstallation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LegoPhase = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Legos", x => x.LegoId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Legos");
        }
    }
}
