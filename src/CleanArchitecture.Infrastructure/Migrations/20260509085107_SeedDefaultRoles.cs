using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitecture.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedDefaultRoles : Migration
    {
        // Fixed GUIDs — ทำให้ migration นี้ idempotent และ reference ได้จาก migration อื่นๆ
        private static readonly Guid AdminRoleId     = new("00000000-0000-0000-0000-000000000001");
        private static readonly Guid UserRoleId      = new("00000000-0000-0000-0000-000000000002");
        private static readonly Guid ModeratorRoleId = new("00000000-0000-0000-0000-000000000003");

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "Id", "Name", "Description" },
                values: new object[,]
                {
                    { AdminRoleId,     "Admin",     "Full system access with all permissions." },
                    { UserRoleId,      "User",      "Standard user with basic access." },
                    { ModeratorRoleId, "Moderator", "Can moderate content and manage users." }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValues: new object[]
                {
                    AdminRoleId,
                    UserRoleId,
                    ModeratorRoleId
                });
        }
    }
}
