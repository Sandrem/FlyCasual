using Content;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Malice : GenericUpgrade
    {
        public Malice() : base()
        {
            IsWIP = true;

            UpgradeInfo = new UpgradeCardInfo
            (
                "Malice",
                UpgradeType.ForcePower,
                cost: 4,
                restriction: new TagRestriction(Tags.DarkSide),
                abilityType: typeof(Abilities.SecondEdition.MaliceAbility)       
            );

            ImageUrl = "https://i.imgur.com/mRyM1s0.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MaliceAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}