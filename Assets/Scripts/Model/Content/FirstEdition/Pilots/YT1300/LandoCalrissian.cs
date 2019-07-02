﻿using ActionsList;
using Ship;
using SubPhases;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.YT1300
    {
        public class LandoCalrissian : YT1300
        {
            public LandoCalrissian() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Lando Calrissian",
                    7,
                    44,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.LandoCalrissianRebelPilotAbility),
                    extraUpgradeIcons: new List<UpgradeType>() { UpgradeType.Talent, UpgradeType.Missile }
                );

                ShipInfo.ArcInfo.Arcs.ForEach(a => a.Firepower = 3);
                ShipInfo.Hull = 8;
                ShipInfo.Shields = 5;
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class LandoCalrissianRebelPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementExecuted += CheckLandoCalrissianPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementExecuted -= CheckLandoCalrissianPilotAbility;
        }

        protected virtual void CheckLandoCalrissianPilotAbility(GenericShip ship)
        {
            if (BoardTools.Board.IsOffTheBoard(ship)) return;

            if (ship.AssignedManeuver.ColorComplexity == Movement.MovementComplexity.Easy)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementExecuted, LandoCalrissianPilotAbility);
            }
        }

        protected void LandoCalrissianPilotAbility(object sender, System.EventArgs e)
        {
            SelectTargetForAbility(
                GrantFreeAction,
                FilterAbilityTargets,
                GetAiAbilityPriority,
                HostShip.Owner.PlayerNo,
                HostShip.PilotInfo.PilotName,
                GetAbilityDescription(),
                HostShip
            );
        }

        protected virtual string GetAbilityDescription()
        {
            return "Choose another ship.\nIt may perform free action shown in its action bar.";
        }

        protected virtual bool FilterAbilityTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly }) && FilterTargetsByRange(ship, 1, 1);
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

        private void GrantFreeAction()
        {
            Selection.ThisShip = TargetShip;

            RegisterAbilityTrigger(TriggerTypes.OnFreeActionPlanned, PerformFreeAction);

            Triggers.ResolveTriggers(TriggerTypes.OnFreeActionPlanned, SelectShipSubPhase.FinishSelection);
        }

        protected virtual void PerformFreeAction(object sender, System.EventArgs e)
        {
            List<GenericAction> actions = TargetShip.GetAvailableActions();
            List<GenericAction> actionBarActions = actions.Where(n => n.IsInActionBar).ToList();

            TargetShip.AskPerformFreeAction(
                actionBarActions,
                delegate {
                    Selection.ThisShip = HostShip;
                    Phases.CurrentSubPhase.Resume();
                    Triggers.FinishTrigger();
                },
                HostShip.PilotInfo.PilotName,
                "You may perform 1 free action shown in your action bar",
                HostShip
            );
        }
    }
}