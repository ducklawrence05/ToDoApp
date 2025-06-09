using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoApp.Migrations
{
    /// <inheritdoc />
    public partial class addBalanceColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "Students",
                type: "decimal(18,2)", //18 là tổng số chữ số nguyên và chữ số thập phân, 2 chỉ là chữ số thập phân
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Students",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Students");
        }
    }
}
