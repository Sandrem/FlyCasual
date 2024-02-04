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

            UpgradeInfo = new UpgradeCardInfo
            (
                "Feedback Ping",
                UpgradeType.Talent,
                cost: 1,
                abilityType: typeof(Abilities.SecondEdition.FeedbackPingAbility),
                restrictions: new UpgradeCardRestrictions
                (
                    new TagRestriction(Content.Tags.Tie),
                    new ActionBarRestriction(typeof(ReloadAction))
                )
            );

            ImageUrl = "https://i.imgur.com/48SK1DJ.png";
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