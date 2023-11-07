using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dissectr.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Dissectr.ViewModels;

public partial class NewProjectViewModel: ObservableObject
{
    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private TimeSpan intervalLength = TimeSpan.FromSeconds(10);

    [ObservableProperty]
    public ObservableCollection<Dimension> dimensions;

    public NewProjectViewModel()
    {
        //IntervalLengthChanged = new RelayCommand<ValueChangedEventArgs>(IntervalLengthChangedHandler);
     //   AddDimensionOptionCommand = new RelayCommand<Dimension>(AddDimensionOptionHandler);
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

    [RelayCommand]
    private void Browse()
    {
        throw new NotImplementedException();
    }

    [RelayCommand]
    private void IntervalLengthChanged(ValueChangedEventArgs args)
    {
        IntervalLength = TimeSpan.FromSeconds(args.NewValue);
    }

    [RelayCommand]
    private void AddDimension()
    {
        Dimensions.Add(new Dimension());
    }

    [RelayCommand]
    private void RemoveDimension(Dimension? dimension)
    {
        if (dimension is null)
        {
            return;
        }
        Dimensions.Remove(dimension);
    }

    [RelayCommand]
    private void AddDimensionOption(Dimension? dimension)
    {
        if (dimension is null)
        {
            return;
        }
        var maxCode = dimension.DimensionOptions.Select(d => d.Code).Max();
        dimension.DimensionOptions.Add(new DimensionOption(maxCode + 1, "New option"));
    }

    [RelayCommand]
    private void RemoveDimensionOption(DimensionOption? dimensionOption)
    {
        if (dimensionOption is null)
        {
            return;
        }
        var dimension = Dimensions.FirstOrDefault(d => d.DimensionOptions.Contains(dimensionOption));
        if (dimension is null)
        {
            return;
        }
        dimension.DimensionOptions.Remove(dimensionOption);
    }
}
