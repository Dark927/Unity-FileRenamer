using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace FileRenamer
{
    public class FileRenamerUtilities
    {
        public static Vector2 GetWindowCenterScreenPos(EditorWindow window, Resolution screenResolution)
        {
            float screenWidth = screenResolution.width;
            float screenHeight = screenResolution.height;

            float xPosition = (screenWidth - window.position.width) / 2f;
            float yPosition = (screenHeight - window.position.height) / 2f;

            return new Vector2(xPosition, yPosition);
        }

        /// <summary>
        /// Helper method to sort files in requested order (ASC or DSC).
        /// </summary>
        public static void SortFiles(List<string> files, bool sortAscending)
        {
            if (sortAscending)
            {
                files.Sort();
            }
            else
            {
                files.Sort((a, b) => string.Compare(b, a));
            }
        }

        /// <summary>
        /// Helper method to create the folder.
        /// </summary>
        public static string CreateFolder(string path, string folderName)
        {
            string targetFolderPath = Path.Combine(path, folderName);
            Directory.CreateDirectory(targetFolderPath);

            return targetFolderPath;
        }

        /// <summary>
        /// Helper method to extract existing numbering from a file name
        /// </summary>
        public static string ExtractNumbering(string fileName)
        {
            var match = Regex.Match(fileName, @"(\d+)$");
            if (match.Success)
            {
                // Return the numbering part
                return match.Groups[1].Value;
            }
            // No numbering found
            return null;
        }


        /// <summary>
        /// Helper method to open the target directory in the explorer
        /// </summary>
        public static void OpenFolder(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                Process.Start("explorer.exe", path);
            }
            else
            {
                UnityEngine.Debug.LogError("# Can not open the folder! Path is empty!");
            }
        }
    }
}
