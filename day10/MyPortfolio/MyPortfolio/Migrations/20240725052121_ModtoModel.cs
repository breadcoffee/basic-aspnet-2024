using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPortfolio.Migrations
{
    /// <inheritdoc />
    public partial class ModtoModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Board_User_UserId1",
                table: "Board");

            migrationBuilder.DropIndex(
                name: "IX_Board_UserId1",
                table: "Board");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Board");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Board");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Board",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Board",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Board_UserId",
                table: "Board",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Board_User_UserId",
                table: "Board",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Board_User_UserId",
                table: "Board");

            migrationBuilder.DropIndex(
                name: "IX_Board_UserId",
                table: "Board");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Board");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Board",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Board",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "Board",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Board_UserId1",
                table: "Board",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Board_User_UserId1",
                table: "Board",
                column: "UserId1",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
