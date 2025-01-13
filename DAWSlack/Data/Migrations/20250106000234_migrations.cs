using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAWSlack.Data.Migrations
{
    /// <inheritdoc />
    public partial class migrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "ChatChannelId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChatChannelId1",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ChatChannelId",
                table: "AspNetUsers",
                column: "ChatChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ChatChannelId1",
                table: "AspNetUsers",
                column: "ChatChannelId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Channels_ChatChannelId",
                table: "AspNetUsers",
                column: "ChatChannelId",
                principalTable: "Channels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Channels_ChatChannelId1",
                table: "AspNetUsers",
                column: "ChatChannelId1",
                principalTable: "Channels",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Channels_ChatChannelId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Channels_ChatChannelId1",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ChatChannelId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ChatChannelId1",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ChatChannelId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ChatChannelId1",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
