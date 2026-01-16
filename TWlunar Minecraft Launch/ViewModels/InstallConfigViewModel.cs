using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using MinecraftLaunch.Base.Interfaces;
using MinecraftLaunch.Base.Models.Network;
using MinecraftLaunch.Components.Installer;
using MinecraftLaunch.Utilities;

namespace TWlunar_Minecraft_Launch.ViewModels;

public partial class InstallConfigViewModel : ViewModelBase
{
    [ObservableProperty]
    private VersionManifestEntry _selectedVersion;
    
    [ObservableProperty]
    private string _installPath = ".\\.minecraft";
    
    [ObservableProperty]
    private bool _installWithForge = false;
    
    [ObservableProperty]
    private bool _installWithFabric = false;
    
    [ObservableProperty]
    private bool _installWithOptifine = false;

    [ObservableProperty]
    private bool _installWithQuilt = false;

    [ObservableProperty]
    private string _selectedJavaPath = string.Empty;

    [ObservableProperty]
    private List<string> _javaPaths = new();

    [ObservableProperty]
    private string _installProgressText = "准备安装...";

    

    [ObservableProperty]
    private double _installProgress = 0.0;

    [ObservableProperty]
    private bool _isInstalling = false;

    private static List<string>? _cachedJavaPaths;

    public InstallConfigViewModel(VersionManifestEntry selectedVersion)
    {
        SelectedVersion = selectedVersion;
        // 如果有缓存的Java路径，直接使用
        if (_cachedJavaPaths != null && _cachedJavaPaths.Count > 0)
        {
            JavaPaths = new List<string>(_cachedJavaPaths);
            SelectedJavaPath = JavaPaths[0];
        }
    }

    public async Task InitializeAsync()
    {
        // 如果还没有缓存Java路径，则加载
        if (_cachedJavaPaths == null || _cachedJavaPaths.Count == 0)
        {
            await LoadJavaPathsAsync();
        }
        else
        {
            // 使用缓存的Java路径
            JavaPaths = new List<string>(_cachedJavaPaths);
            if (JavaPaths.Count > 0 && string.IsNullOrEmpty(SelectedJavaPath))
            {
                SelectedJavaPath = JavaPaths[0];
            }
        }
    }

    private async Task LoadJavaPathsAsync()
    {
        try
        {
            InstallProgressText = "正在扫描本地Java...";
            var paths = new List<string>();
            await foreach (var java in JavaUtil.EnumerableJavaAsync())
            {
                paths.Add(java.JavaPath);
            }
            
            // 缓存Java路径
            _cachedJavaPaths = paths;
            
            JavaPaths = paths;
            if (paths.Count > 0)
            {
                SelectedJavaPath = paths[0];
                InstallProgressText = $"已找到 {paths.Count} 个Java版本";
            }
            else
            {
                InstallProgressText = "未找到Java版本，请手动安装Java";
            }
        }
        catch (Exception ex)
        {
            InstallProgressText = $"加载Java路径失败: {ex.Message}";
        }
    }

    public async Task<bool> ValidateCombinationAsync()
    {
        // 验证Mod加载器组合
        int selectedCount = 0;
        if (InstallWithForge) selectedCount++;
        if (InstallWithFabric) selectedCount++;
        if (InstallWithOptifine) selectedCount++;

        // 原版安装
        if (selectedCount == 0)
            return true;

        // Quilt不支持与其他组件组合
        if (InstallWithQuilt && selectedCount > 1)
        {
            InstallProgressText = "错误: Quilt不支持与其他Mod加载器组合";
            return false;
        }

        // Fabric + OptiFine组合需要OptiFabric（需要额外安装Mod，这里只做提示）
        if (InstallWithFabric && InstallWithOptifine)
        {
            InstallProgressText = "注意: Fabric + OptiFine组合需要安装OptiFabric Mod";
        }

        // Forge + OptiFine是最常用的组合
        if (InstallWithForge && InstallWithOptifine)
        {
            InstallProgressText = "正在安装Forge + OptiFine组合";
        }

        return true;
    }

