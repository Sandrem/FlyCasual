using Upgrade;
using Tokens;
using System.Collections.Generic;
using RuleSets;
using Ship;
using Abilities.SecondEdition;
using ActionsList;

namespace UpgradesList
{
    public class HanSoloGunnerScum : GenericUpgrade, ISecondEditionUpgrade
    {
        public HanSoloGunnerScum() : base()
        {
            Types.Add(UpgradeType.Gunner);
            Name = "Han Solo";
            Cost = 4;
            isUnique = true;

            UpgradeRuleType = typeof(SecondEdition);

            UpgradeAbilities.Add(new HanSoloGunnerAbilityScum());

            SEImageNumber = 163;
        }

        public void AdaptUpgradeToSecondEdition()
        {
            //Nothing to do, already second edition upgrade
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Scum;
        }
    }
}

namespace Abilities.SecondEdition
{
    public class HanSoloGunnerAbilityScum : GenericAbility
    {
        // Before you engage, you may perform a red (Focus) action.

        public override void ActivateAbility()
        {
            HostShip.OnCombatActivation += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCombatActivation -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatActivation, UseAbility);
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            Messages.ShowInfo(HostUpgrade.Name + ": You can perform free Red Focus action");

            HostShip.AskPerformFreeAction(new FocusAction() { IsRed = true }, Triggers.FinishTrigger);
        }
    }
}