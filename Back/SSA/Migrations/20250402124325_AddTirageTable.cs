using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSA.Migrations
{
    /// <inheritdoc />
    public partial class AddTirageTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tirages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PartieId = table.Column<int>(type: "INTEGER", nullable: false),
                    OffrantId = table.Column<int>(type: "INTEGER", nullable: false),
                    DestinataireId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tirages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tirages_Parties_PartieId",
                        column: x => x.PartieId,
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tirages_Users_DestinataireId",
                        column: x => x.DestinataireId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tirages_Users_OffrantId",
                        column: x => x.OffrantId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tirages_DestinataireId",
                table: "Tirages",
                column: "DestinataireId");

            migrationBuilder.CreateIndex(
                name: "IX_Tirages_OffrantId",
                table: "Tirages",
                column: "OffrantId");

            migrationBuilder.CreateIndex(
                name: "IX_Tirages_PartieId",
                table: "Tirages",
                column: "PartieId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tirages");
        }
    }
}
