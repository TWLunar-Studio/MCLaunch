using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using TWlunar_Minecraft_Launch.ViewModels;
using MinecraftLaunch.Components.Installer;

namespace TWlunar_Minecraft_Launch.Views;

public partial class InstallConfig : UserControl
{
    public InstallConfig()
    {
        InitializeComponent();
    }

    public InstallConfig(InstallConfigViewModel viewModel) : this()
    {
        DataContext = viewModel;
        _ = InitializeViewModelAsync();
    }

    private async Task InitializeViewModelAsync()
    {
        if (DataContext is InstallConfigViewModel viewModel)
        {
            await viewModel.InitializeAsync();
        }
    }

    private void CancelButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var parent = this.Parent;
        while (parent != null)
        {
            if (parent is MainWindow mainWindow)
            {
                mainWindow.CloseBorderButton_OnClick(sender, e);
                break;
            }
            parent = (parent as Control)?.Parent;
        }
    }

    private async void InstallButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is InstallConfigViewModel viewModel)
        {
            // 验证安装路径
            if (string.IsNullOrWhiteSpace(viewModel.InstallPath))
            {
                viewModel.InstallProgressText = "错误: 安装路径不能为空";
                return;
            }

            // 验证Java路径（如果需要安装Forge、Optifine或Quilt）
            if ((viewModel.InstallWithForge || viewModel.InstallWithOptifine || viewModel.InstallWithQuilt) && 
                string.IsNullOrWhiteSpace(viewModel.SelectedJavaPath))
            {
                viewModel.InstallProgressText = "警告: 未选择Java路径，某些组件可能无法正常安装";
            }
            
            // 执行安装
            bool success = await viewModel.InstallAsync();
            
            if (!success)
            {
                viewModel.InstallProgressText = "安装失败，请检查错误信息";
            }
        }
    }
}