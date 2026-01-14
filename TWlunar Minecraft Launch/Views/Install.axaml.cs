using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MinecraftLaunch.Base.Models.Network;
using TWlunar_Minecraft_Launch.ViewModels;

namespace TWlunar_Minecraft_Launch.Views;

public partial class Install : UserControl
{
    protected override async void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        // 获取 DataContext 并调用异步方法
        if (DataContext is InstallViewModel vm)
        {
            await vm.LoadAsync();
        }
    }
    public Install()
    {
        InitializeComponent();
        DataContext = new InstallViewModel();
    }

    private void Version_filterList_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is not InstallViewModel vm) return;
        if (VersionfilterList.SelectedItem is ListBoxItem selectedItem)
        {
            string? content = selectedItem.Content?.ToString();
            if (content == "全部版本")
                vm.to_all();
            else if (content == "正式版")
                vm.to_release();
            else if (content == "预览版")
                vm.to_snapshot();
            else if (content == "考古版")
                vm.to_old();
        }
    }

    private void VersionListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox listBox && listBox.SelectedItem is VersionManifestEntry selectedVersion)
        {
            Console.WriteLine($"选择了版本: {selectedVersion.Id}");
            
            // 查找 MainWindow 并打开配置界面
            var parent = this.Parent;
            while (parent != null)
            {
                if (parent is MainWindow mainWindow)
                {
                    // 创建配置 ViewModel
                    var configViewModel = new InstallConfigViewModel(selectedVersion);
                    
                    // 打开 Border，使用全屏覆盖（heightRatio = 1.0）
                    mainWindow.OpenBorder(1.0);
                    
                    // 设置 CurrentPage
                    var viewModel = mainWindow.DataContext as MainWindowViewModel;
                    if (viewModel != null)
                    {
                        viewModel.CurrentPage = configViewModel;
                    }
                    break;
                }
                parent = (parent as Control)?.Parent;
            }
        }
    }
}