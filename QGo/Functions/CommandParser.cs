using QGo.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QGo.Functions
{
    public class CommandParser
    {
        // Regular expression to identify command types (UNC path, local folder, website, etc.)
        private static readonly Regex WebsiteRegex = new Regex(@"^(http|https)://", RegexOptions.IgnoreCase);
        private static readonly Regex UncPathRegex = new Regex(@"^\\\\", RegexOptions.IgnoreCase);
        private static readonly Regex LocalPathRegex = new Regex(@"^([a-zA-Z]:\\|%[A-Z0-9_]+%)", RegexOptions.IgnoreCase);
        private Dictionary<string, string> _shortcuts;
        private readonly string _jsonLocation = "";
        private static readonly Regex ExecutableFileRegex = new Regex(@"^([a-zA-Z]:\\|%[A-Z0-9_]+%).*\.exe$", RegexOptions.IgnoreCase);
        public CommandParser(string jsonLocation)
        {
            _jsonLocation = jsonLocation;
            _shortcuts = LoadShortcuts(_jsonLocation);
        }
        public QSearchResult ExecuteCommand(QQuery query)
        {
            var searchResult = new QSearchResult();

            try
            {
                // Trim the command to handle any extra spaces.
                query.Command = query.Command.Trim();

                if (string.IsNullOrEmpty(query.Command))
                {
                    searchResult.Success = false;
                    return searchResult;
                }

                // Check if the command is a shortcut
                if (_shortcuts.ContainsKey(query.Command))
                {
                    query.Command = _shortcuts[query.Command];

                    //Replace the {param} placeholder with the actual parameter in query.Param
                    if (!string.IsNullOrEmpty(query.Param))
                    {
                        query.Command = query.Command.Replace("{param}", query.Param);
                    }
                }

                // Determine the type of command and execute accordingly
                if (WebsiteRegex.IsMatch(query.Command))
                {
                    OpenWebsite(query.Command);
                    searchResult.Success = true;
                    searchResult.Message = "Running Command";
                    return searchResult;
                }
                else if (UncPathRegex.IsMatch(query.Command))
                {
                    OpenUncPath(query.Command);
                    searchResult.Success = true;
                    searchResult.Message = "Running Command";
                    return searchResult;
                }
                else if (ExecutableFileRegex.IsMatch(query.Command))
                {
                    RunExecutable(query.Command);
                    searchResult.Success = true;
                    searchResult.Message = "Running Command";
                    return searchResult;
                }
                else if (LocalPathRegex.IsMatch(query.Command))
                {
                    OpenLocalFolder(query.Command);
                    searchResult.Success = true;
                    searchResult.Message = "Running Command";
                    return searchResult;
                }
                else
                {
                    searchResult.Success = false;
                    searchResult.Message = "";
                    return searchResult;
                }
            }
            catch (Exception ex)
            {
                searchResult.Success = false;
                searchResult.Message = $"Error executing command: {ex.Message}";
                return searchResult;
            }
        }


        private void OpenWebsite(string url)
        {
            try
            {
                // Use the default browser to navigate to the website.
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to open website: {ex.Message}");
            }
        }

        private void OpenUncPath(string uncPath)
        {
            try
            {
                if (Directory.Exists(uncPath))
                {
                    // Open the network location in File Explorer.
                    Process.Start(new ProcessStartInfo("explorer", uncPath) { UseShellExecute = true });
                }
                else
                {
                    Console.WriteLine("The specified UNC path does not exist.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to open UNC path: {ex.Message}");
            }
        }

        private void OpenLocalFolder(string localPath)
        {
            try
            {
                if (localPath.StartsWith("%"))
                {
                    Process.Start(new ProcessStartInfo("explorer", localPath) { UseShellExecute = true });
                    return;
                }

                if (Directory.Exists(localPath))
                {
                    // Open the local folder in File Explorer.
                    Process.Start(new ProcessStartInfo("explorer", localPath) { UseShellExecute = true });
                }
                else
                {
                    Console.WriteLine("The specified local folder does not exist.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to open local folder: {ex.Message}");
            }
        }

        private void RunExecutable(string executablePath)
        {
            try
            {
                if (File.Exists(executablePath))
                {
                    // Run the executable file.
                    Process.Start(new ProcessStartInfo(executablePath) { UseShellExecute = true });
                }
                else
                {
                    Console.WriteLine("The specified executable file does not exist.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to run executable: {ex.Message}");
            }
        }

        private Dictionary<string, string> LoadShortcuts(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    return JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load shortcuts: {ex.Message}");
            }

            return new Dictionary<string, string>();
        }

        public void RefreshShortuts()
        {
            _shortcuts = LoadShortcuts(_jsonLocation);
        }


        public IEnumerable<string> GetMatchingShortcuts(string prefix)
        {
            foreach (string key in _shortcuts.Keys)
            {
                if (key.Contains(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    yield return key;
                }
            }
        }

    }
}
