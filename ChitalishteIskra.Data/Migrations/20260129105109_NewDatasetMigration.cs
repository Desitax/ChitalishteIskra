using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChitalishteIskra.Data.Migrations
{
    /// <inheritdoc />
    public partial class NewDatasetMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Types_TypeId",
                table: "Lessons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Types",
                table: "Types");

            migrationBuilder.RenameTable(
                name: "Types",
                newName: "LessonType");

            migrationBuilder.RenameIndex(
                name: "IX_Types_Name",
                table: "LessonType",
                newName: "IX_LessonType_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LessonType",
                table: "LessonType",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_LessonType_TypeId",
                table: "Lessons",
                column: "TypeId",
                principalTable: "LessonType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_LessonType_TypeId",
                table: "Lessons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LessonType",
                table: "LessonType");

            migrationBuilder.RenameTable(
                name: "LessonType",
                newName: "Types");

            migrationBuilder.RenameIndex(
                name: "IX_LessonType_Name",
                table: "Types",
                newName: "IX_Types_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Types",
                table: "Types",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Types_TypeId",
                table: "Lessons",
                column: "TypeId",
                principalTable: "Types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
