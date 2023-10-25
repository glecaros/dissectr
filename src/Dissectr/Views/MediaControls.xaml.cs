using System.Windows.Input;

namespace Dissectr.Views;

public partial class MediaControls : ContentView
{
    #region Duration Property
    public static readonly BindableProperty DurationProperty = BindableProperty.Create(
        nameof(Duration),
        typeof(TimeSpan),
        typeof(MediaControls));

    public TimeSpan Duration
    {
        get => (TimeSpan)GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }
    #endregion

    #region Position Property
    public static readonly BindableProperty PositionProperty = BindableProperty.Create(
        nameof(Position),
        typeof(TimeSpan),
        typeof(MediaControls));

    public TimeSpan Position
    {
        get => (TimeSpan)GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
    }
    #endregion

    #region IsPlaying Property
    public static readonly BindableProperty IsPlayingProperty = BindableProperty.Create(
        nameof(IsPlaying),
        typeof(bool),
        typeof(MediaControls));

    public bool IsPlaying
    {
        get => (bool)GetValue(IsPlayingProperty);
        set => SetValue(IsPlayingProperty, value);
    }
    #endregion

    #region Play Property
    public static readonly BindableProperty PlayProperty = BindableProperty.Create(nameof(Play), typeof(ICommand), typeof(MediaControls));
    
    public ICommand Play
    {
        get => (ICommand)GetValue(PlayProperty);
        set => SetValue(PlayProperty, value);
    }
    #endregion

    #region Pause Property
    public static readonly BindableProperty PauseProperty = BindableProperty.Create(nameof(Pause), typeof(ICommand), typeof(MediaControls));

    public ICommand Pause
    {
        get => (ICommand)GetValue(PauseProperty);
        set => SetValue(PauseProperty, value);
    }
    #endregion

    #region Seek Property
    public static readonly BindableProperty SeekProperty = BindableProperty.Create(nameof(Seek), typeof(ICommand), typeof(MediaControls));
    
    public ICommand Seek
    {
        get => (ICommand)GetValue(SeekProperty);
        set => SetValue(SeekProperty, value);
    }
    #endregion

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

    private bool wasPlaying = false;

    private void dragStarted(object sender, EventArgs e)
    {
        slider.RemoveBinding(PositionProperty);
        wasPlaying = IsPlaying;
        if (wasPlaying)
        {
            Pause?.Execute(null);
        }
    }

    private void dragCompleted(object sender, EventArgs e)
    {
        var position = slider.Position;
        Seek?.Execute(position);
        if (wasPlaying)
        {
            Play?.Execute(null);
        }
        slider.SetBinding(PositionSlider.PositionProperty, new Binding("Position"));

    }
}