using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class AddMetadataChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OriginFileName",
                table: "CorpusesMetaDataXml",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OriginFileName",
                table: "ChunkListMetaData",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginFileName",
                table: "CorpusesMetaDataXml");

            migrationBuilder.DropColumn(
                name: "OriginFileName",
                table: "ChunkListMetaData");
        }
    }
}
