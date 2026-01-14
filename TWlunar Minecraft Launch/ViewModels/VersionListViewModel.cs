using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    public VersionListViewModel()
    {
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
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"VersionListViewModel 错误: {ex.Message}");
            Versions = new List<MinecraftEntry>();
        }
    }
}