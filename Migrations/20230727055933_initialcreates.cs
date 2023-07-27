using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinimalChatAppApi.Migrations
{
    /// <inheritdoc />
    public partial class initialcreates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Logs",
                table: "Logs");

            migrationBuilder.RenameTable(
                name: "Logs",
                newName: "Logss");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Logss",
                table: "Logss",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Logss",
                table: "Logss");

            migrationBuilder.RenameTable(
                name: "Logss",
                newName: "Logs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Logs",
                table: "Logs",
                column: "Id");
        }
    }
}
