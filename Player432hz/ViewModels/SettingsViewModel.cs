using FluentAvalonia.Styling;
using HanumanInstitute.Common.Avalonia.App;

namespace HanumanInstitute.Player432hz.ViewModels;

public class SettingsViewModel : SettingsViewModel<AppSettingsData>
{
    public SettingsViewModel(ISettingsProvider<AppSettingsData> settingsProvider, FluentAvaloniaTheme fluentTheme) :
        base(settingsProvider, fluentTheme)
    {
    }

    protected override void RestoreDefaultImpl()
    {
        Settings.AntiAlias = false;
        Settings.AntiAliasLength = 32;
        Settings.Speed = 1;
        Settings.AutoDetectPitch = true;
        Settings.PitchFrom = 440;
        Settings.PitchTo = 432;
    }
}
