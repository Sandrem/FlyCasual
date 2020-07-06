using System;
using System.Collections.Generic;

public interface IDiceRollStep
{
    bool IsExecuted { get; set; }

    void Start();
    void WhenFinish();
}
