using Upgrade;
using Ship;
using ActionsList;

namespace UpgradesList.SecondEdition
{
    public class FeedbackPing : GenericUpgrade
    {
        public FeedbackPing() : base()
        {
            IsWIP = true;

            FromMod = typeof(Mods.ModsList.UnreleasedContentMod);

            UpgradeInfo = new UpgradeCardInfo
            (
                "Feedback Ping",
                UpgradeType.Talent,
                cost: 1,
                abilityType: typeof(Abilities.SecondEdition.FeedbackPingAbility),
                restriction: new ActionBarRestriction(typeof(ReloadAction))
            );

            ImageUrl = "https://i.imgur.com/48SK1DJ.png";
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is TIE;
        }
    }
}

namespace Abilities.SecondEdition
{
    public class FeedbackPingAbility : GenericAbility
    {
        public override void ActivateAbility()
        {

        }

        public override void DeactivateAbility()
        {

        }
    }
}