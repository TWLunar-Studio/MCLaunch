using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using MinecraftLaunch;
using Avalonia.Markup.Xaml;
using TWlunar_Minecraft_Launch.ViewModels;
using TWlunar_Minecraft_Launch.Views;

namespace TWlunar_Minecraft_Launch;

public partial class App : Application
{
    public App()
    {
        InitializeHelper.Initialize(settings => {
            settings.MaxThread = 256; // 最大下载线程
            settings.MaxFragment = 128; // 最大文件分片数量
            settings.MaxRetryCount = 4; // 最大下载重试次数
            settings.IsEnableMirror = true; // 是否启用 Minecraft 国内下载镜像源（BMCLAPI）
            settings.IsEnableFragment = true; // 是否启用分片下载
            settings.CurseForgeApiKey = "$2a$10$yXBSeP9DfAZGiZq9jrUDk.ZGQvcEXN3aybUVA5TbwJxz00P4WU9Sm"; // Curseforge API Key 设置
            settings.UserAgent = "MLTest/1.0"; // 自定义请求时所使用的 User Agent
        });
    }
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            var viewModel = new MainWindowViewModel();
            desktop.MainWindow = new MainWindow
            {
                DataContext = viewModel,
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}