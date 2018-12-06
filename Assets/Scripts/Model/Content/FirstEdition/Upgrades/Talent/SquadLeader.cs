using Upgrade;
using System.Collections.Generic;
using Ship;
using System.Linq;
using SubPhases;
using Tokens;
using ActionsList;

namespace UpgradesList.FirstEdition
{
    public class SquadLeader : GenericUpgrade
    {
        public SquadLeader() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Squad Leader",
                UpgradeType.Talent,
                cost: 2,
                isLimited: true,
                abilityType: typeof(Abilities.FirstEdition.SquadLeaderAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class SquadLeaderAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateActions += SquadLeaderAddAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateActions -= SquadLeaderAddAction;
        }

        private void SquadLeaderAddAction(GenericShip host)
        {
            GenericAction newAction = new SquadLeaderAction()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                HostShip = HostShip,
                Source = HostUpgrade
            };
            host.AddAvailableAction(newAction);
        }
    }
}

namespace ActionsList
{

    public class SquadLeaderAction : GenericAction
    {

        public SquadLeaderAction()
        {
            Name = DiceModificationName = "Squad Leader";
        }

        public override void ActionTake()
        {
            SelectSquadLeaderTargetSubPhase newPhase = (SelectSquadLeaderTargetSubPhase)Phases.StartTemporarySubPhaseNew(
                "Select target for Squad Leader",
                typeof(SelectSquadLeaderTargetSubPhase),
                Phases.CurrentSubPhase.CallBack
            );
            newPhase.SquadLeaderOwner = this.HostShip;
            newPhase.SquadLeaderUpgrade = this.Source;
            newPhase.Start();
        }

    }

}

namespace SubPhases
{

    public class SelectSquadLeaderTargetSubPhase : SelectShipSubPhase
    {
        public GenericShip SquadLeaderOwner;
        public GenericUpgrade SquadLeaderUpgrade;

        public override void Prepare()
        {
            PrepareByParameters(
                SelectSquadLeaderTarget,
                FilterAbilityTargets,
                GetAiAbilityPriority,
                Selection.ThisShip.Owner.PlayerNo,
                true,
                SquadLeaderUpgrade.UpgradeInfo.Name,
                "Choose a ship that has lower pilot skill than you. It may perform 1 free action.",
                SquadLeaderUpgrade
            );
        }

        private bool FilterAbilityTargets(GenericShip ship)
        {
            BoardTools.DistanceInfo distanceInfo = new BoardTools.DistanceInfo(SquadLeaderOwner, ship);
            return (ship.State.Initiative < SquadLeaderOwner.State.Initiative) && (distanceInfo.Range <= 2) && (ship.Owner.PlayerNo == SquadLeaderOwner.Owner.PlayerNo) && (ship.ShipId != SquadLeaderOwner.ShipId);
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            int result = 0;

            result += NeedTokenPriority(ship);
            result += ship.PilotInfo.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.UpgradeInfo.Cost);

            return result;
        }

        private int NeedTokenPriority(GenericShip ship)
        {
            if (!ship.Tokens.HasToken(typeof(FocusToken))) return 100;
            if (ship.ActionBar.HasAction(typeof(EvadeAction)) && !ship.Tokens.HasToken(typeof(EvadeToken))) return 50;
            if (ship.ActionBar.HasAction(typeof(TargetLockAction)) && !ship.Tokens.HasToken(typeof(BlueTargetLockToken), '*')) return 50;
            return 0;
        }

        private void SelectSquadLeaderTarget()
        {
            var squadLeaderShip = Selection.ThisShip;
            Selection.ThisShip = TargetShip;

            Triggers.RegisterTrigger(
                new Trigger()
                {
                    Name = "Squad Leader: Free action",
                    TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnFreeActionPlanned,
                    EventHandler = PerformFreeAction
                }
            );

            MovementTemplates.ReturnRangeRuler();

            Triggers.ResolveTriggers(TriggerTypes.OnFreeActionPlanned, delegate {
                Selection.ThisShip = squadLeaderShip;
                Phases.FinishSubPhase(typeof(SelectSquadLeaderTargetSubPhase));
                CallBack();
            });
        }

        public override void RevertSubPhase() { }

        private void PerformFreeAction(object sender, System.EventArgs e)
        {
            List<GenericAction> actions = Selection.ThisShip.GetAvailableActions();

            TargetShip.AskPerformFreeAction(actions, Triggers.FinishTrigger);
        }

        public override void SkipButton()
        {
            Phases.FinishSubPhase(this.GetType());
            Phases.CurrentSubPhase.Resume();
            CallBack();
        }

    }

}