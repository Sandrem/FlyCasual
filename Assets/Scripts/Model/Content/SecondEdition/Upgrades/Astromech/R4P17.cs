using Upgrade;
using Ship;
using Movement;
using System;
using ActionsList;
using System.Collections.Generic;

namespace UpgradesList.SecondEdition
{
    public class R4P17 : GenericUpgrade
    {
        public R4P17() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "R4-P17",
                UpgradeType.Astromech,
                cost: 4,
                charges: 2,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.R4P17Ability),
                restriction: new FactionRestriction(Faction.Republic)
            );
            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/9c/08/9c089203-13b7-4639-8366-2498c9fe9982/swz32_r4-p17_astromech.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you fully execute a red maneuver, you may spend 1 charge to perform an action, even while stressed.
    public class R4P17Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= RegisterTrigger;
        }

        private void RegisterTrigger(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, UseAbility);
        }

        private void UseAbility(object sender, EventArgs e)
        {
            if (HostShip.AssignedManeuver.ColorComplexity == MovementComplexity.Complex && HostUpgrade.State.Charges > 0)
            {
                HostShip.BeforeActionIsPerformed += SpendCharge;

                HostShip.OnCheckCanPerformActionsWhileStressed += ConfirmThatIsPossible;
                HostShip.OnCanPerformActionWhileStressed += AlwaysAllow;

                List<GenericAction> actions = HostShip.GetAvailableActions();

                HostShip.AskPerformFreeAction(
                    actions, 
                    delegate
                    {
                        HostShip.OnCheckCanPerformActionsWhileStressed -= ConfirmThatIsPossible;
                        HostShip.OnCanPerformActionWhileStressed -= AlwaysAllow;

                        HostShip.BeforeActionIsPerformed -= SpendCharge;
                        Triggers.FinishTrigger();
                    },
                    HostUpgrade.UpgradeInfo.Name,
                    "After you fully execute a red maneuver, you may spend 1 Charge to perform an action, even while stressed",
                    HostUpgrade
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void SpendCharge(GenericAction action, ref bool isFreeAction)
        {
            HostUpgrade.State.SpendCharge();
            HostShip.BeforeActionIsPerformed -= SpendCharge;
        }

        private void ConfirmThatIsPossible(ref bool isAllowed)
        {
            AlwaysAllow(null, ref isAllowed);
        }

        private void AlwaysAllow(GenericAction action, ref bool isAllowed)
        {
            isAllowed = true;
        }
    }
}