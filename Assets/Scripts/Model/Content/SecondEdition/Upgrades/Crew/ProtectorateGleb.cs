using Upgrade;
using ActionsList;
using Actions;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class ProtectorateGleb : GenericUpgrade
    {
        public ProtectorateGleb() : base()
        {
            FromMod = typeof(Mods.ModsList.UnreleasedContentMod);

            UpgradeInfo = new UpgradeCardInfo(
                "Protectorate Gleb",
                UpgradeType.Crew,
                cost: 1,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Scum, Faction.Imperial, Faction.FirstOrder),
                addAction: new ActionInfo(typeof(CoordinateAction), ActionColor.Red),
                abilityType: typeof(Abilities.SecondEdition.ProtectorateGlebAbility)
            );

            ImageUrl = "";
        }        
    }
}

namespace Abilities.SecondEdition
{
    //After you coordinate, you may transfer 1 orange or red token to the ship you coordinated.
    public class ProtectorateGlebAbility : GenericAbility
    {
        public override void ActivateAbility()
        {

        }

        public override void DeactivateAbility()
        {
            
        }
    }
}