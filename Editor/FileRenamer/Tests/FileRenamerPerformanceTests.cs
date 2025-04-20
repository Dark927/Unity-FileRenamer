
using FileRenamer;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEngine;

public class FileRenamerPerformanceTests
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


    #region Volume Testing

    private string _testDirectory = Path.Combine(Application.dataPath, "TestFiles");


    [Test]
    [TestCase(1, "png")]
    [TestCase(1, "fbx")]
    [TestCase(1, "WRONG123")]

    [TestCase(5, "png")]
    [TestCase(5, "fbx")]
    [TestCase(5, "WRONG123")]

    [TestCase(50, "png")]
    [TestCase(50, "fbx")]
    [TestCase(50, "WRONG123")]

    [TestCase(500, "png")]
    [TestCase(500, "fbx")]
    [TestCase(500, "WRONG123")]

    [TestCase(1000, "png")]
    [TestCase(1000, "fbx")]
    [TestCase(1000, "WRONG123")]

    [TestCase(5000, "png")]
    [TestCase(5000, "fbx")]
    [TestCase(5000, "WRONG123")]

    [TestCase(50000, "png")]
    [TestCase(50000, "fbx")]
    [TestCase(50000, "WRONG123")]
    public void VolumeTesting(int numberOfFilesToProcess, string filesFormat)
    {
        PerformVolumeTest(numberOfFilesToProcess, filesFormat);
    }

    [Test]
    [TestCase(5000, 0)]
    [TestCase(5000, 50000)]
    [TestCase(5000, 5000000000)]
    [TestCase(50000, 0)]
    [TestCase(50000, 50000)]
    [TestCase(50000, 5000000000)]
    public void VolumeTestingWithNumbering(int numberOfFilesToProcess, int startIndex)
    {
        _fileRenamer.Settings.AddNumbering = true;
        _fileRenamer.Settings.PreserveExistingNumbering = false;
        _fileRenamer.Settings.NumberingStartIndex = startIndex;
        PerformVolumeTest(numberOfFilesToProcess, "png");
    }

    private void PerformVolumeTest(int numberOfFilesToProcess, string filesFormat)
    {
        List<string> testFiles = GenerateLargeTestFiles(numberOfFilesToProcess, filesFormat);

        var addFilesMethod = typeof(FileRenamerLogic).GetMethod("AddFilePathsToProcess",
            BindingFlags.NonPublic | BindingFlags.Instance);

        addFilesMethod?.Invoke(_fileRenamer, new object[] { testFiles });

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        _fileRenamer.ProcessFiles();

        stopwatch.Stop();

        UnityEngine.Debug.Log($"Time taken for processing {numberOfFilesToProcess} files: {stopwatch.ElapsedMilliseconds} ms");
        Assert.LessOrEqual(stopwatch.ElapsedMilliseconds, 5000, "File renaming took too long!");

        // Check the memory usage
        long memoryUsage = Process.GetCurrentProcess().PrivateMemorySize64;
        UnityEngine.Debug.Log($"Memory usage after processing: {memoryUsage / 1024 / 1024} MB");
        Assert.LessOrEqual(memoryUsage / 1024 / 1024, 500, "Memory usage exceeded the limit!");

        DeleteTestDirectory();
    }


    public List<string> GenerateLargeTestFiles(int numberOfFiles, string filesFormat)
    {
        List<string> filePaths = new List<string>();

        if (!Directory.Exists(_testDirectory))
        {
            Directory.CreateDirectory(_testDirectory);
        }

        for (int i = 0; i < numberOfFiles; i++)
        {
            string filePath = Path.Combine(_testDirectory, $"test_file_{i + 1}.{filesFormat}");
            File.Create(filePath).Dispose();
            filePaths.Add(filePath);
        }

        return filePaths;
    }

    public void DeleteTestDirectory()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, recursive: true);
        }
    }

    #endregion
}

