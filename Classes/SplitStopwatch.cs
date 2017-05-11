using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class LapStopwatch: Stopwatch { 

    private long _lapLastTick = 0;
    public LastLapStruct LastLap = new LastLapStruct(0);
    public enum TimeType: long
    {
        Tick = 1,
        Millisecond = TimeSpan.TicksPerMillisecond,
        Second = TimeSpan.TicksPerSecond
    }
    public struct LastLapStruct
    {
        public double Seconds
        {
            get
            {
                return Ticks / TimeSpan.TicksPerSecond;
            }
        }
        public double Milliseconds
        {
            get
            {
                return Ticks / TimeSpan.TicksPerMillisecond;
            }
        }
        public readonly long Ticks;
        public LastLapStruct(long Ticks)
        {
            this.Ticks = Ticks;
        }
    }
    
    /// <summary>
    /// Returns the time since the last Lap() call.
    /// </summary>
    /// <param name="timeType">The </param>
    /// <returns></returns>
    public LastLapStruct Lap()
    {
        LastLap = new LastLapStruct(ElapsedTicks - _lapLastTick);
        _lapLastTick = ElapsedTicks;
        return LastLap;
    }
    public new void Reset()
    {
        base.Reset();
        _lapLastTick = 0;
        LastLap = new LastLapStruct(0);
    }
    public new void Restart()
    {
        _lapLastTick = 0;
        LastLap = new LastLapStruct(0);
        base.Restart();
    }
}