    public async Task<bool> InstallAsync()
    {
        if (IsInstalling) return false;
        
        IsInstalling = true;
        InstallProgress = 0.0;
        InstallProgressText = "开始安装...";

        try
        {
            // 验证组合
            if (!await ValidateCombinationAsync())
            {
                return false;
            }

            // 获取原版Minecraft条目
            var vanillaEntry = (await VanillaInstaller.EnumerableMinecraftAsync())
                .First(x => x.Id == SelectedVersion.Id);

            // 获取Java路径
            string javaPath = string.IsNullOrEmpty(SelectedJavaPath) && JavaPaths.Count > 0 
                ? JavaPaths[0] 
                : SelectedJavaPath;

            int selectedCount = 0;
            if (InstallWithForge) selectedCount++;
            if (InstallWithFabric) selectedCount++;
            if (InstallWithOptifine) selectedCount++;
            if (InstallWithQuilt) selectedCount++;

            // 原版安装
            if (selectedCount == 0)
            {
                InstallProgressText = "安装原版Minecraft...";
                var installer = VanillaInstaller.Create(InstallPath, vanillaEntry);
                
                installer.ProgressChanged += (_, arg) =>
                {
                    InstallProgress = arg.Progress * 100;
                    InstallProgressText = $"{arg.StepName} - {arg.FinishedStepTaskCount}/{arg.TotalStepTaskCount} - {InstallProgress:F2}%";
                };

                await installer.InstallAsync();
                InstallProgressText = "安装成功!";
                InstallProgress = 100.0;
                return true;
            }

            // Quilt单独安装
            if (InstallWithQuilt)
            {
                InstallProgressText = "获取Quilt版本列表...";
                var quiltVersions = await QuiltInstaller.EnumerableQuiltAsync(SelectedVersion.Id);
                if (quiltVersions.Any())
                {
                    var installer = QuiltInstaller.Create(InstallPath, quiltVersions.First());
                    
                    installer.ProgressChanged += (_, arg) =>
                    {
                        InstallProgress = arg.Progress * 100;
                        InstallProgressText = $"{arg.StepName} - {arg.FinishedStepTaskCount}/{arg.TotalStepTaskCount} - {InstallProgress:F2}%";
                    };

                    await installer.InstallAsync();
                    InstallProgressText = "Quilt安装成功!";
                    InstallProgress = 100.0;
                }
                return true;
            }

            // Fabric + OptiFine组合（需要OptiFabric Mod）
            if (InstallWithFabric && InstallWithOptifine)
            {
                InstallProgressText = "获取Fabric版本列表...";
                var fabricVersions = await FabricInstaller.EnumerableFabricAsync(SelectedVersion.Id);
                
                InstallProgressText = "获取OptiFine版本列表...";
                var optifineVersions = await OptifineInstaller.EnumerableOptifineAsync(SelectedVersion.Id);

                if (fabricVersions.Any() && optifineVersions.Any())
                {
                    var allEntries = new List<IInstallEntry> { vanillaEntry, fabricVersions.First(), optifineVersions.First() };

                    var installer = CompositeInstaller.Create(
                        allEntries, 
                        InstallPath, 
                        javaPath, 
                        $"{SelectedVersion.Id}-Fabric-OptiFine");

                    installer.ProgressChanged += (_, arg) =>
                    {
                        InstallProgress = arg.Progress * 100;
                        InstallProgressText = $"{arg.StepName} - {arg.FinishedStepTaskCount}/{arg.TotalStepTaskCount} - {InstallProgress:F2}%";
                    };

                    installer.Completed += (_, arg) =>
                    {
                        if (arg.IsSuccessful)
                        {
                            InstallProgressText = "安装成功! 注意: 需要安装OptiFabric Mod以使用OptiFine";
                            InstallProgress = 100.0;
                        }
                        else
                        {
                            InstallProgressText = $"安装失败: {arg.Exception?.Message}";
                        }
                    };

                    await installer.InstallAsync();
                }
                return true;
            }

            // Forge + OptiFine组合
            if (InstallWithForge && InstallWithOptifine)
            {
                InstallProgressText = "获取Forge版本列表...";
                var forgeVersions = await ForgeInstaller.EnumerableForgeAsync(SelectedVersion.Id);
                
                InstallProgressText = "获取OptiFine版本列表...";
                var optifineVersions = await OptifineInstaller.EnumerableOptifineAsync(SelectedVersion.Id);

                if (forgeVersions.Any() && optifineVersions.Any())
                {
                    var allEntries = new List<IInstallEntry> { vanillaEntry, forgeVersions.First(), optifineVersions.First() };

                    var installer = CompositeInstaller.Create(
                        allEntries, 
                        InstallPath, 
                        javaPath, 
                        $"{SelectedVersion.Id}-Forge-OptiFine");

                    installer.ProgressChanged += (_, arg) =>
                    {
                        InstallProgress = arg.Progress * 100;
                        InstallProgressText = $"{arg.StepName} - {arg.FinishedStepTaskCount}/{arg.TotalStepTaskCount} - {InstallProgress:F2}%";
                    };

                    installer.Completed += (_, arg) =>
                    {
                        if (arg.IsSuccessful)
                        {
                            InstallProgressText = "安装成功!";
                            InstallProgress = 100.0;
                        }
                        else
                        {
                            InstallProgressText = $"安装失败: {arg.Exception?.Message}";
                        }
                    };

                    await installer.InstallAsync();
                }
                return true;
            }

            // 单独安装Forge
            if (InstallWithForge)
            {
                InstallProgressText = "获取Forge版本列表...";
                var forgeVersions = await ForgeInstaller.EnumerableForgeAsync(SelectedVersion.Id);
                if (forgeVersions.Any())
                {
                    var installer = ForgeInstaller.Create(InstallPath, javaPath, forgeVersions.First());
                    
                    installer.ProgressChanged += (_, arg) =>
                    {
                        InstallProgress = arg.Progress * 100;
                        InstallProgressText = $"{arg.StepName} - {arg.FinishedStepTaskCount}/{arg.TotalStepTaskCount} - {InstallProgress:F2}%";
                    };

                    await installer.InstallAsync();
                    InstallProgressText = "Forge安装成功!";
                    InstallProgress = 100.0;
                }
                return true;
            }

            // 单独安装Fabric
            if (InstallWithFabric)
            {
                InstallProgressText = "获取Fabric版本列表...";
                var fabricVersions = await FabricInstaller.EnumerableFabricAsync(SelectedVersion.Id);
                if (fabricVersions.Any())
                {
                    var installer = FabricInstaller.Create(InstallPath, fabricVersions.First());
                    
                    installer.ProgressChanged += (_, arg) =>
                    {
                        InstallProgress = arg.Progress * 100;
                        InstallProgressText = $"{arg.StepName} - {arg.FinishedStepTaskCount}/{arg.TotalStepTaskCount} - {InstallProgress:F2}%";
                    };

                    await installer.InstallAsync();
                    InstallProgressText = "Fabric安装成功!";
                    InstallProgress = 100.0;
                }
                return true;
            }

            // 单独安装OptiFine
            if (InstallWithOptifine)
            {
                InstallProgressText = "获取OptiFine版本列表...";
                var optifineVersions = await OptifineInstaller.EnumerableOptifineAsync(SelectedVersion.Id);
                if (optifineVersions.Any())
                {
                    var installer = OptifineInstaller.Create(InstallPath, javaPath, optifineVersions.First());
                    
                    installer.ProgressChanged += (_, arg) =>
                    {
                        InstallProgress = arg.Progress * 100;
                        InstallProgressText = $"{arg.StepName} - {arg.FinishedStepTaskCount}/{arg.TotalStepTaskCount} - {InstallProgress:F2}%";
                    };

                    await installer.InstallAsync();
                    InstallProgressText = "OptiFine安装成功!";
                    InstallProgress = 100.0;
                }
                return true;
            }

            return true;
        }
        catch (Exception ex)
        {
            InstallProgressText = $"安装失败: {ex.Message}";
            return false;
        }
        finally
        {
            IsInstalling = false;
        }
    }
}
