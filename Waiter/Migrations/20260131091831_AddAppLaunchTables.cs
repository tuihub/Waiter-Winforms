using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Waiter.Migrations
{
    /// <summary>
    /// Migration to add App Launch feature tables.
    /// </summary>
    public partial class AddAppLaunchTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // AppPackageLaunchSettings table
            migrationBuilder.CreateTable(
                name: "AppPackageLaunchSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AppPackageId = table.Column<long>(type: "INTEGER", nullable: false),
                    ExecutablePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    WorkingDirectory = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    MonitoringMode = table.Column<int>(type: "INTEGER", nullable: false),
                    ProcessName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    UseShellExecute = table.Column<bool>(type: "INTEGER", nullable: false),
                    LaunchTimeout = table.Column<int>(type: "INTEGER", nullable: false),
                    SaveDataPath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppPackageLaunchSettings", x => x.Id);
                });

            // RuntimeSessions table
            migrationBuilder.CreateTable(
                name: "RuntimeSessions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AppPackageId = table.Column<long>(type: "INTEGER", nullable: false),
                    DeviceId = table.Column<long>(type: "INTEGER", nullable: false),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExitCode = table.Column<int>(type: "INTEGER", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    UploadAttempted = table.Column<bool>(type: "INTEGER", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuntimeSessions", x => x.Id);
                });

            // CachedUploads table
            migrationBuilder.CreateTable(
                name: "CachedUploads",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RuntimeSessionId = table.Column<long>(type: "INTEGER", nullable: true),
                    UploadType = table.Column<int>(type: "INTEGER", nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Metadata = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RetryCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LastError = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CachedUploads", x => x.Id);
                });

            // Indexes for AppPackageLaunchSettings
            migrationBuilder.CreateIndex(
                name: "IX_AppPackageLaunchSettings_AppPackageId",
                table: "AppPackageLaunchSettings",
                column: "AppPackageId",
                unique: true);

            // Indexes for RuntimeSessions
            migrationBuilder.CreateIndex(
                name: "IX_RuntimeSessions_AppPackageId",
                table: "RuntimeSessions",
                column: "AppPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_RuntimeSessions_DeviceId",
                table: "RuntimeSessions",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_RuntimeSessions_Status",
                table: "RuntimeSessions",
                column: "Status");

            // Indexes for CachedUploads
            migrationBuilder.CreateIndex(
                name: "IX_CachedUploads_RuntimeSessionId",
                table: "CachedUploads",
                column: "RuntimeSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_CachedUploads_ExpiresAt",
                table: "CachedUploads",
                column: "ExpiresAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "CachedUploads");
            migrationBuilder.DropTable(name: "RuntimeSessions");
            migrationBuilder.DropTable(name: "AppPackageLaunchSettings");
        }
    }
}
