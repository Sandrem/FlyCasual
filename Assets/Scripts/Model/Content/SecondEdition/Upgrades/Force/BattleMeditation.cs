using Actions;
using ActionsList;
using BoardTools;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class BattleMeditation : GenericUpgrade
    {
        public BattleMeditation() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Battle Meditation",
                UpgradeType.Force,
                cost: 0,
                isLimited: false,
                restriction: new FactionRestriction(Faction.Republic),
                addAction: new ActionInfo(typeof(BattleMeditationCoordinateAction), ActionColor.Purple),
                abilityType: typeof(Abilities.SecondEdition.BattleMeditationAbility)
                // seImageNumber: ??
            );

            FromMod = typeof(Mods.ModsList.UnreleasedContentMod);
            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/e3/0b/e30ba082-91c7-408b-9738-d631079911c7/swz32_battle-meditation.png";
        }        
    }
}


namespace Abilities.SecondEdition
{
    // You cannot coordinate limited ships. While you perform a purple coordinate action, you may coordinate
    // 1 additional friendly non-limited ship of the same type. Both ships must perform the same action.
    public class BattleMeditationAbility : GenericAbility
    {
        GenericShip battleMeditationShip1 = null;
        bool secondCoordPerformed = false;

        public override void ActivateAbility()
        {
            HostShip.OnCoordinateTargetIsSelected += NoteBattleMeditationTarget;
            HostShip.OnActionIsPerformed += PerformSecondBattleMeditationCoordinate;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCoordinateTargetIsSelected -= NoteBattleMeditationTarget;
            HostShip.OnActionIsPerformed -= PerformSecondBattleMeditationCoordinate;
        }

        private void NoteBattleMeditationTarget(GenericShip ship)
        {
            battleMeditationShip1 = ship;
        }

        private void PerformSecondBattleMeditationCoordinate(GenericAction action)
        {
            if (action is BattleMeditationCoordinateAction && battleMeditationShip1 != null && !secondCoordPerformed)
            {
                HostShip.OnActionDecisionSubphaseEnd += BattleMeditationEffect;
            }
        }

        private void BattleMeditationEffect(GenericShip ship)
        {
            HostShip.OnActionDecisionSubphaseEnd -= BattleMeditationEffect;
            
            RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, PerformAction);
        }
        private void PerformAction(object sender, System.EventArgs e)
        {

            Messages.ShowInfoToHuman("Battle Meditation: Select a second non-limited ship to coordinate.");
            secondCoordPerformed = true;
            HostShip.RemoveAlreadyExecutedAction(new BattleMeditationCoordinateAction());
            BattleMeditationCoordinateAction battleMedCoordinate = new BattleMeditationCoordinateAction();
            battleMedCoordinate.setPreviouslyCoordinatedShip(battleMeditationShip1);
            HostShip.AskPerformFreeAction(battleMedCoordinate, CleanUp, false);
        }

        private void CleanUp()
        {
            Triggers.FinishTrigger();
            battleMeditationShip1 = null;
            secondCoordPerformed = false;
        }
    }
}

namespace ActionsList
{
    public class BattleMeditationCoordinateAction : CoordinateAction
    {
        GenericShip alreadyCoordinated = null;

        public override void ActionTake()
        {
            BattleMeditationCoordinateTargetSubPhase subphase = Phases.StartTemporarySubPhaseNew<BattleMeditationCoordinateTargetSubPhase>(
                "Select target for Battle Meditation",
                Phases.CurrentSubPhase.CallBack
            );
            subphase.HostAction = this;
            if (alreadyCoordinated != null)
            {
                subphase.alreadyCoordinated = alreadyCoordinated;
            }
            subphase.Start();
        }

        public void setPreviouslyCoordinatedShip(GenericShip ship) 
        {
            alreadyCoordinated = ship;
        }
    }
}

namespace SubPhases
{
    public class BattleMeditationCoordinateTargetSubPhase : CoordinateTargetSubPhase
    {
        public GenericShip alreadyCoordinated = null;

        public override void Prepare()
        {
            PrepareByParameters(
                SelectCoordinateTarget,
                FilterBattleMeditationTargets,
                GetAiCoordinatePriority,
                Selection.ThisShip.Owner.PlayerNo,
                false,
                "Coordinate Action",
                "Select another ship.\nIt performs free action."
            );
        }

        private bool FilterBattleMeditationTargets(GenericShip ship)
        {
            return ship.Owner.PlayerNo == Selection.ThisShip.Owner.PlayerNo
                && Board.CheckInRange(Selection.ThisShip, ship, 1, 2, RangeCheckReason.CoordinateAction)
                && !ship.PilotInfo.IsLimited
                && ship != alreadyCoordinated;
        }
    }
}