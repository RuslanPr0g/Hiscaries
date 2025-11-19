using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hiscary.Stories.Persistence.Context.Migrations
{
    /// <inheritdoc />
    public partial class ImageUrlNameRemoveLocalHost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
    migrationBuilder.Sql(@"
UPDATE stories.""Stories""
SET ""ImagePreviewUrl"" = jsonb_set(
    ""ImagePreviewUrl"",
    '{Urls,small}',
    to_jsonb(replace(""ImagePreviewUrl""->'Urls'->>'small','https://localhost:5001/',''))
)
WHERE ""ImagePreviewUrl""->'Urls'->>'small' IS NOT NULL;

UPDATE stories.""Stories""
SET ""ImagePreviewUrl"" = jsonb_set(
    ""ImagePreviewUrl"",
    '{Urls,medium}',
    to_jsonb(replace(""ImagePreviewUrl""->'Urls'->>'medium','https://localhost:5001/',''))
)
WHERE ""ImagePreviewUrl""->'Urls'->>'medium' IS NOT NULL;

UPDATE stories.""Stories""
SET ""ImagePreviewUrl"" = jsonb_set(
    ""ImagePreviewUrl"",
    '{Urls,large}',
    to_jsonb(replace(""ImagePreviewUrl""->'Urls'->>'large','https://localhost:5001/',''))
)
WHERE ""ImagePreviewUrl""->'Urls'->>'large' IS NOT NULL;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
