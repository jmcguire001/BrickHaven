using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrickHaven.Migrations.Lego
{
    /// <inheritdoc />
    public partial class lego3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Orders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Orders",
                type: "int",
                nullable: true);
        }
    }
}
