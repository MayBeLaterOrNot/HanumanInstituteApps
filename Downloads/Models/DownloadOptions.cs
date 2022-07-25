﻿using HanumanInstitute.BassAudio;

namespace HanumanInstitute.Downloads;

/// <summary>
/// Contains options for automatic download stream selection.
/// </summary>
[PropertyChanged.AddINotifyPropertyChangedInterface]
public class DownloadOptions
{
    /// <summary>
    /// Gets or sets the maximum height of the video stream to download. Useful to limit bandwidth usage.
    /// </summary>
    public int MaxQuality { get; set; }
    
    /// <summary>
    /// Gets or sets the preferred video stream type to download.
    /// </summary>
    public StreamContainerOption PreferredVideo { get; set; } = StreamContainerOption.Best;
    
    /// <summary>
    /// Gets or sets the preferred audio stream type to download.
    /// </summary>
    public StreamContainerOption PreferredAudio { get; set; } = StreamContainerOption.Best;
    
    /// <summary>
    /// Gets or sets the maximum amount of simultaneous downloads to allow.
    /// </summary>
    public int ConcurrentDownloads { get; set; } = 2;
    
    /// <summary>
    /// Gets or sets optional audio encode settings, if we want to re-encode the audio.
    /// </summary>
    public EncodeSettings? EncodeAudio { get; set; }

    /// <summary>
    /// Returns a copy of this instance.
    /// </summary>
    public DownloadOptions Clone() => (DownloadOptions)MemberwiseClone();
}
