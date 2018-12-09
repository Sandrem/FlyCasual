using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Upgrade;
using Actions;
using Ship;
using Abilities.SecondEdition;
using System.Linq;
using Arcs;

namespace Ship
{
    namespace SecondEdition.RZ2AWing
    {
        public class RZ2AWing : FirstEdition.AWing.AWing
        {
            public RZ2AWing() : base()
            {
                ShipInfo.ShipName = "RZ-2 A-wing";

                ShipInfo.ArcInfo = new ShipArcsInfo(ArcType.SingleTurret, 2);

                ShipInfo.UpgradeIcons.Upgrades.Remove(UpgradeType.Modification);

                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(BarrelRollAction)));

                ShipInfo.DefaultShipFaction = Faction.Resistance;
                ShipInfo.FactionsAll = new List<Faction>() { Faction.Resistance };

                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.SegnorsLoop), MovementComplexity.Complex);
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.SegnorsLoop), MovementComplexity.Complex);
                DialInfo.RemoveManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn));

                ShipAbilities.Add(new VectoredThrustersRZ2());

                IconicPilots[Faction.Resistance] = typeof(TallissanLintra);

                // ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/b/b4/Maneuver_a-wing.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you perform an action, you may perform a red boost action.
    public class VectoredThrustersRZ2 : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += PerformVectoredThrusters;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= PerformVectoredThrusters;
        }

        private void PerformVectoredThrusters(GenericAction ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnFreeAction, AskPerformVectoredThrusters);
        }

        private void AskPerformVectoredThrusters(object sender, System.EventArgs e)
        {
            Messages.ShowInfoToHuman("Vectored Thrusters: you may perform a red boost or a red rotate arc action");

            HostShip.AskPerformFreeAction(
                new List<GenericAction>()
                    {
                        new BoostAction() { IsRed = true },
                        new RotateArcAction() { IsRed = true }
                    },
                Triggers.FinishTrigger
            );
        }
    }
}