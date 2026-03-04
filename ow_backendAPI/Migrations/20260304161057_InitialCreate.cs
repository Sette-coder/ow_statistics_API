using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ow_backendAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "data");

            migrationBuilder.CreateTable(
                name: "app_users",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Client"),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    last_login = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_app_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "hero_list",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hero_list", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "map_list",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    mode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    mode_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_map_list", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    token = table.Column<string>(type: "text", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    revoked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_refresh_tokens_app_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "data",
                        principalTable: "app_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "game_records",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    map_id = table.Column<int>(type: "integer", nullable: false),
                    submit_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    season = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    rank = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    rank_division = table.Column<int>(type: "integer", nullable: false),
                    rank_percentage = table.Column<int>(type: "integer", nullable: false),
                    hero_1_id = table.Column<int>(type: "integer", nullable: false),
                    team_ban_1_id = table.Column<int>(type: "integer", nullable: false),
                    team_ban_2_id = table.Column<int>(type: "integer", nullable: false),
                    enemy_team_ban_1_id = table.Column<int>(type: "integer", nullable: false),
                    enemy_team_ban_2_id = table.Column<int>(type: "integer", nullable: false),
                    match_result = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    hero_2_id = table.Column<int>(type: "integer", nullable: true),
                    hero_3_id = table.Column<int>(type: "integer", nullable: true),
                    team_notes = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    enemy_team_notes = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_records", x => x.id);
                    table.ForeignKey(
                        name: "FK_game_records_app_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "data",
                        principalTable: "app_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_game_records_hero_list_enemy_team_ban_1_id",
                        column: x => x.enemy_team_ban_1_id,
                        principalSchema: "data",
                        principalTable: "hero_list",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_game_records_hero_list_enemy_team_ban_2_id",
                        column: x => x.enemy_team_ban_2_id,
                        principalSchema: "data",
                        principalTable: "hero_list",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_game_records_hero_list_hero_1_id",
                        column: x => x.hero_1_id,
                        principalSchema: "data",
                        principalTable: "hero_list",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_game_records_hero_list_hero_2_id",
                        column: x => x.hero_2_id,
                        principalSchema: "data",
                        principalTable: "hero_list",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_game_records_hero_list_hero_3_id",
                        column: x => x.hero_3_id,
                        principalSchema: "data",
                        principalTable: "hero_list",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_game_records_hero_list_team_ban_1_id",
                        column: x => x.team_ban_1_id,
                        principalSchema: "data",
                        principalTable: "hero_list",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_game_records_hero_list_team_ban_2_id",
                        column: x => x.team_ban_2_id,
                        principalSchema: "data",
                        principalTable: "hero_list",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_game_records_map_list_map_id",
                        column: x => x.map_id,
                        principalSchema: "data",
                        principalTable: "map_list",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_app_users_email",
                schema: "data",
                table: "app_users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_app_users_username",
                schema: "data",
                table: "app_users",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_game_records_enemy_team_ban_1_id",
                schema: "data",
                table: "game_records",
                column: "enemy_team_ban_1_id");

            migrationBuilder.CreateIndex(
                name: "IX_game_records_enemy_team_ban_2_id",
                schema: "data",
                table: "game_records",
                column: "enemy_team_ban_2_id");

            migrationBuilder.CreateIndex(
                name: "IX_game_records_hero_1_id",
                schema: "data",
                table: "game_records",
                column: "hero_1_id");

            migrationBuilder.CreateIndex(
                name: "IX_game_records_hero_2_id",
                schema: "data",
                table: "game_records",
                column: "hero_2_id");

            migrationBuilder.CreateIndex(
                name: "IX_game_records_hero_3_id",
                schema: "data",
                table: "game_records",
                column: "hero_3_id");

            migrationBuilder.CreateIndex(
                name: "IX_game_records_map_id",
                schema: "data",
                table: "game_records",
                column: "map_id");

            migrationBuilder.CreateIndex(
                name: "IX_game_records_team_ban_1_id",
                schema: "data",
                table: "game_records",
                column: "team_ban_1_id");

            migrationBuilder.CreateIndex(
                name: "IX_game_records_team_ban_2_id",
                schema: "data",
                table: "game_records",
                column: "team_ban_2_id");

            migrationBuilder.CreateIndex(
                name: "IX_game_records_user_id",
                schema: "data",
                table: "game_records",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_user_id",
                schema: "data",
                table: "refresh_tokens",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "game_records",
                schema: "data");

            migrationBuilder.DropTable(
                name: "refresh_tokens",
                schema: "data");

            migrationBuilder.DropTable(
                name: "hero_list",
                schema: "data");

            migrationBuilder.DropTable(
                name: "map_list",
                schema: "data");

            migrationBuilder.DropTable(
                name: "app_users",
                schema: "data");
        }
    }
}
