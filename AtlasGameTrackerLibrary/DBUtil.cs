using AtlasGameTrackerLibrary.models;
using Microsoft.Data.Sqlite;

namespace AtlasGameTrackerLibrary
{
    public static class DBUtil
    {
        private static string _connectionString => $"Data Source={getDBPath()}";

        private static string getDBPath()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string dbDirectory = System.IO.Path.Combine(appDataPath, "AtlasGameTracker");
            if (!System.IO.Directory.Exists(dbDirectory))
            {
                System.IO.Directory.CreateDirectory(dbDirectory);
            }
            return System.IO.Path.Combine(dbDirectory, "atlas_game_tracker.db");
        }

        public static void EnsureDatabaseExists()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var createRegisteredApps = @"
                CREATE TABLE IF NOT EXISTS RegisteredApps (
                    RegisteredAppId INTEGER PRIMARY KEY AUTOINCREMENT,
                    ProcessName TEXT NOT NULL UNIQUE,
                    DisplayName TEXT,
                    IsTracked INTEGER NOT NULL DEFAULT 1
                );
            ";

            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = createRegisteredApps;
                cmd.ExecuteNonQuery();
            }

            var createSnapshots = @"
                CREATE TABLE IF NOT EXISTS Snapshots (
                    SnapshotId INTEGER PRIMARY KEY AUTOINCREMENT,
                    RegisteredAppId INTEGER NOT NULL,
                    PollTime DATETIME NOT NULL,
                    StartTime DATETIME,
                    EndTime DATETIME
                );
            ";

            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = createSnapshots;
                cmd.ExecuteNonQuery();
            }
        }

        public static void RegisterApp(string processName, string? displayName = null)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT OR IGNORE INTO RegisteredApps (ProcessName, DisplayName, IsTracked)
                VALUES ($processName, $displayName, 1);
            ";
            command.Parameters.AddWithValue("$processName", processName);
            command.Parameters.AddWithValue("$displayName", (object?)displayName ?? DBNull.Value);
            command.ExecuteNonQuery();
        }

        public static void UnregisterApp(int registeredAppId)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();

            command.CommandText = @"
                UPDATE RegisteredApps
                SET IsTracked = 0
                WHERE RegisteredAppId = $registeredAppId;
            ";
            command.Parameters.AddWithValue("$registeredAppId", registeredAppId);
            command.ExecuteNonQuery();
        }

        public static List<RegisteredApp> GetTrackedRegisteredApps()
        {
            var results = new List<RegisteredApp>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT RegisteredAppId, ProcessName, DisplayName, IsTracked FROM RegisteredApps WHERE IsTracked = 1;";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var app = new RegisteredApp
                {
                    RegisteredAppId = reader.GetInt32(0),
                    ProcessName = reader.GetString(1),
                    DisplayName = reader.IsDBNull(2) ? null : reader.GetString(2),
                    IsTracked = reader.GetInt32(3) != 0
                };
                results.Add(app);
            }
            return results;
        }

        public static List<RegisteredApp> GetAllRegisteredApps()
        {
            var results = new List<RegisteredApp>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT RegisteredAppId, ProcessName, DisplayName, IsTracked FROM RegisteredApps;";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var app = new RegisteredApp
                {
                    RegisteredAppId = reader.GetInt32(0),
                    ProcessName = reader.GetString(1),
                    DisplayName = reader.IsDBNull(2) ? null : reader.GetString(2),
                    IsTracked = reader.GetInt32(3) != 0
                };
                results.Add(app);
            }
            return results;
        }

        public static void SaveSnapshot(Snapshot snapshot)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Snapshots (RegisteredAppId, PollTime, StartTime, EndTime)
                VALUES ($registeredAppId, $pollTime, $startTime, $endTime);";
            command.Parameters.AddWithValue("$registeredAppId", snapshot.RegisteredAppId);
            command.Parameters.AddWithValue("$pollTime", snapshot.PollTime);
            command.Parameters.AddWithValue("$startTime", (object?)snapshot.StartTime ?? DBNull.Value);
            command.Parameters.AddWithValue("$endTime", (object?)snapshot.EndTime ?? DBNull.Value);
            command.ExecuteNonQuery();
        }

        public static void UpdateSnapshot(Snapshot snapshot)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Snapshots
                SET RegisteredAppId = $registeredAppId,
                    PollTime = $pollTime,
                    StartTime = $startTime,
                    EndTime = $endTime
                WHERE SnapshotId = $snapshotId;";
            command.Parameters.AddWithValue("$registeredAppId", snapshot.RegisteredAppId);
            command.Parameters.AddWithValue("$pollTime", snapshot.PollTime);
            command.Parameters.AddWithValue("$startTime", (object?)snapshot.StartTime ?? DBNull.Value);
            command.Parameters.AddWithValue("$endTime", (object?)snapshot.EndTime ?? DBNull.Value);
            command.Parameters.AddWithValue("$snapshotId", snapshot.SnapshotId);
            command.ExecuteNonQuery();
        }

        public static void DeleteSnapshot(int snapShotId)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                DELETE FROM Snapshots
                WHERE SnapshotId = $snapshotId;";
            command.Parameters.AddWithValue("$snapshotId", snapShotId);
            command.ExecuteNonQuery();
        }

        public static Snapshot? GetLatestSnapshot(int registeredAppId)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT SnapshotId, RegisteredAppId, PollTime, StartTime, EndTime
                FROM Snapshots
                WHERE RegisteredAppId = $registeredAppId
                ORDER BY PollTime DESC
                LIMIT 1;";
            command.Parameters.AddWithValue("$registeredAppId", registeredAppId);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Snapshot
                {
                    SnapshotId = reader.GetInt32(0),
                    RegisteredAppId = reader.GetInt32(1),
                    PollTime = reader.GetDateTime(2),
                    StartTime = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                    EndTime = reader.IsDBNull(4) ? null : reader.GetDateTime(4)
                };
            }
            return null;
        }
    }
}
