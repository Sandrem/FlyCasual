﻿using ActionsList;
using Ship;
using SubPhases;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ModifiedYT1300LightFreighter
    {
        public class LandoCalrissian : ModifiedYT1300LightFreighter
        {
            public LandoCalrissian() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Lando Calrissian",
                    5,
                    79,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.LandoCalrissianRebelPilotAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 70
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LandoCalrissianRebelPilotAbility : Abilities.FirstEdition.LandoCalrissianRebelPilotAbility
    {

        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += CheckLandoCalrissianPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= CheckLandoCalrissianPilotAbility;
        }

        protected override string GetAbilityDescription()
        {
            return "Choose a ship.\nIt may perform an action.";
        }

        protected override void CheckLandoCalrissianPilotAbility(GenericShip ship)
        {
            if (BoardTools.Board.IsOffTheBoard(ship)) return;

            if (ship.AssignedManeuver.ColorComplexity == Movement.MovementComplexity.Easy)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, LandoCalrissianPilotAbility);
            }
        }

        protected override bool FilterAbilityTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.This, TargetTypes.OtherFriendly }) && FilterTargetsByRange(ship, 0, 3);
        }

        protected override void PerformFreeAction(object sender, System.EventArgs e)
        {
            List<GenericAction> actions = TargetShip.GetAvailableActions();

            TargetShip.AskPerformFreeAction(
                actions,
                delegate {
                    Selection.ThisShip = HostShip;
                    Phases.CurrentSubPhase.Resume();
                    Triggers.FinishTrigger();
                },
                HostShip.PilotInfo.PilotName,
                "You may perform an action",
                HostShip
            );
        }
    }
}