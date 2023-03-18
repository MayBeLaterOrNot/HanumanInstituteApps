﻿using System.Diagnostics.CodeAnalysis;
using HanumanInstitute.Common.Avalonia.App;
using Splat;

namespace HanumanInstitute.Player432Hz;

// ReSharper disable once ClassNeverInstantiated.Global
public class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SettingsPlaylistItem))]
    [STAThread]
    public static void Main(string[] args) => AppStarter.Start<App>(args,
        () => ViewModelLocator.SettingsProvider.Value,
        () => Locator.Current.GetService<IAppPathService>()?.UnhandledExceptionLogPath);

    // Avalonia configuration, don't remove; also used by visual designer.
    // ReSharper disable once MemberCanBePrivate.Global
    public static AppBuilder BuildAvaloniaApp() => AppStarter.BuildAvaloniaApp<App>();
}
