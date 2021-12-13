﻿using System;
using System.Collections.Generic;

namespace HanumanInstitute.Player432hz.Business
{
    public interface IAppPathService
    {
        /// <summary>
        /// Returns all valid video extensions.
        /// </summary>
        IList<string> VideoExtensions { get; }
        /// <summary>
        /// Returns all valid audio extensions
        /// </summary>
        IList<string> AudioExtensions { get; }
        /// <summary>
        /// Returns all valid image extensions.
        /// </summary>
        IList<string> ImageExtensions { get; }
        /// <summary>
        /// Returns the path where unhandled exceptions are logged.
        /// </summary>
        string UnhandledExceptionLogPath { get; }
        /// <summary>
        /// Returns the path where the 432hz Player is storing its Avisynth script during playback.
        /// </summary>
        string Player432hzScriptFile { get; }
        /// <summary>
        /// Returns the path where the 432hz Player settings file is stored.
        /// </summary>
        string Player432hzConfigFile { get; }
        /// <summary>
        /// Returns the relative path to access the temp folder within the Natural Grounding folder.
        /// </summary>
        string LocalTempPath { get; }
        /// <summary>
        /// Returns the system temp folder.
        /// </summary>
        string SystemTempPath { get; }
    }
}
