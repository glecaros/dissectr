namespace Dissectr.ViewModels;

internal partial class MainViewModel
{
    private TimeSpan interval;
    public TimeSpan Interval
    {
        get => interval;
        set
        {
            SetProperty(ref interval, value);
            if (value.TotalSeconds < 1)
            {
                SetProperty(ref intervalCount, null);
            }
            else
            {
                SetProperty(ref intervalCount, (uint)Math.Ceiling(Duration.TotalSeconds / value.TotalSeconds));
            }
        }
    }

    private uint? intervalCount;
    public uint? IntervalCount => intervalCount;

    private uint currentIntervalIndex;
    public uint CurrentIntervalIndex
    {
        get => currentIntervalIndex;
        set
        {
            if (IntervalCount is uint count)
            {
                if (value >= count)
                {
                    throw new IndexOutOfRangeException();
                }
                SetProperty(ref currentIntervalIndex, value);
                SetProperty(ref currentIntervalStart, TimeSpan.FromSeconds(value * Interval.TotalSeconds));
                SetProperty(
                    ref currentIntervalEnd,
                    TimeSpan.FromSeconds(Math.Min((value + 1) * Interval.TotalSeconds, Duration.TotalSeconds))
                );
            }
            else
            {
                value = 0;
                SetProperty(ref currentIntervalIndex, value);
                SetProperty(ref currentIntervalStart, TimeSpan.Zero);
                SetProperty(ref currentIntervalEnd, Duration);
            }
        }
    }

    private TimeSpan currentIntervalStart;
    public TimeSpan CurrentIntervalStart => currentIntervalStart;

    private TimeSpan currentIntervalEnd;
    public TimeSpan CurrentIntervalEnd => currentIntervalEnd;

}
