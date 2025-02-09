using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace habyx.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileImageSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfilePictureUrl",
                table: "UserProfiles",
                newName: "ProfileImagePath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfileImagePath",
                table: "UserProfiles",
                newName: "ProfilePictureUrl");
        }
    }
}
