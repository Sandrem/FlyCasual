using GameModes;
using Movement;
using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class SlaveI : GenericUpgrade
    {
        public SlaveI() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Slave I",
                UpgradeType.Title,
                cost: 2,
                isLimited: true,
                addSlot: new UpgradeSlot(UpgradeType.Torpedo),
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.FiresprayClassPatrolCraft.FiresprayClassPatrolCraft)),
                abilityType: typeof(Abilities.SecondEdition.SlaveIAbility),
                seImageNumber: 154
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    //After you reveal a turn or bank maneuver, you may set your dial to the maneuver of the same speed and bearing in the other direction.
    public class SlaveIAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsRevealed += RegisterAskChangeManeuver;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsRevealed -= RegisterAskChangeManeuver;
        }

        private void RegisterAskChangeManeuver(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, AskChangeManeuver);
        }

        private void AskChangeManeuver(object sender, System.EventArgs e)
        {
            if (HostShip.AssignedManeuver.Bearing == ManeuverBearing.Bank || HostShip.AssignedManeuver.Bearing == ManeuverBearing.Turn)
            {
                HostShip.Owner.ChangeManeuver(GameMode.CurrentGameMode.AssignManeuver, IsManeuverSameSpeedAndBearing);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private bool IsManeuverSameSpeedAndBearing(string maneuverString)
        {
            bool result = false;
            ManeuverHolder movementStruct = new ManeuverHolder(maneuverString);
            if ((movementStruct.Bearing == ManeuverBearing.Bank || movementStruct.Bearing == ManeuverBearing.Turn) 
                && movementStruct.Bearing == HostShip.AssignedManeuver.Bearing
                && movementStruct.Speed == HostShip.AssignedManeuver.ManeuverSpeed)
            {
                result = true;
            }
            return result;
        }
    }
}