using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Malice : GenericUpgrade
    {
        public Malice() : base()
        {
            IsWIP = true;

            FromMod = typeof(Mods.ModsList.UnreleasedContentMod);

            UpgradeInfo = new UpgradeCardInfo
            (
                "Malice",
                UpgradeType.ForcePower,
                cost: 1,
                restriction: new ForceAlignmentRestriction(ForceAlignment.Dark),
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