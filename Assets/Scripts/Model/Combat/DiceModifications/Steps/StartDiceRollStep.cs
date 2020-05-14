using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class StartDiceRollStep : IDiceRollStep
{
    public bool IsExecuted { get; set; }

    public Type SubphaseType { get; private set; }

    public StartDiceRollStep(Type subphaseType)
    {
        SubphaseType = subphaseType;
    }

    public void Start()
    {
        IsExecuted = true;

        Combat.DiceModifications.ShowDiceModificationsUiEmpty();

        GenericSubPhase subphase = Phases.StartTemporarySubPhaseNew(
            SubphaseType.Name,
            SubphaseType,
            Combat.DiceModifications.Next
        );

        subphase.Start();
    }

    public void WhenFinish() { }
}
