using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MinecraftLaunch;
using MinecraftLaunch.Base.Models.Game;
using MinecraftLaunch.Components.Authenticator;
using MinecraftLaunch.Components.Parser;
using MinecraftLaunch.Extensions;
using MinecraftLaunch.Launch;
using MinecraftLaunch.Utilities;

namespace TWlunar_Minecraft_Launch.ViewModels;

public partial class StartViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _selectedVersion = "未选择版本";

    public StartViewModel()
    {
        Console.WriteLine("StartViewModel: 构造函数被调用");
        // 尝试立即订阅
        SubscribeToMainWindowViewModel();
    }

    private async Task<MinecraftProcess> Go()
    {
        MinecraftParser minecraftParser = ".minecraft";
        
        var minecraft = minecraftParser.GetMinecraft("1.12.2");
        Console.WriteLine(minecraft.Id);
        
        var javas = await JavaUtil.EnumerableJavaAsync().ToListAsync();
        var java = minecraft.GetAppropriateJava(javas);
        
        var authenticator = new MicrosoftAuthenticator("YOUR_CLIENT_ID");
        
        var account = new OfflineAuthenticator().Authenticate("PLAYER_NAME");
        
        MinecraftRunner runner = new(new LaunchConfig {
            Account = account,
            MaxMemorySize = 2048,
            MinMemorySize = 512,
            LauncherName = "MinecraftLaunch",
            JavaPath = java,
        }, minecraftParser);
        var process = await runner.RunAsync(minecraft);

        return process;
    }
    
    private void SubscribeToMainWindowViewModel()
    {
        try
        {
            Console.WriteLine($"StartViewModel: MainWindowViewModel.Instance = {(MainWindowViewModel.Instance != null ? "not null" : "null")}");
            
            if (MainWindowViewModel.Instance != null)
            {
                UpdateSelectedVersion();
                MainWindowViewModel.Instance.PropertyChanged += (s, e) =>
                {
                    Console.WriteLine($"StartViewModel: PropertyChanged事件触发, PropertyName = {e.PropertyName}");
                    if (e.PropertyName == nameof(MainWindowViewModel.SelectedVersion))
                    {
                        UpdateSelectedVersion();
                    }
                };
                Console.WriteLine("StartViewModel: 成功订阅MainWindowViewModel.PropertyChanged");
            }
            else
            {
                Console.WriteLine("StartViewModel: MainWindowViewModel.Instance 为 null，将延迟重试");
                // 如果Instance为null，延迟重试
                Task.Run(async () =>
                {
                    await Task.Delay(200);
                    Avalonia.Threading.Dispatcher.UIThread.Post(() => SubscribeToMainWindowViewModel());
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"StartViewModel 订阅失败: {ex.Message}");
        }
    }

    private void UpdateSelectedVersion()
    {
        try
        {
            Console.WriteLine($"StartViewModel: UpdateSelectedVersion被调用");
            if (MainWindowViewModel.Instance?.SelectedVersion != null)
            {
                SelectedVersion = MainWindowViewModel.Instance.SelectedVersion.Version.VersionId;
                Console.WriteLine($"StartViewModel: 更新版本为 {SelectedVersion}");
            }
            else
            {
                SelectedVersion = "未选择版本";
                Console.WriteLine("StartViewModel: MainWindowViewModel.Instance 或 SelectedVersion 为 null");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"StartViewModel UpdateSelectedVersion 错误: {ex.Message}");
        }
    }
}