using System;
using System.Linq;
using System.Threading.Tasks;
using MinecraftLaunch.Base.Enums;
using MinecraftLaunch.Components.Downloader;
using MinecraftLaunch.Components.Installer;

namespace MCLaunch.Models;

public class Install
{
    private readonly string _Version;
    public Install(string V)
    {
        _Version = V;
    }
    public async Task AInstall()
    { 
        var mc = (await VanillaInstaller.EnumerableMinecraftAsync())
            .First(x => x.McVersion.Equals(_Version));

        var forgeEntry = (await ForgeInstaller.EnumerableForgeAsync(_Version))
            .First();

        var Optifine = (await OptifineInstaller.EnumerableOptifineAsync(_Version))
            .First();

        var installer5 = CompositeInstaller.Create([mc, forgeEntry], ".\\.minecraft", "C:\\Program Files\\Microsoft\\jdk-21.0.7.6-hotspot\\bin\\javaw.exe", "ForgeMC");
        installer5.ProgressChanged += (_, arg) =>
            Console.WriteLine($"{(arg.PrimaryStepName is InstallStep.Undefined ? "" : $"{arg.PrimaryStepName} - ")}{arg.StepName} - {arg.FinishedStepTaskCount}/{arg.TotalStepTaskCount} - {(arg.IsStepSupportSpeed ? $"{DefaultDownloader.FormatSize(arg.Speed, true)} - {arg.Progress * 100:0.00}%" : $"{arg.Progress * 100:0.00}%")}");

        installer5.Completed += (_, arg) =>
            Console.WriteLine(arg.IsSuccessful ? "安装成功" : $"安装失败 - {arg.Exception}");

        var installer6 = OptifineInstaller.Create(".\\.minecraft", ".\\Java", Optifine);

        var minecraft5 = await installer5.InstallAsync();
        Console.WriteLine(minecraft5.Id);
    }
}