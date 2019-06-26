using Ship;
using Upgrade;
using System.Linq;
using System.Collections.Generic;
using Tokens;
using BoardTools;
using ActionsList;

namespace UpgradesList.SecondEdition
{
    public class GeneralHux : GenericUpgrade
    {
        public GeneralHux() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "General Hux",
                UpgradeType.Crew,
                cost: 10,
                isLimited: true,
                restrictions: new UpgradeCardRestrictions(
                    new FactionRestriction(Faction.FirstOrder),
                    new ActionBarRestriction(typeof(CoordinateAction))
                ),
                abilityType: typeof(Abilities.SecondEdition.GeneralHuxAbility)
            );

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/fa0b8492eff625bc66f00bd561015465.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class GeneralHuxAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}