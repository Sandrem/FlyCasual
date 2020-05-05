using ActionsList;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;

public class DiceModificationStep : IDiceRollStep
{
    public CombatStep CombatStep { get; private set; }
    public PlayerRole StepOwner { get; private set; }

    public Type SubphaseType { get; private set; }
    public bool IsExecuted { get; set; }

    public DiceModificationStep(CombatStep combatStep, PlayerRole stepOwner, Type subphaseType)
    {
        CombatStep = combatStep;
        StepOwner = stepOwner;
        SubphaseType = subphaseType;
    }

    public void Start()
    {
        Combat.DiceModifications.AvailableDiceModifications = new Dictionary<string, GenericAction>();

        IsExecuted = true;

        Selection.ActiveShip = (StepOwner == PlayerRole.Attacker) ? Selection.ThisShip : Selection.AnotherShip;

        GenericSubPhase subphase = Phases.StartTemporarySubPhaseNew(
            SubphaseType.Name,
            SubphaseType,
            Combat.DiceModifications.Next
        );
        subphase.RequiredPlayer = Selection.ActiveShip.Owner.PlayerNo;
        Phases.CurrentSubPhase.Start();

        ShowDiceModifications();
    }

    public void ShowDiceModifications()
    {
        Selection.ActiveShip.GenerateDiceModifications(DiceModificationTimingType.Normal);
        List<GenericAction> diceModifications = Selection.ActiveShip.GetDiceModificationsGenerated();

        Combat.DiceModifications.ShowDiceModificationButtons(diceModifications);
        Selection.ActiveShip.Owner.UseDiceModifications(DiceModificationTimingType.Normal);

        /*if (diceModifications.Count > 0)
        {

        }
        else
        {
            ReplaysManager.ExecuteWithDelay(Phases.CurrentSubPhase.Next);
        }*/
    }

    public void Finish()
    {
        Combat.Attacker.ClearAlreadyUsedDiceModifications();
        Combat.Defender.ClearAlreadyUsedDiceModifications();

        // For Heavy Laser Cannon
        if (CombatStep == CombatStep.Attack && StepOwner == PlayerRole.Attacker) Combat.Attacker.CallAfterAttackDiceModification();

        // DiceCompareHelper.currentDiceCompareHelper.Close();
        // MovementTemplates.ReturnRangeRuler();

        // For HotShot gunner
        // Defender.CallAfterModifyDefenseDiceStep(delegate { Phases.StartTemporarySubPhaseOld("Compare results", typeof(CompareResultsSubPhase)); });

        IsExecuted = true;

        Phases.FinishSubPhase(SubphaseType);
    }
}

namespace SubPhases
{
    public class DiceModificationSubphase : GenericSubPhase
    {
        public override List<GameCommandTypes> AllowedGameCommandTypes { get { return new List<GameCommandTypes>() { GameCommandTypes.DiceModification }; } }

        public override void Next()
        {
            Phases.CurrentSubPhase = Phases.CurrentSubPhase.PreviousSubPhase;
            CallBack();
        }

        public override bool ThisShipCanBeSelected(GenericShip ship, int mouseKeyIsPressed)
        {
            bool result = false;
            return result;
        }

        public override bool AnotherShipCanBeSelected(GenericShip anotherShip, int mouseKeyIsPressed)
        {
            bool result = false;
            return result;
        }
    }

    public class DiceModificationAttackByDefenderSubphase : DiceModificationSubphase { }
    public class DiceModificationAttackByAttackerSubphase : DiceModificationSubphase { }
    public class DiceModificationDefenseByAttackerSubphase : DiceModificationSubphase { }
    public class DiceModificationDefenseByDefenderSubphase : DiceModificationSubphase { }
    public class DiceModificationCompareResultsByDefenderSubphase : DiceModificationSubphase { }
    public class DiceModificationCompareResultsByAttackerSubphase : DiceModificationSubphase { }
}
