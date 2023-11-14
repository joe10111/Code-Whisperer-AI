using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeWhispererAI.Migrations
{
    /// <inheritdoc />
    public partial class ConnectUserToCodeAndResults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "application_user_id",
                table: "code_snippets",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "application_user_id",
                table: "code_analyses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_code_snippets_application_user_id",
                table: "code_snippets",
                column: "application_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_code_analyses_application_user_id",
                table: "code_analyses",
                column: "application_user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_code_analyses_users_application_user_id",
                table: "code_analyses",
                column: "application_user_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_code_snippets_users_application_user_id",
                table: "code_snippets",
                column: "application_user_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_code_analyses_users_application_user_id",
                table: "code_analyses");

            migrationBuilder.DropForeignKey(
                name: "fk_code_snippets_users_application_user_id",
                table: "code_snippets");

            migrationBuilder.DropIndex(
                name: "ix_code_snippets_application_user_id",
                table: "code_snippets");

            migrationBuilder.DropIndex(
                name: "ix_code_analyses_application_user_id",
                table: "code_analyses");

            migrationBuilder.DropColumn(
                name: "application_user_id",
                table: "code_snippets");

            migrationBuilder.DropColumn(
                name: "application_user_id",
                table: "code_analyses");
        }
    }
}
