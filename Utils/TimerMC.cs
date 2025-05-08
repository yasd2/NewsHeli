/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsHeli;

internal class TimerMC
{
    public enum EState
    {
        None,
        Running,
        Finished,
    }

    public bool HasStarted { get; set; }

    public System.Timers.Timer Timer { get; set; }
    public int TimeInSeconds { get; set; }

    public Action Action { get; set; }

    public TimerMC(int timeInSeconds, Action action = null)
    {
        TimeInSeconds = timeInSeconds;
        if (action != null) Action = action;
    }

    public void Start()
    {
        Timer = new System.Timers.Timer(TimeInSeconds * 1000);
        Timer.Start();

        HasStarted = true;

        Timer.Elapsed += (sender, e) =>
        {
            Action?.Invoke();
        };
    }

    public void Stop()
    {
        if (HasStarted) Timer.Stop();
    }   
}
*/