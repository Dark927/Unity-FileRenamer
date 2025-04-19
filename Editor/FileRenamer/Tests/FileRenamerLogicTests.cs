using NUnit.Framework;
using FileRenamer;
using System.Linq;
using System.Reflection;

/// <summary>
/// Contains unit tests for verifying condition coverage in the file renaming functionality.
/// These tests ensure that all logical conditions and edge cases in the file renaming process
/// are properly exercised and validated.
/// </summary>
/// <remarks>
/// The tests in this class focus on:
/// <list type="bullet">
///   <item>Validating correct behavior with various file path inputs</item>
///   <item>Testing edge cases and invalid inputs</item>
///   <item>Verifying internal state changes through reflection when needed</item>
///   <item>Ensuring proper error handling for exceptional cases</item>
/// </list>
/// </remarks>
public class FileRenamerLogicTests
{
    #region Fields

    private FileRenamerLogic _fileRenamer;

    #endregion


    #region Setup: Initialize FileRenamerLogic and import files

    [SetUp]
    public void SetUp()
    {
        _fileRenamer = new FileRenamerLogic(new FileRenamerSettings());
    }

    #endregion


    #region Test A4: Files import

    [Test]
    [Order(0)]
    public void TestFilesImport()
    {
        _fileRenamer.ReimportFiles();
        Assert.IsTrue(_fileRenamer.HasInputFiles);
    }

    [Test]
    [Order(0)]
    [TestCase("ValidAssetName")]
    [TestCase("Valid_Asset_Name")]
    [TestCase("Коректна_Назва-#Ассета_0215")]
    public void TestValidFileName(string validFilePath)
    {
        ForceAddFileToFileRenamer(_fileRenamer, validFilePath);

        // Verify the file was added to internal list
        Assert.IsTrue(_fileRenamer.HasInputFiles);
        Assert.IsTrue(_fileRenamer.InputFilePaths.Contains(validFilePath));
    }

    [Test]
    [Order(0)]
    [TestCase("Invalid<\"?\">AssetPath")]
    [TestCase("Another|Invalid*Path")]
    [TestCase("Більше/Invalid\\CharsInPath")]
    public void TestInvalidFileName(string invalidFilePath)
    {
        int filesCountToProcessBefore = _fileRenamer.FilesCountToProcess;
        ForceAddFileToFileRenamer(_fileRenamer, invalidFilePath);

        // Verify the file was not added to internal list
        Assert.IsTrue(_fileRenamer.FilesCountToProcess == filesCountToProcessBefore);
        Assert.IsTrue(!_fileRenamer.InputFilePaths.Contains(invalidFilePath));
    }

    #endregion


    #region Test A6: Files uniqueness

    [Test]
    [Order(1)]
    public void TestUniqueFiles()
    {
        ForceAddFileToFileRenamer(_fileRenamer, "Asset-1");
        ForceAddFileToFileRenamer(_fileRenamer, "Asset-1");

        var inputFilePaths = _fileRenamer.InputFilePaths;
        var distinctCount = inputFilePaths.Distinct().Count();
        Assert.AreEqual(inputFilePaths.Count(), distinctCount, "Input file list contains duplicate paths");
    }

    #endregion


    #region Test A8: Name Template

    [Test]
    [Order(2)]
    [TestCase("ValidTemplate")]
    [TestCase("ІгровийАссет_Valid_Template")]
    [TestCase("Asset_Valid-Template#_0215")]
    public void TestValidNameTemplate(string validTemplate)
    {
        Assert.DoesNotThrow(() => _fileRenamer.Settings.SetFileNameTemplate(validTemplate));
    }

    [Test]
    [Order(2)]
    [TestCase("Invalid<\"?\">Template")]
    [TestCase("Another|Invalid*Template")]
    [TestCase("More/Invalid\\Chars")]
    public void TestInvalidNameTemplate(string invalidTemplate)
    {
        _fileRenamer.ProcessFiles();
        Assert.Catch(() => _fileRenamer.Settings.SetFileNameTemplate(invalidTemplate));
        Assert.IsTrue(!string.IsNullOrEmpty(_fileRenamer.LastErrorMsg));
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
