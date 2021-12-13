﻿using System;
using HanumanInstitute.Player432hz.Business;

namespace HanumanInstitute.Player432hz.ViewModels
{
    /// <summary>
    /// Represents the media player.
    /// </summary>
    public interface IPlayerViewModel
    {
        /// <summary>
        /// Gets an instance of IPlaylistPlayer that can be bound to the UI for playback.
        /// </summary>
        IPlaylistPlayer Player { get; }

        /// <summary>
        /// Plays the next file when playback ends.
        /// </summary>
        void MediaFinished();
    }
}