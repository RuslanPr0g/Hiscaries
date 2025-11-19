using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hiscary.PlatformUsers.Persistence.Context.Migrations
{
    /// <inheritdoc />
    public partial class ImageUrlNameRemoveLocalHost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
    migrationBuilder.Sql(@"
UPDATE platformusers.""Libraries""
SET ""AvatarImageUrls"" = jsonb_set(
    ""AvatarImageUrls"",
    '{Urls,small}',
    to_jsonb(replace(""AvatarImageUrls""->'Urls'->>'small','https://localhost:5001/',''))
)
WHERE ""AvatarImageUrls""->'Urls'->>'small' IS NOT NULL;

UPDATE platformusers.""Libraries""
SET ""AvatarImageUrls"" = jsonb_set(
    ""AvatarImageUrls"",
    '{Urls,medium}',
    to_jsonb(replace(""AvatarImageUrls""->'Urls'->>'medium','https://localhost:5001/',''))
)
WHERE ""AvatarImageUrls""->'Urls'->>'medium' IS NOT NULL;

UPDATE platformusers.""Libraries""
SET ""AvatarImageUrls"" = jsonb_set(
    ""AvatarImageUrls"",
    '{Urls,large}',
    to_jsonb(replace(""AvatarImageUrls""->'Urls'->>'large','https://localhost:5001/',''))
)
WHERE ""AvatarImageUrls""->'Urls'->>'large' IS NOT NULL;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
