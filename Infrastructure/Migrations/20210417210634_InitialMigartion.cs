using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class InitialMigartion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Corpuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Corpuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Chunklists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CorpusId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chunklists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chunklists_Corpuses_CorpusId",
                        column: x => x.CorpusId,
                        principalTable: "Corpuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CorpusesMetaDataXml",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    NumberOfProcessedFiles = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorpusesMetaDataXml", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CorpusesMetaDataXml_Corpuses_Id",
                        column: x => x.Id,
                        principalTable: "Corpuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChunkListMetaData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    NumberOfChunks = table.Column<int>(type: "int", nullable: false),
                    NumberOfSentences = table.Column<int>(type: "int", nullable: false),
                    NumberOfTokens = table.Column<int>(type: "int", nullable: false),
                    JsonDictionaryLookUp = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChunkListMetaData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChunkListMetaData_Chunklists_Id",
                        column: x => x.Id,
                        principalTable: "Chunklists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Chunks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    XmlChunkId = table.Column<int>(type: "int", nullable: false),
                    ChunklistId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chunks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chunks_Chunklists_ChunklistId",
                        column: x => x.ChunklistId,
                        principalTable: "Chunklists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sentences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    XmlSentenceId = table.Column<int>(type: "int", nullable: false),
                    Xml = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChunkId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sentences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sentences_Chunks_ChunkId",
                        column: x => x.ChunkId,
                        principalTable: "Chunks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chunklists_CorpusId",
                table: "Chunklists",
                column: "CorpusId");

            migrationBuilder.CreateIndex(
                name: "IX_Chunks_ChunklistId",
                table: "Chunks",
                column: "ChunklistId");

            migrationBuilder.CreateIndex(
                name: "IX_Sentences_ChunkId",
                table: "Sentences",
                column: "ChunkId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChunkListMetaData");

            migrationBuilder.DropTable(
                name: "CorpusesMetaDataXml");

            migrationBuilder.DropTable(
                name: "Sentences");

            migrationBuilder.DropTable(
                name: "Chunks");

            migrationBuilder.DropTable(
                name: "Chunklists");

            migrationBuilder.DropTable(
                name: "Corpuses");
        }
    }
}
