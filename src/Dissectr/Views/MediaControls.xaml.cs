using CommunityToolkit.Maui.Core.Primitives;
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
        typeof(MediaControls),
        propertyChanged: OnPositionChanged);

    static void OnPositionChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is MediaControls mediaControls)
        {
            mediaControls.AttachPosition();
        }
    }

    public TimeSpan Position
    {
        get => (TimeSpan)GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
    }
    #endregion

    #region PlaybackState Property
    public static readonly BindableProperty PlaybackStateProperty = BindableProperty.Create(
        nameof(PlaybackState),
        typeof(MediaElementState),
        typeof(MediaControls));

    public MediaElementState PlaybackState
    {
        get => (MediaElementState)GetValue(PlaybackStateProperty);
        set => SetValue(PlaybackStateProperty, value);
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

    private void togglePlayClicked(object sender, EventArgs e)
    {
        if (IsPlaying)
        {
            Pause?.Execute(null);
        }
        else
        {
            Play?.Execute(null);
        }
    }

    private void stopButtonClicked(object sender, EventArgs e)
    {

    }

    public bool IsPlaying => PlaybackState == MediaElementState.Playing;
    private bool wasPlaying = false;
    private bool isPositionBound = false;

    private void AttachPosition()
    {
        if (!isPositionBound)
        {
            slider.SetBinding(PositionSlider.PositionProperty, new Binding("Position"));
        }
    }

    private void DetachPosition()
    {
        if (isPositionBound)
        {
            slider.RemoveBinding(PositionProperty);
        }
    }

    private void dragStarted(object sender, EventArgs e)
    {
        DetachPosition();
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
    }
}
