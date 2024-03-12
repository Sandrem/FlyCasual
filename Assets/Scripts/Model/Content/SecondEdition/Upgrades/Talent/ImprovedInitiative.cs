using Upgrade;
using System.Collections.Generic;
using Ship;
using System.Linq;

namespace UpgradesList.SecondEdition
{
    public class ImprovedInitiative : GenericUpgrade
    {
        public ImprovedInitiative() : base()
        {
            IsHidden = !DebugManager.FreeMode;

            UpgradeInfo = new UpgradeCardInfo(
                "Improved Initiative",
                UpgradeType.Talent,
                cost: 0,
                abilityType: typeof(Abilities.SecondEdition.ImprovedInitiativeAbility)
            );

            ImageUrl = "https://i.imgur.com/nvHEwLO.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ImprovedInitiativeAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void ActivateAbilityForSquadBuilder()
        {
            HostShip.PilotInfo.Initiative++;
        }

        public override void DeactivateAbility()
        {
            
        }

        public override void DeactivateAbilityForSquadBuilder()
        {
            HostShip.PilotInfo.Initiative--;
        }
    }
}