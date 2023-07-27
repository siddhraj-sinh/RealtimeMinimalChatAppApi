using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinimalChatAppApi.Migrations
{
    /// <inheritdoc />
    public partial class thirdCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Logss",
                table: "Logss");

            migrationBuilder.RenameTable(
                name: "Logss",
                newName: "Log");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Log",
                table: "Log",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Log",
                table: "Log");

            migrationBuilder.RenameTable(
                name: "Log",
                newName: "Logss");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Logss",
                table: "Logss",
                column: "Id");
        }
    }
}
