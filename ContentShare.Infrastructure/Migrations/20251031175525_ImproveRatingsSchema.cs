using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContentShare.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ImproveRatingsSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Ratings_MediaContentId_UserId_CreatedAt",
                table: "Ratings");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Ratings",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_MediaContentId_UserId",
                table: "Ratings",
                columns: new[] { "MediaContentId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_UserId",
                table: "Ratings",
                column: "UserId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Ratings_Score_Range",
                table: "Ratings",
                sql: "\"Score\" >= 1 AND \"Score\" <= 5");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_AspNetUsers_UserId",
                table: "Ratings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_AspNetUsers_UserId",
                table: "Ratings");

            migrationBuilder.DropIndex(
                name: "IX_Ratings_MediaContentId_UserId",
                table: "Ratings");

            migrationBuilder.DropIndex(
                name: "IX_Ratings_UserId",
                table: "Ratings");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Ratings_Score_Range",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Ratings");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_MediaContentId_UserId_CreatedAt",
                table: "Ratings",
                columns: new[] { "MediaContentId", "UserId", "CreatedAt" });
        }
    }
}
