using ActionsList;
using Ship;
using System;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class BackwardsTailslide : GenericUpgrade
    {
        public BackwardsTailslide() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Backwards Tailslide",
                UpgradeType.Talent,
                cost: 2,
                restrictions: new UpgradeCardRestrictions(
                    new TagRestriction(Content.Tags.XWing),
                    new UpgradePresentRestriction(UpgradeType.Configuration)
                ),
                abilityType: typeof(Abilities.SecondEdition.BackwardsTailslideAbility)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/c4/d5/c4d543bc-bcf0-4c88-b8df-f652210752b9/swz68_backward-tailslide.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class BackwardsTailslideAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCheckIgnoreObstaclesDuringBarrelRoll += AllowIfSFoilsAreClosed;
            HostShip.OnCheckIgnoreObstaclesDuringBoost += AllowIfSFoilsAreClosed;

            HostShip.OnActionIsPerformed += CheckRepositionBonus;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckIgnoreObstaclesDuringBarrelRoll -= AllowIfSFoilsAreClosed;
            HostShip.OnCheckIgnoreObstaclesDuringBoost -= AllowIfSFoilsAreClosed;

            HostShip.OnActionIsPerformed -= CheckRepositionBonus;
        }

        private void CheckRepositionBonus(GenericAction action)
        {
            if ((action is BarrelRollAction && (action as BarrelRollAction).IsThroughObstacle)
                || (action is BoostAction && (action as BoostAction).IsThroughObstacle))
            {
                if (!HostShip.IsLandedOnObstacle)
                {
                    RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, ApplyRepositionBonus);
                }
            }
        }

        private void ApplyRepositionBonus(object sender, EventArgs e)
        {
            Messages.ShowInfo($"{HostUpgrade.UpgradeInfo.Name}: 1 Evade token is gained");

            HostShip.Tokens.AssignToken(
                typeof(Tokens.EvadeToken),
                Triggers.FinishTrigger
            );
        }

        private void AllowIfSFoilsAreClosed(ref bool isAllowed)
        {
            GenericUpgrade sfoils = HostShip.UpgradeBar.GetInstalledUpgrade(UpgradeType.Configuration);
            if (sfoils != null && sfoils.UpgradeInfo.Name.Contains("(Closed)"))
            {
                isAllowed = true;
            }
        }
    }
}