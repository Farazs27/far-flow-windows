namespace FarFlow.Core;

public enum GestureAction { None, Start, Stop }

// Times use a monotonic clock; the hook callback only queues edges to the UI.
public sealed class TriggerGesture
{
    public const long HoldMilliseconds = 220;
    public const long DoubleTapMilliseconds = 350;
    public bool Active { get; private set; }
    public bool HandsFree { get; private set; }
    private bool down;
    private long pressedAt;
    private long? releasedAt;
    public GestureAction Press(long now)
    {
        if (down) return GestureAction.None;
        down = true;
        if (HandsFree) { Reset(); down = true; return GestureAction.Stop; }
        if (Active && releasedAt is long release && now - release <= DoubleTapMilliseconds)
        { HandsFree = true; releasedAt = null; return GestureAction.None; }
        if (Active) return GestureAction.None;
        Active = true; pressedAt = now; releasedAt = null;
        return GestureAction.Start;
    }
    public GestureAction Release(long now)
    {
        if (!down) return GestureAction.None;
        down = false;
        if (!Active || HandsFree) return GestureAction.None;
        if (now - pressedAt >= HoldMilliseconds) { Reset(); return GestureAction.Stop; }
        releasedAt = now;
        return GestureAction.None;
    }
    public GestureAction Tick(long now)
    {
        if (Active && !HandsFree && releasedAt is long release && now - release > DoubleTapMilliseconds)
        { Reset(); return GestureAction.Stop; }
        return GestureAction.None;
    }
    public void Reset() { Active = HandsFree = down = false; releasedAt = null; }
}
