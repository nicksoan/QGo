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

namespace QGo
{
    public class CommandParser
    {
        // Regular expression to identify command types (UNC path, local folder, website, etc.)
        private static readonly Regex WebsiteRegex = new Regex(@"^(http|https)://", RegexOptions.IgnoreCase);
        private static readonly Regex UncPathRegex = new Regex(@"^\\\\", RegexOptions.IgnoreCase);
        private static readonly Regex LocalPathRegex = new Regex(@"^[a-zA-Z]:\\", RegexOptions.IgnoreCase);
        private readonly Dictionary<string, string> _shortcuts;
        public CommandParser()
        {
            _shortcuts = LoadShortcuts("shortcuts.json");
        }
        public QSearchResult ExecuteCommand(string command)
        {
            var searchResult = new QSearchResult();

            try
            {
                // Trim the command to handle any extra spaces.
                command = command.Trim();

                if (string.IsNullOrEmpty(command))
                {
                    searchResult.Success = false;
                    searchResult.Message = "Empty command. Please provide a valid command.";
                    return searchResult;
                }

                // Check if the command is a shortcut
                if (_shortcuts.ContainsKey(command))
                {
                    command = _shortcuts[command];
                }

                // Determine the type of command and execute accordingly
                if (WebsiteRegex.IsMatch(command))
                {
                    OpenWebsite(command);
                    searchResult.Success = true;
                    searchResult.Message = "Running Command";
                    return searchResult;

                }
                else if (UncPathRegex.IsMatch(command))
                {
                    OpenUncPath(command);
                    searchResult.Success = true;
                    searchResult.Message = "Running Command";
                    return searchResult;
                }
                else if (LocalPathRegex.IsMatch(command))
                {
                    OpenLocalFolder(command);
                    searchResult.Success = true;
                    searchResult.Message = "Running Command";
                    return searchResult;
                }
                else
                {
                    searchResult.Success = false;
                    searchResult.Message = "Unknown Command";
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

        public IEnumerable<string> GetMatchingShortcuts(string prefix)
        {
            foreach (var key in _shortcuts.Keys)
            {
                if (key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    yield return key;
                }
            }
        }
    }
}
