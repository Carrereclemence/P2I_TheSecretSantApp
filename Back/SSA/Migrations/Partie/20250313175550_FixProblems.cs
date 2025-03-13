using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSA.Migrations.Partie
{
    /// <inheritdoc />
    public partial class FixProblems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Parties_Users_AdminId",
                table: "Parties");

            migrationBuilder.RenameColumn(
                name: "AdminId",
                table: "Parties",
                newName: "ChefId");

            migrationBuilder.RenameIndex(
                name: "IX_Parties_AdminId",
                table: "Parties",
                newName: "IX_Parties_ChefId");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Parties",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Parties_Users_ChefId",
                table: "Parties",
                column: "ChefId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Parties_Users_ChefId",
                table: "Parties");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Parties");

            migrationBuilder.RenameColumn(
                name: "ChefId",
                table: "Parties",
                newName: "AdminId");

            migrationBuilder.RenameIndex(
                name: "IX_Parties_ChefId",
                table: "Parties",
                newName: "IX_Parties_AdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_Parties_Users_AdminId",
                table: "Parties",
                column: "AdminId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
