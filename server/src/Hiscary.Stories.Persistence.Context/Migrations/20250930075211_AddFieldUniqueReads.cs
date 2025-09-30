using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hiscary.Stories.Persistence.Context.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldUniqueReads : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UniqueReads",
                schema: "stories",
                table: "Stories",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UniqueReads",
                schema: "stories",
                table: "Stories");
        }
    }
}
