using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TWlunar_Minecraft_Launch.Models;

public class InstallerTask : INotifyPropertyChanged
{
    private string _name;
    public string Name
    {
        get => _name;
        set => SetField(ref _name, value);
    }

    private string _currentStepName;
    public string CurrentStepName
    {
        get => _currentStepName;
        set => SetField(ref _currentStepName, value);
    }

    private double _progressPercentage;
    public double ProgressPercentage
    {
        get => _progressPercentage;
        set => SetField(ref _progressPercentage, value);
    }

    private string _progressText;
    public string ProgressText
    {
        get => _progressText;
        set => SetField(ref _progressText, value);
    }

    private string _speedText;
    public string SpeedText
    {
        get => _speedText;
        set => SetField(ref _speedText, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

public class TaskQueue
{
    public ObservableCollection<InstallerTask> Tasks { get; } = new();
}
