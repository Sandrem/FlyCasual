using System;
using System.Collections.Generic;
using System.Linq;

public class NetworkTask
{
    public string Name { get; private set; }
    public Action Callback { get; private set; }
    public int FinishesCount { get; private set; }

    public static NetworkTask CurrentTask { get; private set; }

    public NetworkTask(string name, Action callback)
    {
        Name = name;
        Callback = callback;

        CurrentTask = this;
    }

    public void FinishOne()
    {
        FinishesCount++;

        if (FinishesCount == 2)
        {
            FinishTask();
        }
    }

    private void FinishTask()
    {
        CurrentTask = null;
        Callback();
    }
}
