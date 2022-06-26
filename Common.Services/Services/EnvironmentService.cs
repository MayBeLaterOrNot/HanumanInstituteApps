﻿using System.Collections.Generic;
using System.IO;
using System.Reflection;

// ReSharper disable CheckNamespace
namespace HanumanInstitute.Common.Services;

/// <inheritdoc />
public class EnvironmentService : IEnvironmentService
{
    /// <inheritdoc />
    public IEnumerable<string> CommandLineArguments => Environment.GetCommandLineArgs();
    /// <inheritdoc />
    public Version AppVersion => Assembly.GetEntryAssembly()?.GetName().Version ?? new Version();
    /// <inheritdoc />
    public string AppFriendlyName => AppDomain.CurrentDomain.FriendlyName;
    /// <inheritdoc />
    public string ApplicationDataPath => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    /// <inheritdoc />
    public string AppDirectory => AppDomain.CurrentDomain.BaseDirectory;
    /// <inheritdoc />
    public string SystemRootDirectory => Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)) ?? string.Empty;
    /// <inheritdoc />
    public string ProgramFilesX86 => Environment.GetFolderPath(Environment.Is64BitOperatingSystem ? Environment.SpecialFolder.ProgramFilesX86 : Environment.SpecialFolder.ProgramFiles);
    /// <inheritdoc />
    public char DirectorySeparatorChar => Path.DirectorySeparatorChar;
    /// <inheritdoc />
    public char AltDirectorySeparatorChar => Path.AltDirectorySeparatorChar;
    /// <inheritdoc />
    public DateTime Now => DateTime.Now;
    /// <inheritdoc />
    public DateTime UtcNow => DateTime.UtcNow;
    /// <inheritdoc />
    public int ProcessorCount => Environment.ProcessorCount;
}
