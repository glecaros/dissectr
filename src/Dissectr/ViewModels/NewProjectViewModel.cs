using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace Dissectr.ViewModels;

public class NewProjectViewModel: ObservableObject
{
    public ICommand IntervalLengthChanged { get; }

    private string name = string.Empty;
    public string Name
    {
        get => name;
        set => SetProperty(ref name, value);
    }

    private TimeSpan intervalLength = TimeSpan.FromSeconds(10);
    public TimeSpan IntervalLength
    {
        get => intervalLength;
        set => SetProperty(ref intervalLength, value);
    }

    public NewProjectViewModel()
    {
        IntervalLengthChanged = new RelayCommand<ValueChangedEventArgs>(IntervalLengthChangedHandler);
    }
    private void IntervalLengthChangedHandler(ValueChangedEventArgs args)
    {
        IntervalLength = TimeSpan.FromSeconds(args.NewValue);
    }
}
