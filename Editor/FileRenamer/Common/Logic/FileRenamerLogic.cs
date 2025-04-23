using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System;
using SFB;
using System.Text;

namespace FileRenamer
{
    public class FileRenamerLogic : IDisposable
    {
        #region Fields 

        public const int MaxFileLimit = 1000;

        private List<string> _inputFilePaths;
        private List<string> _overwrittenFiles;
        private Dictionary<string, string> _processedFiles;

        private string _exportFolderPath = "";
        private FileRenamerSettings _settings;

        private bool _processed = false;

        // ToDo : remove this and use events instead
        private string _resultMsg = "";
        private string _errorMsg = "";

        #endregion


        #region Properties 

        public IEnumerable<string> InputFilePaths => _inputFilePaths;
        public int FilesCountToProcess => _inputFilePaths.Count;
        public bool HasInputFiles => _inputFilePaths != null && FilesCountToProcess > 0;
        public bool HasMaxInputFiles => _inputFilePaths != null && FilesCountToProcess == MaxFileLimit;
        public FileRenamerSettings Settings => _settings;
        public Dictionary<string, string> ProcessedFiles => _processedFiles;

        public bool Processed => _processed;
        public string LastResultMsg => _resultMsg;
        public string LastErrorMsg => _errorMsg;

        #endregion


        #region Methods

        #region Init

        public FileRenamerLogic(FileRenamerSettings settings)
        {
            _settings = settings;
            _inputFilePaths = new List<string>();
            _overwrittenFiles = new List<string>();
            _processedFiles = new Dictionary<string, string>();

            Settings.OnNamingSettingsUpdated += UnsetProcessedStatus;
        }

        public void Dispose()
        {
            Settings.OnNamingSettingsUpdated -= UnsetProcessedStatus;
        }

        #endregion

        public void ReimportFiles()
        {
            ClearFiles();
            AddFiles();
        }

        public void AddFiles()
        {
            if (_inputFilePaths == null)
            {
                return;
            }

            int maxFilesToTake = MaxFileLimit - _inputFilePaths.Count;
            AddFilePathsToProcess(RequestFiles(maxFilesToTake));

            if (_inputFilePaths.Count == MaxFileLimit)
            {
                Debug.LogWarning($"Only the first {maxFilesToTake} files were selected.");
            }
        }

        public void ClearFiles()
        {
            _inputFilePaths?.Clear();
        }

        public string RequestFileName()
        {
            string[] filePath = StandaloneFileBrowser.OpenFilePanel("Select File to extract name", "", FileRenamerSettings.SupportedFileExtensions, false);
            return filePath.Length > 0 ? Path.GetFileNameWithoutExtension(filePath.First()) : null;
        }

        public void RemoveInputFilePath(string targetFilePath)
        {
            if (_inputFilePaths.Remove(targetFilePath))
            {
                UnsetProcessedStatus();
            }
        }

        public string RequestFolder()
        {
            return EditorUtility.OpenFolderPanel("Select Export Folder", "", "");
        }

        public void ProcessFiles()
        {
            if (Processed)
            {
                return;
            }

            _processedFiles.Clear();

            List<string> files = GetValidFiles();
            if (files == null || files.Count == 0)
            {
                _errorMsg = "# Not processed : Files do not exist!";
                return;
            }

            FileRenamerUtilities.SortFiles(files, Settings.SortAscending);

            ProcessFiles(files);

            _resultMsg = $"Processed {files.Count} files (ready for export)";
            _processed = true;
        }

        private List<string> GetValidFiles()
        {
            return _inputFilePaths?.Where(file => File.Exists(file)).ToList();
        }

        private void ProcessFiles(List<string> files)
        {
            int fileNumberingIndex = _settings.NumberingStartIndex;

            foreach (var originalFilePath in files)
            {
                string newFileName = GenerateNewFileName(originalFilePath, ref fileNumberingIndex);
                _processedFiles.Add(originalFilePath, newFileName);
            }
        }

