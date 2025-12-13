using System.Diagnostics;
using Microsoft.Data.SqlClient;
using optimized_restore.Questions;
using optimized_restore.Settings;
using Spectre.Console;

namespace optimized_restore
{
    public class DatabaseService
    {
        public static void HandleRestore(SettingsObject fullConfiguration, Dictionary<QuestionKey, string> context)
        {
            var serverName = fullConfiguration.SqlServerHost;
            var sqlPackagePath = fullConfiguration.SqlPackagePath;

            var configurationToUse = fullConfiguration.RestoreConfigurations.First(x => x.Name == context[QuestionKey.ConfigurationToUse]);

            var executeQueriesAfterRestore = context[QuestionKey.ExecuteQueriesAfterRestore];
            var backupLocation = context[QuestionKey.BackupLocation].Replace("\"", string.Empty);
            var databaseNameWithPrefix = $"{configurationToUse.DatabaseNamePrefix}{context[QuestionKey.DatabaseName]}";

            // Step 1: Drop the database if it exists
            if (DatabaseExists(serverName, databaseNameWithPrefix))
            {
                AnsiConsole.MarkupLine("[yellow]Database exists. Dropping the database...[/]");
                DropDatabase(serverName, databaseNameWithPrefix);
            }
            else
            {
                AnsiConsole.MarkupLine("[yellow]Database does not exists...[/]");
            }

            // Step 2: Restore the .bacpac file
            AnsiConsole.MarkupLine($"[green]Restoring database '{databaseNameWithPrefix}' from '{backupLocation}'...[/]");

            if (RestoreBacpac(backupLocation, serverName, databaseNameWithPrefix, sqlPackagePath))
            {
                AnsiConsole.MarkupLine($"[green]Database '{databaseNameWithPrefix}' successfully restored.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Restore failed. Check the SqlPackage output for details.[/]");
            }

            if (executeQueriesAfterRestore == "Yes")
            {
                // Step 3: Run post-restore SQL queries
                foreach (string query in configurationToUse.QueriesAfterRestore)
                {
                    AnsiConsole.MarkupLine($"[blue]Running query[/]");
                    AnsiConsole.WriteLine(query);

                    if (ExecuteSqlQuery(serverName, databaseNameWithPrefix, query))
                    {
                        AnsiConsole.MarkupLine($"[green]Query executed successfully[/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"[red]Error executing query[/]");
                    }
                }

                AnsiConsole.MarkupLine("[green]Script completed successfully.[/]");
            }
        }

        public static bool DatabaseExists(string server, string database)
        {
            var connectionString = GetConnectionString(server);
            var query = $"SELECT COUNT(*) FROM sys.databases WHERE name = '{database}'";

            try
            {
                using var conn = new SqlConnection(connectionString);
                conn.Open();
                using var cmd = new SqlCommand(query, conn);
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error connecting to database[/]");
                AnsiConsole.WriteException(ex);
                return false;
            }
        }

        public static void DropDatabase(string server, string database)
        {
            var query = $"DROP DATABASE IF EXISTS [{database}]";

            ExecuteSqlQuery(server, database, query);
            AnsiConsole.MarkupLine($"[green]Database '{database}' successfully dropped.[/]");
        }

        public static bool ExecuteSqlQuery(string server, string database, string query)
        {
            var connectionString = GetConnectionString(server, database);

            try
            {
                using var conn = new SqlConnection(connectionString);
                conn.Open();
                using var cmd = new SqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error executing query: {ex.Message}[/]");
                return false;
            }
        }

        public static bool RestoreBacpac(string bacpacFile, string server, string database, string sqlPackagePath)
        {
            if (!System.IO.File.Exists(sqlPackagePath))
            {
                AnsiConsole.MarkupLine("[red]SqlPackage.exe not found. Please ensure SqlPackage is installed.[/]");
                return false;
            }

            var args = $"/Action:Import /SourceFile:\"{bacpacFile}\" /TargetServerName:\"{server}\" /TargetDatabaseName:\"{database}\" /TargetTrustServerCertificate:true";

            var startInfo = new ProcessStartInfo
            {
                FileName = sqlPackagePath,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                using var process = Process.Start(startInfo);
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                process.WaitForExit();
                AnsiConsole.Write(output);
                if (!string.IsNullOrEmpty(error))
                {
                    AnsiConsole.MarkupLine($"[red]Error: {error}[/]");
                    throw new Exception(error);
                }
                return true;
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error executing SqlPackage.exe: {ex.Message}[/]");
                throw;
            }
        }

        private static string GetConnectionString(string server, string database = "master")
        {
            return $"Server={server};Database={database};Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
        }
    }
}
