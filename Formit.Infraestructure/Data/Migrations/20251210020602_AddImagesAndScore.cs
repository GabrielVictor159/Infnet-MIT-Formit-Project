using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Formit.Infraestructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddImagesAndScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Quizzes",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Questions",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "FormSubmissions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "FormSubmissions");
        }
    }
}
