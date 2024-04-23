using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskerBCAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class _002BC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TaskerItems",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "TaskerItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TaskerItems_UserId",
                table: "TaskerItems",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskerItems_AspNetUsers_UserId",
                table: "TaskerItems",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskerItems_AspNetUsers_UserId",
                table: "TaskerItems");

            migrationBuilder.DropIndex(
                name: "IX_TaskerItems_UserId",
                table: "TaskerItems");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TaskerItems");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TaskerItems",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
