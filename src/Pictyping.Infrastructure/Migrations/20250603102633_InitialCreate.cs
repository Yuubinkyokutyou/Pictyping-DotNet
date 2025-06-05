using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pictyping.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    email = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    encrypted_password = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    reset_password_token = table.Column<string>(type: "text", nullable: true),
                    reset_password_sent_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    remember_created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    guest = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    playfabId = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false, defaultValue: "noname"),
                    rating = table.Column<int>(type: "integer", nullable: false),
                    online_game_ban_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    admin = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "omni_auth_identities",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    provider = table.Column<string>(type: "text", nullable: true),
                    uid = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_omni_auth_identities", x => x.id);
                    table.ForeignKey(
                        name: "FK_omni_auth_identities_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "oneside_two_player_typing_matches",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    battle_data_json = table.Column<string>(type: "jsonb", nullable: false, defaultValueSql: "'{}'::jsonb"),
                    match_id = table.Column<string>(type: "text", nullable: false),
                    register_id = table.Column<int>(type: "integer", nullable: false),
                    enemy_id = table.Column<int>(type: "integer", nullable: true),
                    enemy_started_rating = table.Column<int>(type: "integer", nullable: false),
                    started_rating = table.Column<int>(type: "integer", nullable: false),
                    is_finished = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    finished_rating = table.Column<int>(type: "integer", nullable: true),
                    battle_status = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_oneside_two_player_typing_matches", x => x.id);
                    table.ForeignKey(
                        name: "FK_oneside_two_player_typing_matches_users_enemy_id",
                        column: x => x.enemy_id,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_oneside_two_player_typing_matches_users_register_id",
                        column: x => x.register_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "penalty_risk_actions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    action_type = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_penalty_risk_actions", x => x.id);
                    table.ForeignKey(
                        name: "FK_penalty_risk_actions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "index_omni_auth_identities_on_provider_and_uid",
                table: "omni_auth_identities",
                columns: new[] { "provider", "uid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_omni_auth_identities_on_user_id",
                table: "omni_auth_identities",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "index_oneside_two_player_typing_matches_on_enemy_id",
                table: "oneside_two_player_typing_matches",
                column: "enemy_id");

            migrationBuilder.CreateIndex(
                name: "index_oneside_two_player_typing_matches_on_match_id",
                table: "oneside_two_player_typing_matches",
                column: "match_id");

            migrationBuilder.CreateIndex(
                name: "index_oneside_two_player_typing_matches_on_register_id",
                table: "oneside_two_player_typing_matches",
                column: "register_id");

            migrationBuilder.CreateIndex(
                name: "index_penalty_risk_actions_on_user_id",
                table: "penalty_risk_actions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "index_users_on_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_users_on_rating",
                table: "users",
                column: "rating");

            migrationBuilder.CreateIndex(
                name: "index_users_on_reset_password_token",
                table: "users",
                column: "reset_password_token",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "omni_auth_identities");

            migrationBuilder.DropTable(
                name: "oneside_two_player_typing_matches");

            migrationBuilder.DropTable(
                name: "penalty_risk_actions");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
