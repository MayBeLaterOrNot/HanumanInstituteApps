﻿using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace HanumanInstitute.YangDownloader.Views;

/// <summary>
/// Interaction logic for MainView.xaml
/// </summary>
public partial class MainView : Window
{
    public MainView()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    public MainViewModel ViewModel => (MainViewModel)DataContext!;
}
