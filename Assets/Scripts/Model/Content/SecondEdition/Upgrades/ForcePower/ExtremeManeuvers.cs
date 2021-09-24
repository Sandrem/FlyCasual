using Upgrade;
using System.Collections.Generic;
using ActionsList;
using Ship;
using System.Linq;
using Actions;

namespace UpgradesList.SecondEdition
{
    public class ExtremeManeuvers : GenericUpgrade
    {
        public ExtremeManeuvers() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Extreme Maneuvers",
                UpgradeType.ForcePower,
                cost: 5,
                abilityType: typeof(Abilities.SecondEdition.ExtremeManeuversAbility),
                restrictions: new UpgradeCardRestrictions(
                    new BaseSizeRestriction(BaseSize.Small),
                    new ActionBarRestriction(typeof(BoostAction))
                )
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/5f/dd/5fdda9e7-dea9-4188-aaf1-e06446b38e45/swz79_extreme-maneuvers.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ExtremeManeuversAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGetAvailableBoostTemplates += ChangeBoostTemplates;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetAvailableBoostTemplates -= ChangeBoostTemplates;
        }

        private void ChangeBoostTemplates(List<BoostMove> availableMoves, GenericAction action)
        {
            if (HostShip.IsCanUseForceNow())
            {
                availableMoves.Add(new BoostMove(ActionsHolder.BoostTemplates.LeftTurn1, isPurple: true));
                availableMoves.Add(new BoostMove(ActionsHolder.BoostTemplates.RightTurn1, isPurple: true));
            }
        }
    }
}