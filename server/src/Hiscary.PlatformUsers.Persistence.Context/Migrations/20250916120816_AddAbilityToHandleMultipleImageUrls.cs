using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hiscary.PlatformUsers.Persistence.Context.Migrations
{
    public partial class AddAbilityToHandleMultipleImageUrls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarImageUrls",
                schema: "platformusers",
                table: "Libraries",
                type: "jsonb",
                nullable: true,
                defaultValue: "{}");

            migrationBuilder.Sql(@"
                UPDATE ""platformusers"".""Libraries""
                SET ""AvatarImageUrls"" = jsonb_build_object('large', ""AvatarUrl"")
                WHERE ""AvatarUrl"" IS NOT NULL;
            ");

            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                schema: "platformusers",
                table: "Libraries");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                schema: "platformusers",
                table: "Libraries",
                type: "text",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE ""platformusers"".""Libraries""
                SET ""AvatarUrl"" = ""AvatarImageUrls""->>'large'
                WHERE ""AvatarImageUrls"" IS NOT NULL;
            ");

            migrationBuilder.DropColumn(
                name: "AvatarImageUrls",
                schema: "platformusers",
                table: "Libraries");
        }
    }
}