        private string GenerateNewFileName(string originalFilePath, ref int fileNumberingIndex)
        {
            StringBuilder newFileNameBuilder;

            newFileNameBuilder = Settings.PreserveExistingName
                ? new StringBuilder(Path.GetFileNameWithoutExtension(originalFilePath))
                : new StringBuilder(Settings.FileNameTemplate);

            bool hasOwnNumbering = false;

            // Add prefix
            if (!string.IsNullOrEmpty(Settings.FileNamePrefix))
            {
                newFileNameBuilder.Insert(0, Settings.FileNamePrefix);
            }

            // Add suffix
            if (!string.IsNullOrEmpty(Settings.FileNameSuffix))
            {
                newFileNameBuilder.Append(Settings.FileNameSuffix);
            }

            // Extract existing numbering if the option is enabled
            if (Settings.PreserveExistingNumbering)
            {
                string existingNumbering = ExtractExistingNumbering(originalFilePath);
                if (!string.IsNullOrEmpty(existingNumbering))
                {
                    newFileNameBuilder.Append($"_{existingNumbering}");
                    hasOwnNumbering = true;
                }
            }

            // Add numbering if the option is enabled and no existing numbering is found
            if (Settings.AddNumbering && !hasOwnNumbering)
            {
                newFileNameBuilder.Append($"_{fileNumberingIndex:00}");
                fileNumberingIndex++;
            }

            // Add the file extension
            string extension = Path.GetExtension(originalFilePath);
            newFileNameBuilder.Append(extension);

            return newFileNameBuilder.ToString();
        }


        private string ExtractExistingNumbering(string originalFilePath)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFilePath);
            return FileRenamerUtilities.ExtractNumbering(fileNameWithoutExtension);
        }

        public string GetOverwrittenFilesInfo()
        {
            if (_overwrittenFiles.Count > 0)
            {
                return "The following files were overwritten:\n" + string.Join("\n", _overwrittenFiles);
            }
            else
            {
                return "No files were overwritten.";
            }
        }

        public void TryExportFiles()
        {
            if (_processedFiles.Count == 0)
            {
                _errorMsg = "# NOT EXPORTED : No files selected!";
                return;
            }

            // Open folder dialog to select export folder using StandaloneFileBrowser
            _exportFolderPath = RequestFolder();
            _exportFolderPath = TryCreateSubfolder(_settings.FileNameTemplate, _settings.CreateSubFolder);

            if (string.IsNullOrEmpty(_exportFolderPath))
            {
                _errorMsg = "# NOT EXPORTED : Incorrect export path!";
                return;
            }

            ExportFiles();
            TryOpenExportFolder();

            _resultMsg = $"Exported {_processedFiles.Count} files to {_exportFolderPath}";

            if (Settings.OverwriteFiles)
            {
                _resultMsg += $"\n{GetOverwrittenFilesInfo()}";
            }

            Debug.Log(_resultMsg);
        }

        private IEnumerable<string> RequestFiles(int count)
        {
            return StandaloneFileBrowser
                .OpenFilePanel("Select Files", "", FileRenamerSettings.SupportedFileExtensions, true)
                .Take(count);
        }

        private void AddFilePathToProcess(string filePath)
        {
            _inputFilePaths.Add(filePath);
            UnsetProcessedStatus();
        }

        private void AddFilePathsToProcess(IEnumerable<string> filePaths)
        {
            _inputFilePaths.AddRange(filePaths);
            UnsetProcessedStatus();
        }

        private void UnsetProcessedStatus()
        {
            _processed = false;
            _resultMsg = string.Empty;
        }

        private void TryOpenExportFolder()
        {
            if (_settings.OpenExportFolder)
            {
                FileRenamerUtilities.OpenFolder(Path.GetFullPath(_exportFolderPath));
            }
        }

        private void ExportFiles()
        {
            _overwrittenFiles.Clear();

            foreach (var entry in _processedFiles)
            {
                string originalPath = entry.Key;
                string newFileName = entry.Value;
                string newFilePath = Path.Combine(_exportFolderPath, newFileName);

                if (File.Exists(newFilePath) && Settings.OverwriteFiles)
                {
                    _overwrittenFiles.Add(newFilePath);
                }

                File.Copy(originalPath, newFilePath, overwrite: Settings.OverwriteFiles);
            }
        }

        private string TryCreateSubfolder(string fileNameTemplate, bool createSubfolder)
        {
            if (createSubfolder)
            {
                return FileRenamerUtilities.CreateFolder(_exportFolderPath, fileNameTemplate);
            }
            return _exportFolderPath;
        }

        #endregion
    }
}