using CommunityToolkit.Maui.Core.Primitives;
using System.Reflection.Metadata;
using System.Windows.Input;

namespace Dissectr.Views;

public class Interval
{
    private readonly TimeSpan _length;
    private readonly TimeSpan _max;

    public TimeSpan Start { get; }
    public TimeSpan End { get; }

    private Interval(TimeSpan start, TimeSpan length, TimeSpan max)
    {
        _length = length;
        _max = max;
        Start = start;
        End = Min(Start + length, max);
    }

    public Interval(TimeSpan length, TimeSpan max): this(TimeSpan.Zero, length, max)
    {
    }

    public Interval Next()
    {
        if (End == _max)
        {
            return this;
        }
        return new Interval(Start + _length, _length, _max);
    }

    public Interval Prev()
    {
        if (Start == TimeSpan.Zero)
        {
            return this;
        }
        return new Interval(Start - _length, _length, _max);
    }

    public Interval Seek(TimeSpan position)
    {
        if (position >= Start && position <= End)
        {
            return this;
        }
        var lengthTicks = _length.Ticks;
        var newStartTicks = (position.Ticks / lengthTicks) * lengthTicks;
        return new Interval(new TimeSpan(newStartTicks), _length, _max);
    }

    private static TimeSpan Min(TimeSpan a, TimeSpan b) => new TimeSpan(Math.Min(a.Ticks, b.Ticks));
}

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
            mediaControls.HandlePositionChanged();
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

    #region IntervalLenght Property
    public static readonly BindableProperty IntervalLengthProperty = BindableProperty.Create(
        nameof(IntervalLength),
        typeof(TimeSpan),
        typeof(MediaControls),
        propertyChanged: OnIntervalLengthChanged);

    static void OnIntervalLengthChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is MediaControls mediaControls)
        {
            mediaControls.HandleIntervalLengthChanged();
        }
    }

    public TimeSpan IntervalLength
    {
        get => (TimeSpan)GetValue(IntervalLengthProperty);
        set => SetValue(IntervalLengthProperty, value);
    }
    #endregion

    #region CurrentInterval Property

    private static readonly BindablePropertyKey CurrentIntervalPropertyKey = BindableProperty.CreateReadOnly(
        nameof(CurrentInterval),
        typeof(Interval),
        typeof(MediaControls),
        new Interval(TimeSpan.Zero, TimeSpan.Zero));

    public static readonly BindableProperty CurrentIntervalProperty = CurrentIntervalPropertyKey.BindableProperty;

    public Interval CurrentInterval
    {
        get => (Interval)GetValue(CurrentIntervalProperty);
        private set => SetValue(CurrentIntervalPropertyKey, value);
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
        if (sender is Button button)
        {
            if (button.Id == moveBackButton.Id)
            {
                CurrentInterval = CurrentInterval.Prev();
                Seek?.Execute(CurrentInterval.Start);
            }
            else if (button.Id == moveForwardButton.Id)
            {
                CurrentInterval = CurrentInterval.Next();
                Seek?.Execute(CurrentInterval.Start);
            }
        }
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

    public bool IsPlaying => PlaybackState == MediaElementState.Playing;
    private bool wasPlaying = false;
    private bool isAttached = false;
    private void AttachPosition()
    {
        if (!isAttached)
        {
            slider.SetBinding(PositionSlider.PositionProperty, new Binding("Position"));
            isAttached = true;
        }
    }

    private void DetachPosition()
    {
        if (isAttached)
        {
            slider.RemoveBinding(PositionProperty);
            isAttached = false;
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
        CurrentInterval = CurrentInterval.Seek(position);
        Seek?.Execute(position);
        if (wasPlaying)
        {
            Play?.Execute(null);
        }
    }

    private void HandleIntervalLengthChanged()
    {
        Seek?.Execute(TimeSpan.Zero);
        CurrentInterval = new Interval(IntervalLength, Duration);
    }

    private void HandlePositionChanged()
    {
        if (Position >= CurrentInterval.End)
        {
            Seek?.Execute(CurrentInterval.Start);
        }
        AttachPosition();
    }
}
