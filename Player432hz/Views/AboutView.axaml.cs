using Avalonia.Markup.Xaml;

namespace HanumanInstitute.Player432hz.Views;

public partial class AboutView : CommonWindow<AboutViewModel>
{
    protected override void Initialize() => AvaloniaXamlLoader.Load(this);
}

