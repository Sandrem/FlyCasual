﻿using System.Collections.Generic;
using Upgrade;
using SubPhases;
using Ship;
using System.Linq;
using Abilities;
using Tokens;
using ActionsList;
using RuleSets;

namespace UpgradesList
{

    public class SquadLeader : GenericUpgrade, ISecondEditionUpgrade
    {

        public SquadLeader() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Squad Leader";
            isUnique = true;
            Cost = 2;

            UpgradeAbilities.Add(new SquadLeaderAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 4;
            UpgradeAbilities.RemoveAll(a => a is SquadLeaderAbility);
            UpgradeAbilities.Add(new Abilities.SecondEdition.SquadLeaderAbility());

            SEImageNumber = 16;
        }
    }
}

namespace Abilities
{
    public class SquadLeaderAbility: GenericAbility
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
            ActionsList.GenericAction newAction = new ActionsList.SquadLeaderAction()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = HostShip,
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
            SelectSquadLeaderTargetSubPhase newPhase = (SelectSquadLeaderTargetSubPhase) Phases.StartTemporarySubPhaseNew(
                "Select target for Squad Leader",
                typeof(SelectSquadLeaderTargetSubPhase),
                Phases.CurrentSubPhase.CallBack
            );
            newPhase.SquadLeaderOwner = this.Host;
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
                SquadLeaderUpgrade.Name,
                "Choose a ship that has lower pilot skill than you. It may perform 1 free action.",
                SquadLeaderUpgrade.ImageUrl
            );
        }

        private bool FilterAbilityTargets(GenericShip ship)
        {
            BoardTools.DistanceInfo distanceInfo = new BoardTools.DistanceInfo(SquadLeaderOwner, ship);
            return (ship.PilotSkill < SquadLeaderOwner.PilotSkill) && (distanceInfo.Range <= 2) && (ship.Owner.PlayerNo == SquadLeaderOwner.Owner.PlayerNo) && (ship.ShipId != SquadLeaderOwner.ShipId);
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            int result = 0;

            result += NeedTokenPriority(ship);
            result += ship.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.Cost);

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
            List<ActionsList.GenericAction> actions = Selection.ThisShip.GetAvailableActions();

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

namespace Abilities.SecondEdition
{
    //While you coordinate, the ship you choose can perform an action only if that action is also on your action bar.
    public class SquadLeaderAbility : GenericAbility
    {
        public override void ActivateAbility(){}

        public override void ActivateAbilityForSquadBuilder()
        {
            HostShip.ActionBar.AddGrantedAction(new SquadLeaderCoordinateAction() { IsRed = true }, HostUpgrade);
        }

        public override void DeactivateAbility(){}

        public override void DeactivateAbilityForSquadBuilder()
        {
            HostShip.ActionBar.RemoveGrantedAction(typeof(SquadLeaderCoordinateAction), HostUpgrade);
        }

        public class SquadLeaderCoordinateAction : CoordinateAction
        {
            public override void ActionTake()
            {
                var phase = Phases.StartTemporarySubPhaseNew<SquadLeaderTargetSubPhase>(
                    "Select target for Coordinate",
                    Phases.CurrentSubPhase.CallBack
                );
                phase.HostShip = Host;
                phase.Start();
            }
        }

        public class SquadLeaderTargetSubPhase : CoordinateTargetSubPhase
        {
            public GenericShip HostShip;

            protected override List<GenericAction> GetPossibleActions()
            {
                var targetActions = Selection.ThisShip.GetAvailableActions();
                return targetActions.Where(a => HostShip.ActionBar.HasAction(a.GetType())).ToList();
            }
        }
    }
}
