using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SWENG894.Data.Migrations
{
    public partial class Rollup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    ZipCode = table.Column<string>(nullable: true),
                    Rating = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ranking",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BronzeValue = table.Column<int>(nullable: false),
                    SilverValue = table.Column<int>(nullable: false),
                    GoldValue = table.Column<int>(nullable: false),
                    PlatinumValue = table.Column<int>(nullable: false),
                    DiamondValue = table.Column<int>(nullable: false),
                    Timestamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ranking", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FriendRequests",
                columns: table => new
                {
                    RequestedById = table.Column<string>(nullable: false),
                    RequestedForId = table.Column<string>(nullable: false),
                    RequestTime = table.Column<DateTime>(nullable: true),
                    BecameFriendsTime = table.Column<DateTime>(nullable: true),
                    RequestStatus = table.Column<int>(nullable: false),
                    RequesterStatus = table.Column<int>(nullable: false),
                    ReceiverStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendRequests", x => new { x.RequestedById, x.RequestedForId });
                    table.ForeignKey(
                        name: "FK_FriendRequests_AspNetUsers_RequestedById",
                        column: x => x.RequestedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FriendRequests_AspNetUsers_RequestedForId",
                        column: x => x.RequestedForId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SentById = table.Column<string>(nullable: true),
                    SentToId = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(maxLength: 50, nullable: false),
                    Body = table.Column<string>(maxLength: 500, nullable: false),
                    SentTime = table.Column<DateTime>(nullable: false),
                    SendStatus = table.Column<int>(nullable: false),
                    ReadStatus = table.Column<int>(nullable: false),
                    MessageType = table.Column<int>(nullable: false),
                    DeletedBySender = table.Column<bool>(nullable: false),
                    DeletedByReceiver = table.Column<bool>(nullable: false),
                    RelatedId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_AspNetUsers_SentById",
                        column: x => x.SentById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Messages_AspNetUsers_SentToId",
                        column: x => x.SentToId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Workouts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    Time = table.Column<int>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    ScoringType = table.Column<int>(nullable: false),
                    ScalingOptions = table.Column<string>(nullable: true),
                    Published = table.Column<bool>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workouts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Exercises",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Exer = table.Column<int>(nullable: false),
                    Reps = table.Column<int>(nullable: false),
                    WorkoutId = table.Column<int>(nullable: false),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exercises_Workouts_WorkoutId",
                        column: x => x.WorkoutId,
                        principalTable: "Workouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutFavorites",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    WorkoutId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutFavorites", x => new { x.UserId, x.WorkoutId });
                    table.ForeignKey(
                        name: "FK_WorkoutFavorites_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkoutFavorites_Workouts_WorkoutId",
                        column: x => x.WorkoutId,
                        principalTable: "Workouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutResults",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkoutId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    Score = table.Column<int>(nullable: false),
                    ResultNotes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutResults_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkoutResults_Workouts_WorkoutId",
                        column: x => x.WorkoutId,
                        principalTable: "Workouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Challenges",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChallengerId = table.Column<string>(nullable: false),
                    ContenderId = table.Column<string>(nullable: false),
                    WorkoutId = table.Column<int>(nullable: false),
                    ChallengerResultId = table.Column<int>(nullable: true),
                    ContenderResultId = table.Column<int>(nullable: true),
                    ChallengeProgress = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Challenges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Challenges_AspNetUsers_ChallengerId",
                        column: x => x.ChallengerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Challenges_WorkoutResults_ChallengerResultId",
                        column: x => x.ChallengerResultId,
                        principalTable: "WorkoutResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Challenges_AspNetUsers_ContenderId",
                        column: x => x.ContenderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Challenges_WorkoutResults_ContenderResultId",
                        column: x => x.ContenderResultId,
                        principalTable: "WorkoutResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Challenges_Workouts_WorkoutId",
                        column: x => x.WorkoutId,
                        principalTable: "Workouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NewsFeed",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    FeedType = table.Column<int>(nullable: false),
                    RelatedUserId = table.Column<string>(nullable: true),
                    RelatedWorkoutId = table.Column<int>(nullable: true),
                    RelatedChallengeId = table.Column<int>(nullable: true),
                    Dismissed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsFeed", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewsFeed_Challenges_RelatedChallengeId",
                        column: x => x.RelatedChallengeId,
                        principalTable: "Challenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NewsFeed_AspNetUsers_RelatedUserId",
                        column: x => x.RelatedUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NewsFeed_Workouts_RelatedWorkoutId",
                        column: x => x.RelatedWorkoutId,
                        principalTable: "Workouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NewsFeed_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_ChallengerId",
                table: "Challenges",
                column: "ChallengerId");

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_ChallengerResultId",
                table: "Challenges",
                column: "ChallengerResultId");

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_ContenderId",
                table: "Challenges",
                column: "ContenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_ContenderResultId",
                table: "Challenges",
                column: "ContenderResultId");

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_WorkoutId",
                table: "Challenges",
                column: "WorkoutId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_WorkoutId",
                table: "Exercises",
                column: "WorkoutId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_RequestedForId",
                table: "FriendRequests",
                column: "RequestedForId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SentById",
                table: "Messages",
                column: "SentById");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SentToId",
                table: "Messages",
                column: "SentToId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsFeed_RelatedChallengeId",
                table: "NewsFeed",
                column: "RelatedChallengeId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsFeed_RelatedUserId",
                table: "NewsFeed",
                column: "RelatedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsFeed_RelatedWorkoutId",
                table: "NewsFeed",
                column: "RelatedWorkoutId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsFeed_UserId",
                table: "NewsFeed",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutFavorites_WorkoutId",
                table: "WorkoutFavorites",
                column: "WorkoutId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutResults_UserId",
                table: "WorkoutResults",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutResults_WorkoutId",
                table: "WorkoutResults",
                column: "WorkoutId");

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_UserId",
                table: "Workouts",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Exercises");

            migrationBuilder.DropTable(
                name: "FriendRequests");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "NewsFeed");

            migrationBuilder.DropTable(
                name: "Ranking");

            migrationBuilder.DropTable(
                name: "WorkoutFavorites");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Challenges");

            migrationBuilder.DropTable(
                name: "WorkoutResults");

            migrationBuilder.DropTable(
                name: "Workouts");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
