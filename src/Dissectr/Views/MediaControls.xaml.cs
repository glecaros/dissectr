namespace Dissectr.Views;

public partial class MediaControls : ContentView
{
    public static readonly BindableProperty DurationProperty = BindableProperty.Create(
        nameof(Duration),
        typeof(TimeSpan),
        typeof(MediaControls));

    public static readonly BindableProperty PositionProperty = BindableProperty.Create(
        nameof(Position),
        typeof(TimeSpan),
        typeof(MediaControls));

    public static readonly BindableProperty IsPlayingProperty = BindableProperty.Create(
        nameof(IsPlaying),
        typeof(bool),
        typeof(MediaControls));

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

    public bool IsPlaying
    {
        get => (bool)GetValue(IsPlayingProperty);
        set => SetValue(IsPlayingProperty, value);
    }

    public MediaControls()
    {
        InitializeComponent();
    }

    public event EventHandler Play;
    public event EventHandler Pause;
    public event EventHandler Stop;
    public event EventHandler<TimeSpan> Seek;


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

    private bool wasPlaying = false;

    private void dragStarted(object sender, EventArgs e)
    {
        slider.RemoveBinding(PositionProperty);
        wasPlaying = IsPlaying;
        if (wasPlaying)
        {
            Pause?.Invoke(this, EventArgs.Empty);
        }
    }

    private void dragCompleted(object sender, EventArgs e)
    {
        var position = slider.Position;
        Seek?.Invoke(this, position);
        if (wasPlaying)
        {
            Play?.Invoke(this, EventArgs.Empty);
        }

    }
}