namespace Dissectr.Views;

public class PositionSlider : Slider
{
    public static readonly BindableProperty DurationProperty = BindableProperty.Create(nameof(Duration), typeof(TimeSpan), typeof(PositionSlider), new TimeSpan(1),
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            double seconds = ((TimeSpan)newValue).TotalSeconds;
            var slider = bindable as Slider;
            slider.IsEnabled = seconds != 0;
            slider.Maximum = seconds;
        });

    public static readonly BindableProperty PositionProperty = BindableProperty.Create(nameof(Position), typeof(TimeSpan), typeof(PositionSlider), new TimeSpan(0),
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            double seconds = ((TimeSpan)newValue).TotalSeconds;
            var slider = bindable as Slider;
            slider.Value = seconds;
        });

    public TimeSpan Duration
    {
        get => (TimeSpan)GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    public TimeSpan Position
    {
        get => (TimeSpan)GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
    }

    public PositionSlider()
    {
        PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == "Value")
            {
                TimeSpan newPosition = TimeSpan.FromSeconds(Value);
                if (Math.Abs(newPosition.TotalSeconds - Position.TotalSeconds) / Duration.TotalSeconds > 0.01)
                {
                    Position = newPosition;
                }
            }
        };
    }
}