using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using MinecraftLaunch.Base.Models.Network;
using MinecraftLaunch.Components.Installer;

namespace TWlunar_Minecraft_Launch.ViewModels;

public partial class InstallViewModel : ViewModelBase
{
    [ObservableProperty]
    private IEnumerable<VersionManifestEntry>? _versionList = [new VersionManifestEntry { Id = "正在加载版本列表..." }];
    [ObservableProperty]
    private List<VersionManifestEntry>? _releaseversionList = new();
    [ObservableProperty]
    private List<VersionManifestEntry>? _snapshotversionList = new();
    [ObservableProperty]
    private List<VersionManifestEntry>? _oldversionList = new();
    [ObservableProperty]
    public IEnumerable<VersionManifestEntry>? _allVersion;

    // 异步初始化方法
    public async Task LoadAsync()
    {
        try
        {
            AllVersion = await VanillaInstaller.EnumerableMinecraftAsync();
            VersionList = AllVersion;
            Console.WriteLine($"版本个数:{AllVersion.Count()}");
            foreach(var item in AllVersion)
            {
                if(item.Type == "release")
                    ReleaseversionList?.Add(item);
                else if(item.Type == "snapshot")
                    SnapshotversionList?.Add(item);
                else if(item.Type == "old_alpha" || item.Type == "old_beta")
                    OldversionList?.Add(item);
                Console.WriteLine($"{item.Id} {item.Type}");
            }
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("429"))
            {
                VersionList = [new VersionManifestEntry { Id = "加载版本列表失败: 访问过于频繁，请稍后再试。" }];
            }
            else
            {
                VersionList = [new VersionManifestEntry { Id = $"加载版本列表失败: {ex.Message}" }];
            }
            Console.WriteLine($"InstallViewModel 错误: {ex.Message}");
        }
        
    }

    public void to_old()
    {
        VersionList = OldversionList;
    }
    public void to_snapshot()
    {
        VersionList = SnapshotversionList;
    }
    public void to_release()
    {
        VersionList = ReleaseversionList;
    }

    public void to_all()
    {
        VersionList = AllVersion;
    }
}