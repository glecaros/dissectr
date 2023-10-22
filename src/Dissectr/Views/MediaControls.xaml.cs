namespace Dissectr.Views;

public partial class MediaControls : ContentView
{
    public static readonly BindableProperty DurationProperty = BindableProperty.Create(
        nameof(Duration),
        typeof(TimeSpan),
        typeof(MediaControls),
        defaultBindingMode: BindingMode.OneWay);

    public static readonly BindableProperty PositionProperty = BindableProperty.Create(
        nameof(Position),
        typeof(TimeSpan),
        typeof(MediaControls),
        defaultBindingMode: BindingMode.OneWay);

    public event EventHandler Play;
    public event EventHandler Pause;
    public event EventHandler Stop;
    public event EventHandler<TimeSpan> SetPosition;

    public TimeSpan Duration
    {
        get => (TimeSpan)GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    public TimeSpan Position
    {
        get => (TimeSpan )GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
    }

    public MediaControls()
    {
        InitializeComponent();
    }

    private void zoomButtonClicked(object sender, EventArgs e)
    {

    }

    private void loopButtonClicked(object sender, EventArgs e)
    {

    }

    private void moveButtonClicked(object sender, EventArgs e)
    {

    }

    private void togglePlayButtonClicked(object sender, EventArgs e)
    {

    }

    private void stopButtonClicked(object sender, EventArgs e)
    {

    }

    private void dragStarted(object sender, EventArgs e)
    {

    }

    private void dragCompleted(object sender, EventArgs e)
    {

    }

    //static void OnMediaElementChanged(BindableObject bindable, object oldValue, object newValue)
    //{
    //    if (oldValue == newValue)
    //    {
    //        return;
    //    }
    //    var mediaControls = bindable as MediaControls;
    //    if (oldValue is MediaElement oldElement)
    //    {
    //        mediaControls.CleanMediaElement(oldElement);
    //    }
    //    if (newValue is MediaElement newElement)
    //    {
    //        mediaControls.InitMediaElement(newElement);
    //    }

    //}

    //private void InitMediaElement(MediaElement mediaElement)
    //{
    //    mediaElement.MediaOpened += OnMediaOpened;
    //    mediaElement.PositionChanged += OnPositionChanged;
    //}
    //private void CleanMediaElement(MediaElement mediaElement)
    //{
    //    mediaElement.MediaOpened -= OnMediaOpened;
    //    mediaElement.PositionChanged -= OnPositionChanged;
    //}

    //private void OnMediaOpened(object sender, EventArgs e)
    //{
    //    slider.Duration = MediaElement.Duration;
    //    slider.BindingContext = MediaElement;
    //    slider.SetBinding(PositionSlider.PositionProperty, MediaElement.PositionProperty.PropertyName, mode: BindingMode.TwoWay);
    //}

    //private void OnPositionChanged(object sender, MediaPositionChangedEventArgs e)
    //{
    //    //slider.Position = e.Position;

    //}
}
