using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dissectr.Models;
using System.Collections.ObjectModel;
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

    public ObservableCollection<Dimension> Dimensions { get; }

    public NewProjectViewModel()
    {
        IntervalLengthChanged = new RelayCommand<ValueChangedEventArgs>(IntervalLengthChangedHandler);
        Dimensions = new ObservableCollection<Dimension>
        {
            new()
            {
                Name = "CAT1",
                DimensionOptions = new()
                {
                    new(Code: 0, Name: "CAT1OP0"),
                    new(Code: 1, Name: "CAT1OP1"),
                }
            },
            new()
            {
                Name = "CAT2",
                DimensionOptions = new()
                {
                    new(Code: 0, Name: "CAT2OP0"),
                    new(Code: 1, Name: "CAT2OP1"),
                    new(Code: 1, Name: "CAT2OP2"),
                }
            },
            new()
            {
                Name = "CAT3",
                Optional = true,
                DimensionOptions = new()
                {
                    new(Code: 0, Name: "CAT3OP0"),
                }
            },
        };
    }
    private void IntervalLengthChangedHandler(ValueChangedEventArgs args)
    {
        IntervalLength = TimeSpan.FromSeconds(args.NewValue);
    }
}
