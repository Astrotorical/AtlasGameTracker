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

            var createSessions = @"
                CREATE TABLE IF NOT EXISTS Sessions (
                    SessionId INTEGER PRIMARY KEY AUTOINCREMENT,
                    RegisteredAppId INTEGER NOT NULL,
                    PollTime DATETIME NOT NULL,
                    StartTime DATETIME,
                    EndTime DATETIME
                );
            ";

            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = createSessions;
                cmd.ExecuteNonQuery();
            }
        }

        public static bool IsAppRegistered(string processName)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(1) FROM RegisteredApps WHERE ProcessName = $processName;";
            command.Parameters.AddWithValue("$processName", processName);
            long count = (long)command.ExecuteScalar()!;
            return count > 0;
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

        public static void ReregisterApp(int registeredAppId)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();

            command.CommandText = @"
                UPDATE RegisteredApps
                SET IsTracked = 1
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
                app.Sessions = GetSessionsForApp(app.RegisteredAppId);
                results.Add(app);
            }
            return results;
        }

        public static void SaveSession(Session Session)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Sessions (RegisteredAppId, PollTime, StartTime, EndTime)
                VALUES ($registeredAppId, $pollTime, $startTime, $endTime);";
            command.Parameters.AddWithValue("$registeredAppId", Session.RegisteredAppId);
            command.Parameters.AddWithValue("$pollTime", Session.PollTime);
            command.Parameters.AddWithValue("$startTime", (object?)Session.StartTime ?? DBNull.Value);
            command.Parameters.AddWithValue("$endTime", (object?)Session.EndTime ?? DBNull.Value);
            command.ExecuteNonQuery();
        }

        public static void UpdateSession(Session Session)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Sessions
                SET RegisteredAppId = $registeredAppId,
                    PollTime = $pollTime,
                    StartTime = $startTime,
                    EndTime = $endTime
                WHERE SessionId = $SessionId;";
            command.Parameters.AddWithValue("$registeredAppId", Session.RegisteredAppId);
            command.Parameters.AddWithValue("$pollTime", Session.PollTime);
            command.Parameters.AddWithValue("$startTime", (object?)Session.StartTime ?? DBNull.Value);
            command.Parameters.AddWithValue("$endTime", (object?)Session.EndTime ?? DBNull.Value);
            command.Parameters.AddWithValue("$SessionId", Session.SessionId);
            command.ExecuteNonQuery();
        }

        public static List<Session> GetSessionsForApp(int registeredAppId)
        {
            var results = new List<Session>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT SessionId, RegisteredAppId, PollTime, StartTime, EndTime
                FROM Sessions
                WHERE RegisteredAppId = $registeredAppId
                ORDER BY PollTime DESC;";
            command.Parameters.AddWithValue("$registeredAppId", registeredAppId);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var Session = new Session
                {
                    SessionId = reader.GetInt32(0),
                    RegisteredAppId = reader.GetInt32(1),
                    PollTime = reader.GetDateTime(2),
                    StartTime = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                    EndTime = reader.IsDBNull(4) ? null : reader.GetDateTime(4)
                };
                results.Add(Session);
            }
            return results;
        }

        public static void DeleteSession(int SessionId)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                DELETE FROM Sessions
                WHERE SessionId = $SessionId;";
            command.Parameters.AddWithValue("$SessionId", SessionId);
            command.ExecuteNonQuery();
        }

        public static Session? GetLatestSession(int registeredAppId)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT SessionId, RegisteredAppId, PollTime, StartTime, EndTime
                FROM Sessions
                WHERE RegisteredAppId = $registeredAppId
                ORDER BY PollTime DESC
                LIMIT 1;";
            command.Parameters.AddWithValue("$registeredAppId", registeredAppId);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Session
                {
                    SessionId = reader.GetInt32(0),
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
