using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using SukiUI;

namespace MCLaunch.Page;

public partial class Start : UserControl
{
    public Start()
    {
        InitializeComponent();
        SukiTheme.GetInstance().ChangeBaseTheme(ThemeVariant.Dark);
    }
}