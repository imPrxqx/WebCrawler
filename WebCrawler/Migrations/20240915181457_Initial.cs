using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebCrawler.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WebsiteRecord",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Url = table.Column<string>(type: "text", nullable: false),
                    BoundaryRegExp = table.Column<string>(type: "text", nullable: false),
                    LastChange = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Days = table.Column<int>(type: "integer", nullable: false),
                    Hours = table.Column<int>(type: "integer", nullable: false),
                    Minutes = table.Column<int>(type: "integer", nullable: false),
                    Label = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Tags = table.Column<string>(type: "text", nullable: true),
                    LastExecution = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastStatus = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebsiteRecord", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Node",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    CrawlTime = table.Column<string>(type: "text", nullable: false),
                    UrlMain = table.Column<string>(type: "text", nullable: false),
                    WebsiteRecordId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Node", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Node_WebsiteRecord_WebsiteRecordId",
                        column: x => x.WebsiteRecordId,
                        principalTable: "WebsiteRecord",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NodeNeighbour",
                columns: table => new
                {
                    NodeId = table.Column<int>(type: "integer", nullable: false),
                    NeighbourNodeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NodeNeighbour", x => new { x.NodeId, x.NeighbourNodeId });
                    table.ForeignKey(
                        name: "FK_NodeNeighbour_Node_NeighbourNodeId",
                        column: x => x.NeighbourNodeId,
                        principalTable: "Node",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NodeNeighbour_Node_NodeId",
                        column: x => x.NodeId,
                        principalTable: "Node",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Node_WebsiteRecordId",
                table: "Node",
                column: "WebsiteRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_NodeNeighbour_NeighbourNodeId",
                table: "NodeNeighbour",
                column: "NeighbourNodeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NodeNeighbour");

            migrationBuilder.DropTable(
                name: "Node");

            migrationBuilder.DropTable(
                name: "WebsiteRecord");
        }
    }
}
