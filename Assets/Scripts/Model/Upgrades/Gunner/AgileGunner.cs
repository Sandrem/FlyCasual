using Upgrade;
using Tokens;
using System.Collections.Generic;
using RuleSets;
using Ship;
using Abilities.SecondEdition;
using ActionsList;

namespace UpgradesList
{
    public class AgileGunner : GenericUpgrade, ISecondEditionUpgrade
    {
        public AgileGunner() : base()
        {
            Types.Add(UpgradeType.Gunner);
            Name = "Agile Gunner";
            Cost = 10;

            UpgradeRuleType = typeof(SecondEdition);

            UpgradeAbilities.Add(new AgileGunnerAbility());

            SEImageNumber = 162;
        }

        public void AdaptUpgradeToSecondEdition()
        {
            //Nothing to do, already second edition upgrade
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AgileGunnerAbility : GenericAbility
    {
        // During the End Phase, you may rotate your turret indicator.

        public override void ActivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers -= RegisterTrigger;
        }

        private void RegisterTrigger()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatActivation, UseAbility);
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);

            Messages.ShowInfo(HostUpgrade.Name + ": You can rotate your arc");
            HostShip.AskPerformFreeAction(new RotateArcAction() { IsRed = false, CanBePerformedWhileStressed = true }, Triggers.FinishTrigger);
        }
    }
}