using SFB;
using System;
using System.IO;


namespace FileRenamer
{
    public class FileRenamerSettings
    {
        #region Fields 

        public static readonly ExtensionFilter[] SupportedFileExtensions = new[]
{
            new ExtensionFilter("All Files", "*"),
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg", "bmp", "tiff"),
            new ExtensionFilter("3D Models", "fbx", "blender", "obj", "stl", "amf", "3ds", "iges"),
            new ExtensionFilter("Json/XML", "json", "xml"),
        };

        public event Action OnNamingSettingsUpdated;

        private string _fileNameTemplate = "ImageName_Template";    // Template for file names
        private string _fileNamePrefix;                             // Prefix for file names
        private string _fileNameSuffix;                             // Suffix for file names
        private int _numberingStartIndex = 0;
        private bool _sortAscending = true;                         // Sort files in ascending order
        private bool _addNumbering = true;                          // Add numbering to file names
        private bool _preserveExistingName = false;                 // Preserve existing names for files
        private bool _preserveExistingNumbering = false;            // Preserve existing numbering in file names

        #endregion


        #region Properties

        public bool OverwriteFiles { get; set; }                    // Can overwrite files
        public bool CreateSubFolder { get; set; }                   // Create a sub folder.
        public bool OpenExportFolder { get; set; }                  // Open the export folder.

        public string FileNameTemplate => _fileNameTemplate;
        public string FileNamePrefix => _fileNamePrefix;
        public string FileNameSuffix => _fileNameSuffix;

        public int NumberingStartIndex
        {
            get => _numberingStartIndex;
            set
            {
                if (_numberingStartIndex != value)
                {
                    _numberingStartIndex = value;
                    OnNamingSettingsUpdated?.Invoke();
                }
            }
        }

        public bool SortAscending
        {
            get => _sortAscending;
            set
            {
                if (_sortAscending != value)
                {
                    _sortAscending = value;
                    OnNamingSettingsUpdated?.Invoke();
                }
            }
        }

        public bool AddNumbering
        {
            get => _addNumbering;
            set
            {
                if (_addNumbering != value)
                {
                    _addNumbering = value;
                    OnNamingSettingsUpdated?.Invoke();
                }
            }
        }

        public bool PreserveExistingName
        {
            get => _preserveExistingName;
            set
            {
                if (_preserveExistingName != value)
                {
                    _preserveExistingName = value;
                    OnNamingSettingsUpdated?.Invoke();
                }
            }
        }

        public bool PreserveExistingNumbering
        {
            get => _preserveExistingNumbering;
            set
            {
                if (_preserveExistingNumbering != value)
                {
                    _preserveExistingNumbering = value;
                    OnNamingSettingsUpdated?.Invoke();
                }
            }
        }

        #endregion


        #region Methods

        public void SetFileNameTemplate(string targetTemplate)
        {
            SetFileNamingPart(ref _fileNameTemplate, targetTemplate);
        }

        public void SetFileNamePrefix(string targetPrefix)
        {
            SetFileNamingPart(ref _fileNamePrefix, targetPrefix);
        }

        public void SetFileNameSuffix(string targetSuffix)
        {
            SetFileNamingPart(ref _fileNameSuffix, targetSuffix);
        }

        private void SetFileNamingPart(ref string namingPart, string targetValue)
        {
            if (namingPart != targetValue)
            {
                if (ContainsInvalidFileNameChars(targetValue))
                {
                    throw new ArgumentException("File name part contains illegal characters.");
                }

                namingPart = targetValue;
                OnNamingSettingsUpdated?.Invoke();
            }
        }

        private bool ContainsInvalidFileNameChars(string fileName)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();

            foreach (char c in invalidChars)
            {
                if (fileName.Contains(c))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
