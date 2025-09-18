using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hiscary.PlatformUsers.Persistence.Context.Migrations
{
    public partial class AddAbilityToHandleMultipleImageUrls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""platformusers"".""Libraries"" 
                ADD COLUMN ""AvatarImageUrls_Temp"" jsonb DEFAULT '{}'::jsonb;

                UPDATE ""platformusers"".""Libraries""
                SET ""AvatarImageUrls_Temp"" = jsonb_build_object('large', ""AvatarUrl"")
                WHERE ""AvatarUrl"" IS NOT NULL;

                UPDATE ""platformusers"".""Libraries""
                SET ""AvatarUrl"" = NULL;
            ");

            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                schema: "platformusers",
                table: "Libraries");

            migrationBuilder.AddColumn<string>(
                name: "AvatarImageUrls",
                schema: "platformusers",
                table: "Libraries",
                type: "jsonb",
                nullable: true,
                defaultValue: "{}");

            migrationBuilder.Sql(@"
                UPDATE ""platformusers"".""Libraries""
                SET ""AvatarImageUrls"" = ""AvatarImageUrls_Temp"";

                ALTER TABLE ""platformusers"".""Libraries"" 
                DROP COLUMN ""AvatarImageUrls_Temp"";
            ");
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
