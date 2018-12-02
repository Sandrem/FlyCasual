using Ship;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Intimidation : GenericUpgrade
    {
        public Intimidation() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Intimidation",
                UpgradeType.Talent,
                cost: 3,
                abilityType: typeof(Abilities.SecondEdition.IntimidationAbility),
                seImageNumber: 7
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class IntimidationAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.AfterGotNumberOfDefenceDiceGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.AfterGotNumberOfDefenceDiceGlobal -= CheckAbility;
        }

        private void CheckAbility(ref int count)
        {
            if (HostShip.ShipsBumped.Contains(Combat.Defender))
            {
                Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + ": Defender rolls 1 fewer defense die");
                count--;
            }
        }
    }
}