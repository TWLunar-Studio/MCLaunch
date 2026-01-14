using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using SukiUI.Controls;

namespace TWlunar_Minecraft_Launch.Views;

public partial class MainWindow : SukiWindow
{
    private bool _isBorderOpen = false;

    public MainWindow()
    {
        InitializeComponent();
    }

    public async void OpenBorder()
    {
        if (_isBorderOpen) return;
        _isBorderOpen = true;

        // 动态计算目标高度：窗口高度的 2/3（因为 Grid.Row="1" 设置为 2*）
        double targetHeight = this.Height * 2 / 3;
        if (double.IsNaN(targetHeight) || targetHeight <= 0)
        {
            targetHeight = 300; // 如果无法获取窗口高度，使用默认值
        }

        Console.WriteLine($"目标高度: {targetHeight}, 窗口高度: {this.Height}");

        // 从 0 开始动画到目标高度
        for (int i = 0; i <= targetHeight; i += 50)
        {
            Border2.Height = i;
            await System.Threading.Tasks.Task.Delay(1);
        }
        Button1.Height = 25;
        Border1.Height = double.NaN; // 使用自动高度
    }

    private async void CloseBorderButton_OnClick(object? sender, RoutedEventArgs e)
    {
        _isBorderOpen = false;

        // 获取当前 Border2 的实际高度
        double currentHeight = Border2.Bounds.Height;
        if (double.IsNaN(currentHeight) || currentHeight <= 0)
        {
            currentHeight = 300; // 如果无法获取高度，使用默认值
        }

        // 从当前高度动画到 0
        for (int i = (int)currentHeight; i >= 0; i -= 50)
        {
            Border2.Height = i;
            await System.Threading.Tasks.Task.Delay(1);
        }

        Button1.Height = 0;
        Border1.Height = 0;
        Border2.Height = 0;
    }
}