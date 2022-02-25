using Content;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Compassion : GenericUpgrade
    {
        public Compassion() : base()
        {
            IsWIP = true;
            UpgradeInfo = new UpgradeCardInfo
            (
                "Compassion",
                UpgradeType.ForcePower,
                cost: 1,
                restriction: new TagRestriction(Tags.LightSide),
                abilityType: typeof(Abilities.SecondEdition.CompassionAbility)       
            );

            ImageUrl = "https://i.imgur.com/pKEXMaB.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CompassionAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}