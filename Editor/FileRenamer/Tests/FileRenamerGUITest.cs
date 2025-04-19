using NUnit.Framework;
using FileRenamer;
using System.Reflection;
using System.Collections;
using UnityEditor;
using UnityEngine.TestTools;
using System;

public class FileRenamerGUITests
{
    #region Fields

    private FileRenamerLogic _fileRenamer;
    private FileRenamerGUI _fileRenamerGUI;

    #endregion


    #region Setup: Initialize FileRenamerGUI

    [SetUp]
    public void SetUp()
    {
        // Get or create the editor window
        _fileRenamerGUI = EditorWindow.GetWindow<FileRenamerGUI>();
        _fileRenamerGUI.Show();

        var fileRenamerLogicField = typeof(FileRenamerGUI).GetField("_fileRenamer",
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(fileRenamerLogicField, "Could not find _fileRenamer field");

        _fileRenamer = (FileRenamerLogic)fileRenamerLogicField.GetValue(_fileRenamerGUI);
    }

    #endregion


    #region Test A10: Custom numeration index

    static string[] validNumberingCases = new string[] { "0", "0993", int.MaxValue.ToString() };
    static string[] invalidNumberingCases = new string[] { "-5", "-0", $"{(int.MaxValue).ToString()}00", "A1", "1A", "%", "-9999999999999999" };

    [UnityTest]
    public IEnumerator TestValidCustomNumerationIndex([ValueSource(nameof(validNumberingCases))] string validNumbering)
    {
        try
        {
            var numberingFieldInfo = typeof(FileRenamerGUI).GetField("_customNumbering",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(numberingFieldInfo, "Could not find _customNumbering field");

            numberingFieldInfo.SetValue(_fileRenamerGUI, validNumbering);

            yield return null;

            int targetNumbering = 0;
            Assert.DoesNotThrow(() => targetNumbering = int.Parse(validNumbering), "Could not parse valid numbering str to int!");

            Assert.AreEqual((string)validNumbering, (string)numberingFieldInfo.GetValue(_fileRenamerGUI),
                $"Actual numbering in {nameof(FileRenamerGUI)} != expected {nameof(validNumbering)}");

            Assert.AreEqual(targetNumbering, _fileRenamer.Settings.NumberingStartIndex,
                $"Expected target numbering {targetNumbering} was not set in the {nameof(FileRenamerLogic)}, " +
                $"actual value is {_fileRenamer.Settings.NumberingStartIndex}");
        }
        finally
        {
            _fileRenamerGUI.Close();
        }

        yield break;
    }


    [UnityTest]
    public IEnumerator TestInvalidCustomNumerationIndex([ValueSource(nameof(invalidNumberingCases))] string invalidNumbering)
    {
        try
        {
            var numberingFieldInfo = typeof(FileRenamerGUI).GetField("_customNumbering",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(numberingFieldInfo, "Could not find _customNumbering field");

            int previousNumbering = _fileRenamer.Settings.NumberingStartIndex;
            numberingFieldInfo.SetValue(_fileRenamerGUI, invalidNumbering);

            yield return null;

            Assert.AreEqual(previousNumbering, _fileRenamer.Settings.NumberingStartIndex,
                $"Expected target numbering {previousNumbering} was not set in the {nameof(FileRenamerLogic)}, " +
                $"actual value is {_fileRenamer.Settings.NumberingStartIndex}. Invalid input broke the previous numbering index");

            Assert.AreEqual((string)string.Empty, (string)numberingFieldInfo.GetValue(_fileRenamerGUI),
                $"Actual numbering in {nameof(FileRenamerGUI)} == {nameof(invalidNumbering)}, invalid numbering must be cleared!");
        }
        finally
        {
            _fileRenamerGUI.Close();
        }

        yield break;
    }

    #endregion


    #region Test A11: Preview results

    [UnityTest]
    public IEnumerator TestResultsPreview()
    {
        string testFileName = "testAsset.png";

        try
        {
            // Get initial state via reflection

            var previewControlConditionProp = typeof(FileRenamerGUI).GetProperty("CanPreviewResults",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(previewControlConditionProp, "Could not find CanPreviewResults property");

            bool previewState = (bool)previewControlConditionProp.GetValue(_fileRenamerGUI);
            Assert.IsFalse(previewState, "Default state should be false");


            // Check active Preview state
            ForceAddFileToFileRenamer(_fileRenamer, testFileName);

            yield return null;

            previewState = (bool)previewControlConditionProp.GetValue(_fileRenamerGUI);
            Assert.IsTrue(previewState, "State should be true when there are available files to process");

            _fileRenamer.ClearFiles();

            yield return null;

            previewState = (bool)previewControlConditionProp.GetValue(_fileRenamerGUI);
            Assert.IsFalse(previewState, "State should be false when there are no files to process");
        }
        finally
        {
            _fileRenamerGUI.Close();
        }
    }

    #endregion


    #region Test A14: Export with no processed resources

    [Test]
    public void TestFilesExport()
    {
        try
        {
            while (!_fileRenamer.HasInputFiles)
            {
                _fileRenamer.ReimportFiles();
            }

            _fileRenamer.ProcessFiles();
            _fileRenamer.TryExportFiles();
            Assert.IsTrue(_fileRenamer.LastResultMsg != string.Empty, "Result message is empty! User must be notified about export was finished");
        }
        finally
        {
            _fileRenamerGUI.Close();
        }
    }

    [Test]
    public void TestNoProcessedFilesExport()
    {
        try
        {
            ForceAddFileToFileRenamer(_fileRenamer, "file-1");
            _fileRenamer.ClearFiles();
            _fileRenamer.ProcessFiles();
            _fileRenamer.TryExportFiles();
            Assert.IsTrue(_fileRenamer.LastErrorMsg != string.Empty, "Error message is empty! User must be notified about no export was occured");
        }
        finally
        {
            _fileRenamerGUI.Close();
        }
    }

    #endregion


    #region Test 17: Export files overwrite

    [Test]
    public void TestOverwriteFiles()
    {
        try
        {
            _fileRenamer.Settings.OverwriteFiles = true;
            Assert.DoesNotThrow(() => TestFilesExport());
        }
        finally
        {
            _fileRenamerGUI.Close();
        }
    }

    [Test]
    public void TestNoOverwriteFiles()
    {
        try
        {
            _fileRenamer.Settings.OverwriteFiles = false;
            Assert.Catch<Exception>(() => TestFilesExport());
        }
        finally
        {
            _fileRenamerGUI.Close();
        }
    }

    #endregion


    #region Additional Methods

    private void ForceAddFileToFileRenamer(FileRenamerLogic fileRenamer, string filePath)
    {
        var methodInfo = typeof(FileRenamerLogic)
            .GetMethod("AddFilePathToProcess", BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsNotNull(methodInfo, "Could not find AddFilePathToProcess method");
        methodInfo.Invoke(fileRenamer, new object[] { filePath });
    }

    #endregion
}
