﻿using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Upgrade;
using Actions;
using Ship;
using Abilities.SecondEdition;

namespace Ship
{
    namespace SecondEdition.RZ1AWing
    {
        public class RZ1AWing : FirstEdition.AWing.AWing
        {
            public RZ1AWing() : base()
            {
                ShipInfo.ShipName = "RZ-1 A-wing";
                ShipInfo.UpgradeIcons.Upgrades.Remove(UpgradeType.Modification);
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(BarrelRollAction)));

                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.SegnorsLoop), MovementComplexity.Complex);
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.SegnorsLoop), MovementComplexity.Complex);
                DialInfo.RemoveManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn));

                ShipAbilities.Add(new VectoredThrusters());

                IconicPilots[Faction.Rebel] = typeof(JakeFarrell);

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/b/b4/Maneuver_a-wing.png";

                OldShipTypeName = "A-wing";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you perform an action, you may perform a red boost action.
    public class VectoredThrusters : GenericAbility
    {
        public override string Name { get { return "Vectored Thrusters"; } }

        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckConditions;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckConditions;
        }

        private void CheckConditions(GenericAction action)
        {
            if (!(action is BoostAction))
            {
                HostShip.OnActionDecisionSubphaseEnd += PerformBoostAction;
            }
        }

        private void PerformBoostAction(GenericShip ship)
        {
            HostShip.OnActionDecisionSubphaseEnd -= PerformBoostAction;

            RegisterAbilityTrigger(TriggerTypes.OnFreeAction, AskPerformBoostAction);
        }

        private void AskPerformBoostAction(object sender, System.EventArgs e)
        {
            HostShip.AskPerformFreeAction(
                new BoostAction() { Color = ActionColor.Red },
                Triggers.FinishTrigger,
                descriptionShort: Name,
                descriptionLong: "After you perform an action, you may perform a red Boost action"
            );
        }
    }
}