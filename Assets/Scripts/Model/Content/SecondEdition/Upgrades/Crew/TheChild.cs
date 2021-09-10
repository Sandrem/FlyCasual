using Upgrade;
using Ship;
using ActionsList;

namespace UpgradesList.SecondEdition
{
    public class TheChild : GenericUpgrade
    {
        public TheChild() : base()
        {
            IsWIP = true;

            FromMod = typeof(Mods.ModsList.UnreleasedContentMod);

            UpgradeInfo = new UpgradeCardInfo
            (
                "The Child",
                UpgradeType.Crew,
                cost: 5,
                abilityType: typeof(Abilities.SecondEdition.TheChildAbility),
                restriction: new FactionRestriction(Faction.Imperial, Faction.Rebel, Faction.Scum),
                addForce: 2
            );

            ImageUrl = "https://i.imgur.com/8pqkhJr.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TheChildAbility : GenericAbility
    {
        public override void ActivateAbility()
        {

        }

        public override void DeactivateAbility()
        {

        }
    }
}