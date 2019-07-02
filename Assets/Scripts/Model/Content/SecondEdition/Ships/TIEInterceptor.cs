﻿using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Upgrade;
using Ship;
using Actions;

namespace Ship
{
    namespace SecondEdition.TIEInterceptor
    {
        public class TIEInterceptor : FirstEdition.TIEInterceptor.TIEInterceptor, TIE
        {
            public TIEInterceptor() : base()
            {
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Modification);

                ShipAbilities.Add(new Abilities.SecondEdition.AutoThrustersAbility());

                IconicPilots[Faction.Imperial] = typeof(SoontirFel);

                DialInfo.RemoveManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn));
                DialInfo.RemoveManeuver(new ManeuverHolder(ManeuverSpeed.Speed5, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn));
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn), MovementComplexity.Complex);
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.SegnorsLoop), MovementComplexity.Complex);
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.SegnorsLoop), MovementComplexity.Complex);

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/d/da/Maneuver_tie_interceptor.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you perform an action, you may perform a red boost / red barrel roll action.
    public class AutoThrustersAbility : GenericAbility
    {
        public override string Name { get { return "Autothrusters"; } }

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
            HostShip.OnActionDecisionSubphaseEnd += PerformRepositionAction;
        }

        private void PerformRepositionAction(GenericShip ship)
        {
            HostShip.OnActionDecisionSubphaseEnd -= PerformRepositionAction;

            RegisterAbilityTrigger(TriggerTypes.OnFreeAction, AskPerformPerositionAction);
        }

        private void AskPerformPerositionAction(object sender, System.EventArgs e)
        {
            HostShip.AskPerformFreeAction(
                new List<GenericAction>()
                {
                    new BoostAction() { Color = ActionColor.Red },
                    new BarrelRollAction() { Color = ActionColor.Red }
                },
                Triggers.FinishTrigger,
                descriptionShort: Name,
                descriptionLong: "After you perform an action, you may perform a red Barrel Roll or red Boost action"
            );
        }
    }
}
