using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using SukiUI.Controls;

namespace TWlunar_Minecraft_Launch.Utilities;

public class BorderAnimationHelper
{
    private readonly GlassCard _border;
    private readonly Border _overlayBorder;
    private readonly Button _closeButton;
    private readonly Window _window;
    private double _currentHeightRatio = 0;
    private bool _isOpen = false;

    public BorderAnimationHelper(GlassCard border, Border overlayBorder, Button closeButton, Window window)
    {
        _border = border;
        _overlayBorder = overlayBorder;
        _closeButton = closeButton;
        _window = window;
        
        // 确保初始状态完全隐藏
        SetClosedState();
    }

    /// <summary>
    /// 打开 Border 动画
    /// </summary>
    /// <param name="heightRatio">高度比例，默认为 1/3，范围 0-1</param>
    public async Task OpenAsync(double heightRatio = 1.0 / 3.0)
    {
        if (_isOpen) return;
        _isOpen = true;
        _currentHeightRatio = heightRatio;

        // 限制高度比例在 0-1 之间
        heightRatio = Math.Clamp(heightRatio, 0, 1);

        double targetHeight = _window.Height * heightRatio;
        if (double.IsNaN(targetHeight) || targetHeight <= 0)
        {
            targetHeight = _window.Height / 3; // 默认值
        }

        Console.WriteLine($"打开动画 - 目标高度: {targetHeight}, 窗口高度: {_window.Height}, 比例: {heightRatio}");

        // 从 0 开始动画到目标高度
        for (int i = 0; i <= targetHeight; i += 50)
        {
            _border.Height = i;
            await Task.Delay(1);
        }

        _closeButton.Height = 25;
        _closeButton.Width = 25;
        _overlayBorder.Height = _window.Height; // 覆盖整个窗口高度
        _overlayBorder.Width = _window.Width;
    }

    /// <summary>
    /// 关闭 Border 动画
    /// </summary>
    public async Task CloseAsync()
    {
        if (!_isOpen) return;
        _isOpen = false;

        // 获取当前 Border 的实际高度
        double currentHeight = _border.Bounds.Height;
        if (double.IsNaN(currentHeight) || currentHeight <= 0)
        {
            currentHeight = _window.Height / 3; // 默认值
        }

        Console.WriteLine($"关闭动画 - 当前高度: {currentHeight}");

        // 从当前高度动画到 0
        for (int i = (int)currentHeight; i >= 0; i -= 50)
        {
            _border.Height = i;
            await Task.Delay(1);
        }

        SetClosedState();
    }

    /// <summary>
    /// 更新 Border 大小以适应窗口变化
    /// </summary>
    public void UpdateSize()
    {
        if (_isOpen)
        {
            double targetHeight = _window.Height * _currentHeightRatio;
            _border.Height = targetHeight;
            _overlayBorder.Height = _window.Height;
            _overlayBorder.Width = _window.Width;
        }
    }

    /// <summary>
    /// 设置关闭状态
    /// </summary>
    private void SetClosedState()
    {
        _closeButton.Height = 0;
        _closeButton.Width = 0;
        _overlayBorder.Height = 0;
        _overlayBorder.Width = 0;
        _border.Height = 0;
    }

    /// <summary>
    /// 检查 Border 是否已打开
    /// </summary>
    public bool IsOpen => _isOpen;
}