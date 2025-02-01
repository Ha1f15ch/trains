using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DatabaseEngine.Migrations
{
    /// <inheritdoc />
    public partial class New_Tabbles_for_observerPattern : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dict");

            migrationBuilder.CreateTable(
                name: "NewsChannel",
                schema: "dict",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CountSubscribers = table.Column<int>(type: "integer", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsChannel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NewsChannelsPosts",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NewsChannelId = table.Column<int>(type: "integer", nullable: false),
                    TitlePost = table.Column<string>(type: "text", nullable: false),
                    BodyPost = table.Column<string>(type: "text", nullable: false),
                    FooterPost = table.Column<string>(type: "text", nullable: true),
                    AuthorPost = table.Column<string>(type: "text", nullable: false),
                    SurceImage = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsChannelsPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewsChannelsPosts_NewsChannel_NewsChannelId",
                        column: x => x.NewsChannelId,
                        principalSchema: "dict",
                        principalTable: "NewsChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NewsChannelsSubscribers",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NewsChannelId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsChannelsSubscribers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewsChannelsSubscribers_NewsChannel_NewsChannelId",
                        column: x => x.NewsChannelId,
                        principalSchema: "dict",
                        principalTable: "NewsChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NewsChannelsSubscribers_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NewsChannelsPosts_NewsChannelId",
                schema: "dbo",
                table: "NewsChannelsPosts",
                column: "NewsChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsChannelsSubscribers_NewsChannelId",
                schema: "dbo",
                table: "NewsChannelsSubscribers",
                column: "NewsChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsChannelsSubscribers_UserId",
                schema: "dbo",
                table: "NewsChannelsSubscribers",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NewsChannelsPosts",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "NewsChannelsSubscribers",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "NewsChannel",
                schema: "dict");
        }
    }
}
