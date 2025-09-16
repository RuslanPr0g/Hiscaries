using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hiscary.Stories.Persistence.Context.Migrations
{
    public partial class ChangeImagePreviewUrlToJsonb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE ""stories"".""Stories""
                SET ""ImagePreviewUrl"" = jsonb_build_object('large', ""ImagePreviewUrl"")
                WHERE ""ImagePreviewUrl"" IS NOT NULL;
            ");

            migrationBuilder.AlterColumn<string>(
                name: "ImagePreviewUrl",
                schema: "stories",
                table: "Stories",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
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
