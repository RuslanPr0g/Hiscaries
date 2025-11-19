using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hiscary.Notifications.Persistence.Context.Migrations
{
    /// <inheritdoc />
    public partial class ImageUrlNameRemoveLocalHost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
    migrationBuilder.Sql(@"
UPDATE notifications.""Notifications""
SET ""ImageUrls"" = jsonb_set(
    ""ImageUrls"",
    '{Urls,small}',
    to_jsonb(replace(""ImageUrls""->'Urls'->>'small','https://localhost:5001/',''))
)
WHERE ""ImageUrls""->'Urls'->>'small' IS NOT NULL;

UPDATE notifications.""Notifications""
SET ""ImageUrls"" = jsonb_set(
    ""ImageUrls"",
    '{Urls,medium}',
    to_jsonb(replace(""ImageUrls""->'Urls'->>'medium','https://localhost:5001/',''))
)
WHERE ""ImageUrls""->'Urls'->>'medium' IS NOT NULL;

UPDATE notifications.""Notifications""
SET ""ImageUrls"" = jsonb_set(
    ""ImageUrls"",
    '{Urls,large}',
    to_jsonb(replace(""ImageUrls""->'Urls'->>'large','https://localhost:5001/',''))
)
WHERE ""ImageUrls""->'Urls'->>'large' IS NOT NULL;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
