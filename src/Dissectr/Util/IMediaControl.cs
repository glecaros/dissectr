namespace Dissectr.Util;

internal interface IMediaControl
{
    event Action Play;
    event Action Pause;
    event Action<TimeSpan> Seek;
}
