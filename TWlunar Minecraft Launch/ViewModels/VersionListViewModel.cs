using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using MinecraftLaunch.Base.Models.Game;
using MinecraftLaunch.Components.Parser;
namespace TWlunar_Minecraft_Launch.ViewModels;

public partial class VersionListViewModel : ViewModelBase
{
    MinecraftParser minecraftParser = ".\\.minecraft";
    [ObservableProperty]
    private List<MinecraftEntry> _versions;

    [ObservableProperty] private MinecraftEntry _selectedVersion;
    
    [ObservableProperty] private string _iconSource;

    public VersionListViewModel()
    {
        IconSource = "avares://TWlunar_Minecraft_Launch/Assets/Icons/version_icon.png";
        Console.WriteLine("VersionListViewModel: 构造函数被调用");
        try
        {
            Versions = minecraftParser.GetMinecrafts();
            Console.WriteLine($"VersionListViewModel: 加载了 {Versions?.Count ?? 0} 个版本");
            if (Versions != null && Versions.Count > 0)
            {
                foreach (var version in Versions)
                {
                    Console.WriteLine($"版本: {version.Version.VersionId}");
                }
                // 手动触发同步，确保MainWindowViewModel被更新
                SyncToMainWindowViewModel();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"VersionListViewModel 错误: {ex.Message}");
            Versions = new List<MinecraftEntry>();
        }
    }

    partial void OnSelectedVersionChanged(MinecraftEntry value)
    {
        Console.WriteLine($"VersionListViewModel: OnSelectedVersionChanged 被调用, value = {(value?.Version.VersionId ?? "null")}");
        SyncToMainWindowViewModel();
    }

    private void SyncToMainWindowViewModel()
    {
        // 当选中版本变化时，同步到MainWindowViewModel
        if (SelectedVersion != null && MainWindowViewModel.Instance != null)
        {
            Console.WriteLine($"VersionListViewModel: 设置 MainWindowViewModel.SelectedVersion = {SelectedVersion.Version.VersionId}");
            MainWindowViewModel.Instance.SelectedVersion = SelectedVersion;
        }
        else
        {
            Console.WriteLine($"VersionListViewModel: 无法同步 - SelectedVersion = {(SelectedVersion != null ? SelectedVersion.Version.VersionId : "null")}, Instance = {(MainWindowViewModel.Instance != null ? "not null" : "null")}");
        }
    }
}