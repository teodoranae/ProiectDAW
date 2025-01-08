using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAWSlack.Data.Migrations
{
    /// <inheritdoc />
    public partial class mgr2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Verifică existența coloanei înainte de redenumire
            migrationBuilder.Sql(@"
        IF EXISTS (
            SELECT 1 
            FROM INFORMATION_SCHEMA.COLUMNS 
            WHERE TABLE_NAME = 'UserChannels' AND COLUMN_NAME = 'Role'
        )
        BEGIN
            EXEC sp_rename 'UserChannels.Role', 'Roles', 'COLUMN';
        END
    ");

            // Creează tabela JoinRequests
            migrationBuilder.CreateTable(
                name: "JoinRequests",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChannelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinRequests", x => x.RequestId);
                });
        }

    }
}
