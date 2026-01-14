using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using SukiUI.Controls;
using TWlunar_Minecraft_Launch.Utilities;

namespace TWlunar_Minecraft_Launch.Views;

public partial class MainWindow : SukiWindow
{
    private BorderAnimationHelper? _animationHelper;

    public MainWindow()
    {
        InitializeComponent();
        
        // 初始化动画助手
        Loaded += (s, e) =>
        {
            _animationHelper = new BorderAnimationHelper(
                BorderContent,
                BorderOverlay,
                ButtonClose,
                this
            );
        };
        
        // 监听窗口大小变化
        SizeChanged += (s, e) =>
        {
            if (_animationHelper != null && _animationHelper.IsOpen)
            {
                _animationHelper.UpdateSize();
            }
        };
    }

    /// <summary>
    /// 打开 Border 动画
    /// </summary>
    /// <param name="heightRatio">高度比例，默认为 1.0（全屏），范围 0-1</param>
    public async void OpenBorder(double heightRatio = 1.0)
    {
        if (_animationHelper == null)
        {
            _animationHelper = new BorderAnimationHelper(
                BorderContent,
                BorderOverlay,
                ButtonClose,
                this
            );
        }
        
        await _animationHelper.OpenAsync(heightRatio);
    }

    public async void CloseBorderButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_animationHelper != null)
        {
            await _animationHelper.CloseAsync();
        }
    }
}