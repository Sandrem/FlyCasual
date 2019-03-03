using BoardTools;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class DeadmansSwitch : GenericUpgrade
    {
        public DeadmansSwitch() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Deadman's Switch",
                UpgradeType.Illicit,
                cost: 2,
                abilityType: typeof(Abilities.SecondEdition.DeadmansSwitchAbility),
                seImageNumber: 59
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    //After you are destroyed, each other ship at range 0-1 suffers 1 damage.
    public class DeadmansSwitchAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShipIsDestroyed += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShipIsDestroyed -= RegisterAbility;
        }

        private void RegisterAbility(GenericShip ship, bool flag)
        {
            RegisterAbilityTrigger(TriggerTypes.OnShipIsDestroyed, DealDamage);
        }
        
        private void DealDamage(object sender, System.EventArgs e)
        {
            List<GenericShip> sufferedShips = new List<GenericShip>();

            foreach (var ship in Roster.AllShips.Values)
            {
                if (ship.ShipId == HostShip.ShipId) continue;

                DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
                if (distInfo.Range < 2) sufferedShips.Add(ship);
            }

            Messages.ShowInfo("Deadman's Switch damages " + sufferedShips.Count + " ships");
            DealDamageToShips(sufferedShips, 1, false, Triggers.FinishTrigger);
        }
    }
}