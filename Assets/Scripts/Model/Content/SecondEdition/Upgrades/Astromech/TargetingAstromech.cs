using Actions;
using ActionsList;
using BoardTools;
using System;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class TargetingAstromech : GenericUpgrade
    {
        public TargetingAstromech() : base()
        {
            IsHidden = true;

            UpgradeInfo = new UpgradeCardInfo
            (
                "Targeting Astromech",
                UpgradeType.Astromech,
                cost: 0,
                abilityType: typeof(Abilities.SecondEdition.TargetingAstromechAbility)
            );

            ImageUrl = "https://i.imgur.com/caQnNAX.jpg";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TargetingAstromechAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckAbility;
        }

        private void CheckAbility(GenericAction action)
        {
            if (action is TargetLockAction)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AskPerfromRedRotateArcAction);
            }
        }

        private void AskPerfromRedRotateArcAction(object sender, EventArgs e)
        {
            HostShip.AskPerformFreeAction
            (
                new RotateArcAction() { Color = ActionColor.Red },
                Triggers.FinishTrigger,
                descriptionShort: HostUpgrade.UpgradeInfo.Name,
                descriptionLong: "You may perform a red Rotate action"
            );
        }
    }
}