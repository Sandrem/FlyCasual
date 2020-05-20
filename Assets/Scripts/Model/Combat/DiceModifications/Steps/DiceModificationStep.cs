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
    public DiceModificationTimingType DiceModificationTiming { get; private set; }
    public bool IsVital { get; private set; }

    public Type SubphaseType { get; private set; }
    public bool IsExecuted { get; set; }

    public DiceModificationStep(CombatStep combatStep, PlayerRole stepOwner, DiceModificationTimingType timing, Type subphaseType)
    {
        CombatStep = combatStep;
        StepOwner = stepOwner;
        SubphaseType = subphaseType;
        DiceModificationTiming = timing;
        IsVital = IsVitalStep();
    }

    private bool IsVitalStep()
    {
        return DiceModificationTiming == DiceModificationTimingType.Normal && CombatStep != CombatStep.CompareResults;
    }

    public void Start()
    {
        IsExecuted = true;

        if (CombatStep == CombatStep.CompareResults) Combat.AttackStep = CombatStep.CompareResults;

        Combat.Attacker.ClearAlreadyUsedDiceModifications();
        Combat.Defender.ClearAlreadyUsedDiceModifications();

        Combat.DiceModifications.AvailableDiceModifications = new Dictionary<string, GenericAction>();

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
        Selection.ActiveShip.GenerateDiceModifications(DiceModificationTiming);
        List<GenericAction> diceModifications = Selection.ActiveShip.GetDiceModificationsGenerated();

        Combat.DiceModifications.ShowDiceModificationButtons(diceModifications);

        Roster.HighlightPlayer(Selection.ActiveShip.Owner.PlayerNo);
        Phases.CurrentSubPhase.IsReadyForCommands = true;

        if (diceModifications.Count > 0 || IsVital)
        {
            IsVital = true;
            Selection.ActiveShip.Owner.UseDiceModifications(DiceModificationTiming);
        }
        else
        {
            Combat.DiceModifications.HideAllButtons();
            ReplaysManager.ExecuteWithDelay(Combat.DiceModifications.ConfirmDiceResults);
        }
    }

    public void WhenFinish(Action callback)
    {
        // For Heavy Laser Cannon
        if (CombatStep == CombatStep.Attack && StepOwner == PlayerRole.Attacker && DiceModificationTiming == DiceModificationTimingType.Normal)
        {
            Combat.Attacker.CallAfterAttackDiceModification();
        }

        IsExecuted = true;

        // For Hotshot Gunner
        if (CombatStep == CombatStep.Defence && StepOwner == PlayerRole.Defender && DiceModificationTiming == DiceModificationTimingType.Normal)
        {
            Combat.Defender.CallAfterModifyDefenseDiceStep(callback);
        }
        else
        {
            callback();
        }
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
