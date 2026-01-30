using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChitalishteIskra.Data.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration123 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TypeName",
                table: "Lessons",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TypeName",
                table: "Lessons");
        }
    }
}
