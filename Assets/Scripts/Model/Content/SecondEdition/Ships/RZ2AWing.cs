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
                ModelInfo = new ShipModelInfo("RZ-2 A-wing", "Blue");

                ShipInfo.ArcInfo = new ShipArcsInfo(ArcType.SingleTurret, 2);

                ShipInfo.UpgradeIcons.Upgrades.Remove(UpgradeType.Modification);
                ShipInfo.UpgradeIcons.Upgrades.Remove(UpgradeType.Modification);
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Tech);

                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(BarrelRollAction)));

                ShipInfo.DefaultShipFaction = Faction.Resistance;
                ShipInfo.FactionsAll = new List<Faction>() { Faction.Resistance };

                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank), MovementComplexity.Easy);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank), MovementComplexity.Easy);
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.SegnorsLoop), MovementComplexity.Complex);
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.SegnorsLoop), MovementComplexity.Complex);
                DialInfo.RemoveManeuver(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn));

                ShipAbilities.Add(new VectoredThrustersRZ2());

                IconicPilots[Faction.Resistance] = typeof(GreerSonnel);

                // ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/b/b4/Maneuver_a-wing.png";
                HotacManeuverTable = new AI.RZ2AWingTable();
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
            HostShip.OnActionIsPerformed += CheckConditions;
            HostShip.OnGetAvailableArcFacings += RestrictArcFacings;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckConditions;
            HostShip.OnGetAvailableArcFacings -= RestrictArcFacings;
        }

        private void RestrictArcFacings(List<ArcFacing> facings)
        {
            facings.Remove(ArcFacing.Left);
            facings.Remove(ArcFacing.Right);
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