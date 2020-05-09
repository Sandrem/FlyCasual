using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CompareAndDealDamageDiceStep : IDiceRollStep
{
    public bool IsExecuted { get; set; }

    public void Start()
    {
        IsExecuted = true;

        DiceCompareHelper.currentDiceCompareHelper.Close();
        Combat.DiceModifications.HideDiceModificationsUi();

        GenericSubPhase subphase = Phases.StartTemporarySubPhaseNew<CompareResultsSubPhase>(
            "Compare Results",
            Combat.DiceModifications.Next
        );

        Combat.CancelHitsByDefenceDice();
    }

    public void WhenFinish()
    {
        Phases.FinishSubPhase(typeof(CompareResultsSubPhase));
    }
}

namespace SubPhases
{
    public class CompareResultsSubPhase : DiceModificationSubphase { }
}
