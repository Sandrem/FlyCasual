using Upgrade;
using System.Collections.Generic;
using System.Linq;

namespace UpgradesList.SecondEdition
{
    public class TrickShot : GenericUpgrade
    {
        public TrickShot() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Trick Shot",
                UpgradeType.Talent,
                cost: 4,
                abilityType: typeof(Abilities.SecondEdition.TrickShotAbility),
                seImageNumber: 18
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class TrickShotAbility : Abilities.FirstEdition.TrickShotAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotStartAsAttacker += CheckAbilityAndAddDice;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotStartAsAttacker -= CheckAbilityAndAddDice;
        }

        private void CheckAbilityAndAddDice()
        {
            if (Combat.ShotInfo.IsObstructedByObstacle)
            {
                HostShip.AfterGotNumberOfAttackDice += RollExtraDie;
            }
        }
    }
}