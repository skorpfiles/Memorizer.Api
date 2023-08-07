using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkorpFiles.Memorizer.Api.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class IdentityUserToApplicationUserDiscriminator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE [dbo].[AspNetUsers] SET [Discriminator] = 'ApplicationUser' WHERE [Discriminator] = 'IdentityUser'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE [dbo].[AspNetUsers] SET [Discriminator] = 'IdentityUser' WHERE [Discriminator] = 'ApplicationUser'");
        }
    }
}
