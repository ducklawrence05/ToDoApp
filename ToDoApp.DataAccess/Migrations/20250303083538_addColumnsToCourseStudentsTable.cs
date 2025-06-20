using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoApp.Migrations
{
    /// <inheritdoc />
    public partial class addColumnsToCourseStudentsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AssignmentScore",
                table: "CourseStudents",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "FinalScore",
                table: "CourseStudents",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PracticalScore",
                table: "CourseStudents",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignmentScore",
                table: "CourseStudents");

            migrationBuilder.DropColumn(
                name: "FinalScore",
                table: "CourseStudents");

            migrationBuilder.DropColumn(
                name: "PracticalScore",
                table: "CourseStudents");
        }
    }
}
