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
        private int _numberingStartIndex = 0;
        private bool _sortAscending = true;                         // Sort files in ascending order
        private bool _addNumbering = true;                          // Add numbering to file names
        private bool _preserveExistingNumbering = false;            // Preserve existing numbering in file names

        #endregion


        #region Properties

        public bool OverwriteFiles { get; set; }                    // Can overwrite files
        public bool CreateSubFolder { get; set; }                   // Create a sub folder.
        public bool OpenExportFolder { get; set; }                  // Open the export folder.

        public string FileNameTemplate => _fileNameTemplate;

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
            if (_fileNameTemplate != targetTemplate)
            {
                //// Validate the file name template for illegal characters
                //if (ContainsInvalidFileNameChars(value))
                //{
                //    throw new ArgumentException("File name template contains illegal characters.");
                //}

                _fileNameTemplate = targetTemplate;
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
