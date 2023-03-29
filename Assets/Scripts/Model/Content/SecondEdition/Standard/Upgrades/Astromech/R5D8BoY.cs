using Upgrade;
using System.Collections.Generic;
using Content;

namespace UpgradesList.SecondEdition
{
    public class R5D8BoY : GenericUpgrade
    {
        public R5D8BoY() : base()
        {
            IsHidden = true;

            UpgradeInfo = new UpgradeCardInfo
            (
                "R5-D8",
                UpgradeType.Astromech,
                cost: 0,
                abilityType: typeof(Abilities.SecondEdition.R5AstromechAbility),
                charges: 2,
                legalityInfo: new List<Legality>
                {
                    Legality.StandardBanned,
                    Legality.ExtendedLegal
                }
            );

            ImageUrl = "https://i.imgur.com/6hwr0eL.jpg";
        }
    }
}