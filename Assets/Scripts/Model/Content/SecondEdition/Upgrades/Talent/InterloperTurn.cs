using Upgrade;
using System.Collections.Generic;
using Ship;
using BoardTools;

namespace UpgradesList.SecondEdition
{
    public class InterloperTurn : GenericUpgrade
    {
        public InterloperTurn() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Interloper Turn",
                UpgradeType.Talent,
                cost: 1,
                abilityType: typeof(Abilities.SecondEdition.InterloperTurnAbility),
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.TIEDDefender.TIEDDefender))
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/12/25/1225b83d-2bdb-45f1-8301-2150bdc5ec26/swz84_upgrade_interloperturn.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class InterloperTurnAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}