using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrybeHotel.Migrations
{
    /// <inheritdoc />
    public partial class AddStateToCityTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Cities",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "Cities");
        }
    }
}
