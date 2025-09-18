using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hiscary.Notifications.Persistence.Context.Migrations
{
    public partial class AddAbilityToHandleMultipleImageUrls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""notifications"".""Notifications"" 
                ADD COLUMN ""ImageUrls_Temp"" jsonb DEFAULT '{}'::jsonb;

                UPDATE ""notifications"".""Notifications""
                SET ""ImageUrls_Temp"" = jsonb_build_object('large', ""PreviewUrl"")
                WHERE ""PreviewUrl"" IS NOT NULL;

                UPDATE ""notifications"".""Notifications""
                SET ""PreviewUrl"" = NULL;
            ");

            migrationBuilder.DropColumn(
                name: "PreviewUrl",
                schema: "notifications",
                table: "Notifications");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrls",
                schema: "notifications",
                table: "Notifications",
                type: "jsonb",
                nullable: true,
                defaultValue: "{}");

            migrationBuilder.Sql(@"
                UPDATE ""notifications"".""Notifications""
                SET ""ImageUrls"" = ""ImageUrls_Temp"";

                ALTER TABLE ""notifications"".""Notifications"" 
                DROP COLUMN ""ImageUrls_Temp"";
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PreviewUrl",
                schema: "notifications",
                table: "Notifications",
                type: "text",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE ""Notifications""
                SET ""PreviewUrl"" = ""ImageUrls""->>'large'
                WHERE ""ImageUrls"" IS NOT NULL;
            ");

            migrationBuilder.DropColumn(
                name: "ImageUrls",
                schema: "notifications",
                table: "Notifications");
        }
    }
}
