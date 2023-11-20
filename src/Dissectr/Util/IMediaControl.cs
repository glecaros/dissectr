using System;

namespace Dissectr.Util;

public interface IMediaControl
{
    event Action Play;
    event Action Pause;
    event Action<TimeSpan> Seek;
}
