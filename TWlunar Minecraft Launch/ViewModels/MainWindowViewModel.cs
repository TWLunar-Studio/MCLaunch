using System;
using Avalonia.Collections;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MinecraftLaunch.Base.Models.Game;
using SukiUI;
using SukiUI.Models;
using SukiUI.Toasts;

namespace TWlunar_Minecraft_Launch.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly SukiTheme _theme;
    
    public IAvaloniaReadOnlyList<SukiColorTheme> Themes { get; }
    
    public ISukiToastManager ToastManager { get; } = new SukiToastManager();

    [ObservableProperty]
    private ViewModelBase _currentPage ;
    
    [ObservableProperty] private ThemeVariant _baseTheme = ThemeVariant.Light;

    [ObservableProperty]
    private MinecraftEntry? _selectedVersion;

    public static MainWindowViewModel? Instance { get; private set; }

    public MainWindowViewModel()
    {
        Instance = this;
        _theme = SukiTheme.GetInstance();
        Themes = _theme.ColorThemes;
        SukiTheme.GetInstance().OnBaseThemeChanged += variant =>
        {
            BaseTheme = variant;
            Console.WriteLine("主题切换喵~.当前{0}",variant);
            ToastManager.CreateToast()
                .Dismiss().After(TimeSpan.FromSeconds(3))
                .Dismiss().ByClicking()
                .WithTitle("主题切换")
                .WithContent($"主题切换~当前主题{variant}喵~")
                .Queue();
        };
        
    }
    [RelayCommand]
    private void ToggleBaseTheme() =>
        _theme.SwitchBaseTheme();
}