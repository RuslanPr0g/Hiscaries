using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hiscary.Stories.Persistence.Context.Migrations
{
    public partial class AddAbilityToHandleMultipleImageUrls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""stories"".""Stories""
                ADD COLUMN ""ImagePreviewUrl_Temp"" jsonb DEFAULT '{}'::jsonb;

                UPDATE ""stories"".""Stories""
                SET ""ImagePreviewUrl_Temp"" = jsonb_build_object('large', ""ImagePreviewUrl"")
                WHERE ""ImagePreviewUrl"" IS NOT NULL;
            ");

            migrationBuilder.DropColumn(
                name: "ImagePreviewUrl",
                schema: "stories",
                table: "Stories");

            migrationBuilder.AddColumn<string>(
                name: "ImagePreviewUrl",
                schema: "stories",
                table: "Stories",
                type: "jsonb",
                nullable: true,
                defaultValue: "{}");

            migrationBuilder.Sql(@"
                UPDATE ""stories"".""Stories""
                SET ""ImagePreviewUrl"" = ""ImagePreviewUrl_Temp"";

                ALTER TABLE ""stories"".""Stories""
                DROP COLUMN ""ImagePreviewUrl_Temp"";
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImagePreviewUrl",
                schema: "stories",
                table: "Stories",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.Sql(@"
                UPDATE ""stories"".""Stories""
                SET ""ImagePreviewUrl"" = ""ImagePreviewUrl""->>'large'
                WHERE ""ImagePreviewUrl"" IS NOT NULL;
            ");
        }
    }
}
