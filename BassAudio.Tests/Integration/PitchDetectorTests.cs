﻿using System.IO;
using System.IO.Abstractions;
using ManagedBass;
using Xunit.Abstractions;

// ReSharper disable MemberCanBePrivate.Global

namespace HanumanInstitute.BassAudio.Tests.Integration;

public class PitchDetectorTests
{
    public PitchDetectorTests(ITestOutputHelper output)
    {
        _output = output;
    }

    private readonly ITestOutputHelper _output;

    public PitchDetector Model => _model ??= SetupModel();
    private PitchDetector _model;
    private PitchDetector SetupModel()
    {
        return new PitchDetector(FileSystem);
    }

    public IFileSystemService FileSystem => _fileSystem ??= new FileSystemService(new FileSystem(), new WindowsApiService());
    private IFileSystemService _fileSystem;

    [Theory]
    [InlineData("/run/media/hanuman/Storage-ntfs/Music/INNA/Inna/")]
    [InlineData("/run/media/hanuman/Storage-ntfs/Music/Symphony X - V The New Mythology Suite/")]
    [InlineData("/run/media/hanuman/Storage-ntfs/Music/DJ Project/DJ Project - Experience/")]
    public void DetectPitch(string dir)
    {
        foreach (var file in Directory.GetFiles(dir, "*.mp3"))
        {
            // var file = "SourceLong.mp3";
            var pitch = Model.GetPitch(file);

            _output.WriteLine($"{Path.GetFileName(file)}: {pitch.ToStringInvariant()}");
            Assert.True(pitch is > 400 and < 450);
        }
    }

    [Fact]
    public void Detect()
    {
        // BassDevice.Instance.Init();
        Bass.Init();
        var filePath = "SourceLong2.mp3";

        var chan = Bass.CreateStream(filePath, Flags: BassFlags.Decode).Valid();
        var fft = new float[512];
        var read = Bass.ChannelGetData(chan, fft, (int)DataFlags.FFT1024 | 100000);

        foreach (var f in fft)
        {
            _output.WriteLine(f.ToStringInvariant());
        }
        // Assert.Contains(fft, x => x > 0);
    }
}
