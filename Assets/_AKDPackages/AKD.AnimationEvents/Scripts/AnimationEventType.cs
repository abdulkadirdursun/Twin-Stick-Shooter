namespace AKD.AnimationEvents
{
    /// <summary>Lifecycle moments an animation-event registration can target.</summary>
    public enum AnimationEventType
    {
        /// <summary>State entered.</summary>
        OnStart,

        /// <summary>Normalized time crossed a registered threshold (0..1]. A threshold of exactly 0 never fires — use OnStart.</summary>
        OnTime,

        /// <summary>
        /// Clip reached its end naturally: once per loop cycle for looping states, once for one-shots.
        /// Never fires when the state is interrupted.
        /// States that leave via a Has-Exit-Time transition with Exit Time below 1 never reach completion — use OnExit for those, or set Exit Time to 1.
        /// </summary>
        OnComplete,

        /// <summary>State exited for any reason — natural finish or interruption.</summary>
        OnExit
    }
}
