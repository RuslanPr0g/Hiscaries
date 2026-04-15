using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hiscary.PlatformUsers.Persistence.Context.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAnnotatedPdfs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserAnnotatedPdfs",
                schema: "platformusers",
                columns: table => new
                {
                    PlatformUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    PdfUrl = table.Column<string>(type: "text", nullable: false),
                    HasConflict = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAnnotatedPdfs", x => new { x.PlatformUserId, x.StoryId });
                    table.ForeignKey(
                        name: "FK_UserAnnotatedPdfs_PlatformUsers_PlatformUserId",
                        column: x => x.PlatformUserId,
                        principalSchema: "platformusers",
                        principalTable: "PlatformUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAnnotatedPdfs",
                schema: "platformusers");
        }
    }
}
