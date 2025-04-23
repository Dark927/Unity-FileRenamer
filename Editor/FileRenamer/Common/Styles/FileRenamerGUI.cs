using FileRenamer.Styles;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FileRenamer
{
    public class FileRenamerGUI : EditorWindow
    {
        #region Fields 

        private FileRenamerLogic _fileRenamer;
        private string _errorMsg = "";
        private string _resultMsg = "";
        private string _customNumbering;

        private bool _showInputFilesList = false;           // Display input files names.
        private bool _previewResultFilesList = false;       // Display processed files names.

        private bool _showExtraNameTemplateSettings = false;
        private bool _showMainNameTemplateSettings = false;

        private Vector2 _globalScrollPos = Vector2.zero;
        private Vector2 _inputViewScrollPos = Vector2.zero;
        private Vector2 _resultPreviewScrollPos = Vector2.zero;

        #endregion

        #region Properties

        private bool CanPreviewResults => _fileRenamer.HasInputFiles;

        #endregion


        #region Methods

        #region Init

        [MenuItem("Tools/File Renamer")]
        public static void ShowWindow()
        {
            FileRenamerGUI window = GetWindow<FileRenamerGUI>("File Renamer", true);
            window.minSize = FileRenamerStyleGUI.WindowMinSize;

            Vector2 screenCenter = FileRenamerUtilities.GetWindowCenterScreenPos(window, Screen.currentResolution);
            window.position = new Rect(screenCenter.x, screenCenter.y, FileRenamerStyleGUI.WindowDefaultSize.x, FileRenamerStyleGUI.WindowDefaultSize.y);
        }

        private void Awake()
        {
            _fileRenamer = new FileRenamerLogic(new FileRenamerSettings());
        }

        private void OnDisable()
        {
            _fileRenamer?.Dispose();
        }

        #endregion

        private void OnGUI()
        {
            DisplayLayout();
        }

        private void DisplayLayout()
        {
            _globalScrollPos = EditorGUILayout.BeginScrollView(_globalScrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            DisplayHeader();

            DisplayFilesSelection();
            DisplaySelectedFilesStatus();
            ElementsGUI.DrawLine(Color.black);
            EditorGUILayout.Space();


            DisplayNameTemplateSettings();
            ElementsGUI.DrawLine(Color.black);

            EditorGUILayout.Space();
            DisplayProcessSettings();
            EditorGUILayout.Space();

            TryPreviewResult();

            ElementsGUI.DrawLine(Color.black);

            EditorGUILayout.Space();
            DisplayExportToggles();

            ElementsGUI.DrawLine(Color.black);
            EditorGUILayout.Space();

            ExportFiles();

            DisplayErrorMsg();
            DisplayResultMsg();

            EditorGUILayout.EndScrollView();
        }

        private static void DisplayHeader()
        {
            EditorGUILayout.Space();
            ElementsGUI.DrawLine(Color.black);
            GUILayout.Label(FileRenamerStyleGUI.HeaderText, EditorStyles.boldLabel);
            ElementsGUI.DrawLine(Color.black);
            EditorGUILayout.Space();
        }

        private void DisplayFilesSelection()
        {
            GUILayout.Label("Files Selection [ Input ]", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (GUILayout.Button("Select Files", FileRenamerStyleGUI.ButtonsLayouts))
            {
                _fileRenamer.ReimportFiles();
                ClearMessages();
            }
        }

        private void DisplaySelectedFilesStatus()
        {
            if (_fileRenamer == null)
            {
                Close();
                return;
            }

            if (!_fileRenamer.HasInputFiles)
            {
                GUILayout.Label($"No files selected.", FileRenamerStyleGUI.YellowLabel);
                return;
            }


            GUILayout.Label($"Selected Files: {_fileRenamer.FilesCountToProcess}", FileRenamerStyleGUI.GreenLabel);

            if (_fileRenamer.HasMaxInputFiles)
            {
                GUILayout.Label($"(max.)", FileRenamerStyleGUI.YellowLabel);
            }

            // Toggle to show/hide the file list
            ElementsGUI.DisplayCustomToggle("Show File List", ref _showInputFilesList, FileRenamerStyleGUI.ToggleLabelLayouts);
            TryDisplayInputFilesList();
        }

        private void TryDisplayInputFilesList()
        {
            if (!_showInputFilesList)
            {
                return;
            }

            EditorGUILayout.Space();
            ElementsGUI.DrawLine(Color.black);

            // Begin a scroll view for long lists
            _inputViewScrollPos = EditorGUILayout.BeginScrollView(_inputViewScrollPos, GUILayout.Height(150));

            for (int currentFileIndex = 0; currentFileIndex < _fileRenamer.InputFilePaths.Count(); currentFileIndex++)
            {
                string currentFile = _fileRenamer.InputFilePaths.ElementAt(currentFileIndex);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label($"{(currentFileIndex + 1)}.\t{Path.GetFileName(currentFile)}", GUILayout.ExpandWidth(true));


                // Button to remove the file
                if (GUILayout.Button("rm", GUILayout.Width(25)))
                {
                    _fileRenamer.RemoveInputFilePath(currentFile);
                    _resultMsg = _fileRenamer.LastResultMsg;
                    EditorGUILayout.EndHorizontal();

                    // Stop iteration to prevent state corruption
                    break;
                }

                EditorGUILayout.EndHorizontal();
            }

            // Ensure the scroll always ends properly
            EditorGUILayout.EndScrollView();

        }

        private void DisplayNameTemplateSettings()
        {
            GUILayout.Label("File Name Template [ Settings ]", EditorStyles.boldLabel);

            DisplayFileNameMainSettingsFoldout();
            DisplayFileNameAdjustmentsFoldout();
        }
        
        private void DisplayFileNameMainSettingsFoldout()
        {
            EditorGUILayout.Space();

            _showMainNameTemplateSettings = EditorGUILayout.Foldout(_showMainNameTemplateSettings, "Main Template", true);

            if (_showMainNameTemplateSettings)
            {
                ElementsGUI.DisplayCustomToggle(
                    "Preserve Existing File Name",
                    () => _fileRenamer.Settings.PreserveExistingName,
                    value => _fileRenamer.Settings.PreserveExistingName = value,
                    FileRenamerStyleGUI.ToggleLabelLayouts);

                GUI.enabled = !_fileRenamer.Settings.PreserveExistingName;

                ElementsGUI.DisplayTextField(
                    "File Name Template",
                    _fileRenamer.Settings.FileNameTemplate,
                    _fileRenamer.Settings.SetFileNameTemplate,
                    GUILayout.Width(FileRenamerStyleGUI.LongTextFieldMaxWidth));

                EditorGUILayout.Space();

                if (GUILayout.Button("Select template from file", FileRenamerStyleGUI.ButtonsLayouts))
                {
                    string requestedFileName = _fileRenamer.RequestFileName();

                    if (requestedFileName != null)
                    {
                        _fileRenamer.Settings.SetFileNameTemplate(requestedFileName);
                    }
                }

                GUI.enabled = true;
            }
        }

        private void DisplayFileNameAdjustmentsFoldout()
        {
            EditorGUILayout.Space();

            _showExtraNameTemplateSettings = EditorGUILayout.Foldout(_showExtraNameTemplateSettings, "Adjustments", true);

            if (_showExtraNameTemplateSettings)
            {
                ElementsGUI.DisplayTextFieldShort("Name Prefix", _fileRenamer.Settings.FileNamePrefix, _fileRenamer.Settings.SetFileNamePrefix);
                ElementsGUI.DisplayTextFieldShort("Name Suffix", _fileRenamer.Settings.FileNameSuffix, _fileRenamer.Settings.SetFileNameSuffix);
            }
        }

        private void TryPreviewResult()
        {
            if (!CanPreviewResults)
            {
                return;
            }

            ElementsGUI.DisplayCustomToggle("Preview Result", ref _previewResultFilesList, FileRenamerStyleGUI.ToggleLabelLayouts);

            if (!_previewResultFilesList)
            {
                return;
            }

            _fileRenamer.ProcessFiles();
            PreviewResult();
        }

        private void PreviewResult()
        {
            EditorGUILayout.Space();
            GUILayout.Label("Preview:", EditorStyles.boldLabel);
            ElementsGUI.DrawLine(Color.black);

            // Scroll view for displaying old -> new file names
            _resultPreviewScrollPos = EditorGUILayout.BeginScrollView(_resultPreviewScrollPos, GUILayout.Height(150), GUILayout.ExpandHeight(true));

            int counter = 1;

            foreach (var entry in _fileRenamer.ProcessedFiles)
            {
                string oldFileName = Path.GetFileName(entry.Key);
                string newFileName = entry.Value;

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label($"{counter}.   ", GUILayout.Width(30));
                GUILayout.Label(oldFileName, FileRenamerStyleGUI.YellowLabel, GUILayout.Width(100), GUILayout.ExpandWidth(true));
                GUILayout.Label(" → ", GUILayout.Width(30));
                GUILayout.Label(newFileName, FileRenamerStyleGUI.GreenLabel, GUILayout.Width(100), GUILayout.ExpandWidth(true));

                EditorGUILayout.EndHorizontal();
                counter++;
            }

            EditorGUILayout.EndScrollView();
        }

        private void DisplayExportToggles()
        {
            GUILayout.Label("Export Settings [ Settings ]", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            ElementsGUI.DisplayToggle(
                "Can Overwrite Files",
                () => _fileRenamer.Settings.OverwriteFiles,
                value => _fileRenamer.Settings.OverwriteFiles = value);

            ElementsGUI.DisplayToggle(
                "Create Subfolder",
                () => _fileRenamer.Settings.CreateSubFolder,
                value => _fileRenamer.Settings.CreateSubFolder = value);


            ElementsGUI.DisplayToggle(
                "Open Export Folder",
                () => _fileRenamer.Settings.OpenExportFolder,
                value => _fileRenamer.Settings.OpenExportFolder = value);
        }

        private void DisplayProcessSettings()
        {
            GUILayout.Label("Process Settings [ Settings ]", EditorStyles.boldLabel);

            ElementsGUI.DisplayToggle(
                "Sort Ascending",
                () => _fileRenamer.Settings.SortAscending,
                value => _fileRenamer.Settings.SortAscending = value);

            ElementsGUI.DisplayToggle(
                "Preserve Existing Numbering",
                () => _fileRenamer.Settings.PreserveExistingNumbering,
                value => _fileRenamer.Settings.PreserveExistingNumbering = value);


            ElementsGUI.DisplayToggle("Add Numbering",
                () => _fileRenamer.Settings.AddNumbering,
                value => _fileRenamer.Settings.AddNumbering = value);

            TryInputCustomNumbering();

            EditorGUILayout.Space();
        }


        private void TryInputCustomNumbering()
        {
            if (!_fileRenamer.Settings.AddNumbering)
            {
                return;
            }

            // ToDo : Implement this for checking numbers inside TextField!
            //_customNumbering = System.Text.RegularExpressions.Regex.Replace(_customNumbering, "[^0-9]", "");

            ElementsGUI.DisplayTextFieldShort("Custom Index", _customNumbering, (customNumbering) =>
            {
                if (int.TryParse(customNumbering, out int customIndex))
                {
                    _fileRenamer.Settings.NumberingStartIndex = customIndex;
                }
            });

            EditorGUILayout.Space();
        }

        private void ExportFiles()
        {
            if (GUILayout.Button("Process Files", FileRenamerStyleGUI.ButtonsLayouts))
            {
                _fileRenamer.ProcessFiles();
                _fileRenamer.TryExportFiles();
                UpdateMessages(_fileRenamer);
            }
        }
        private void UpdateMessages(FileRenamerLogic fileRenamer)
        {
            _resultMsg = fileRenamer.LastResultMsg;
            _errorMsg = fileRenamer.LastErrorMsg;
        }

        private void DisplayResultMsg()
        {
            if (!string.IsNullOrEmpty(_resultMsg))
            {
                _errorMsg = "";
                GUILayout.Label(_resultMsg, FileRenamerStyleGUI.GreenLabel);
            }
        }

        private void DisplayErrorMsg()
        {
            if (!string.IsNullOrEmpty(_errorMsg))
            {
                GUILayout.Label(_errorMsg, FileRenamerStyleGUI.RedLabel);
            }
        }

        private void ClearMessages()
        {
            _errorMsg = string.Empty;
            _resultMsg = string.Empty;
        }

        #endregion
    }
}