using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hiscary.PlatformUsers.Persistence.Context.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "platformusers");

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                schema: "platformusers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    OccuredOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Error = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlatformUsers",
                schema: "platformusers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Libraries",
                schema: "platformusers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlatformUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Bio = table.Column<string>(type: "text", nullable: true),
                    AvatarUrl = table.Column<string>(type: "text", nullable: true),
                    LinksToSocialMedia = table.Column<List<string>>(type: "text[]", nullable: false),
                    SubscribersCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Libraries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Libraries_PlatformUsers_PlatformUserId",
                        column: x => x.PlatformUserId,
                        principalSchema: "platformusers",
                        principalTable: "PlatformUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlatformUserToLibrarySubscription",
                schema: "platformusers",
                columns: table => new
                {
                    PlatformUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LibraryId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformUserToLibrarySubscription", x => new { x.LibraryId, x.PlatformUserId });
                    table.ForeignKey(
                        name: "FK_PlatformUserToLibrarySubscription_PlatformUsers_PlatformUse~",
                        column: x => x.PlatformUserId,
                        principalSchema: "platformusers",
                        principalTable: "PlatformUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReadingHistories",
                schema: "platformusers",
                columns: table => new
                {
                    PlatformUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastPageRead = table.Column<int>(type: "integer", nullable: false),
                    SoftDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReadingHistories", x => new { x.StoryId, x.PlatformUserId });
                    table.ForeignKey(
                        name: "FK_ReadingHistories_PlatformUsers_PlatformUserId",
                        column: x => x.PlatformUserId,
                        principalSchema: "platformusers",
                        principalTable: "PlatformUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                schema: "platformusers",
                columns: table => new
                {
                    PlatformUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LibraryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => new { x.LibraryId, x.PlatformUserId });
                    table.ForeignKey(
                        name: "FK_Reviews_PlatformUsers_PlatformUserId",
                        column: x => x.PlatformUserId,
                        principalSchema: "platformusers",
                        principalTable: "PlatformUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StoryBookMarks",
                schema: "platformusers",
                columns: table => new
                {
                    PlatformUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryBookMarks", x => new { x.StoryId, x.PlatformUserId });
                    table.ForeignKey(
                        name: "FK_StoryBookMarks_PlatformUsers_PlatformUserId",
                        column: x => x.PlatformUserId,
                        principalSchema: "platformusers",
                        principalTable: "PlatformUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Libraries_PlatformUserId",
                schema: "platformusers",
                table: "Libraries",
                column: "PlatformUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccountId",
                schema: "platformusers",
                table: "PlatformUsers",
                column: "UserAccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlatformUserToLibrarySubscription_PlatformUserId",
                schema: "platformusers",
                table: "PlatformUserToLibrarySubscription",
                column: "PlatformUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReadingHistories_PlatformUserId",
                schema: "platformusers",
                table: "ReadingHistories",
                column: "PlatformUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_PlatformUserId",
                schema: "platformusers",
                table: "Reviews",
                column: "PlatformUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryBookMarks_PlatformUserId",
                schema: "platformusers",
                table: "StoryBookMarks",
                column: "PlatformUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Libraries",
                schema: "platformusers");

            migrationBuilder.DropTable(
                name: "OutboxMessages",
                schema: "platformusers");

            migrationBuilder.DropTable(
                name: "PlatformUserToLibrarySubscription",
                schema: "platformusers");

            migrationBuilder.DropTable(
                name: "ReadingHistories",
                schema: "platformusers");

            migrationBuilder.DropTable(
                name: "Reviews",
                schema: "platformusers");

            migrationBuilder.DropTable(
                name: "StoryBookMarks",
                schema: "platformusers");

            migrationBuilder.DropTable(
                name: "PlatformUsers",
                schema: "platformusers");
        }
    }
}
