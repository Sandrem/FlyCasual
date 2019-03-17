using Ship;
using Upgrade;
using System.Linq;
using Tokens;
using ActionsList;

namespace UpgradesList.SecondEdition
{
    public class UnkarPlutt : GenericUpgrade
    {
        public UnkarPlutt() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Unkar Plutt",
                UpgradeType.Crew,
                cost: 2,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Scum),
                abilityType: typeof(Abilities.SecondEdition.UnkarPluttCrewAbility),
                seImageNumber: 137
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class UnkarPluttCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishUnsuccessfully += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishUnsuccessfully += RegisterAbility;
        }

        private void RegisterAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskToPerformAction);
        }

        private void AskToPerformAction(object sender, System.EventArgs e)
        {
            Messages.ShowInfoToHuman(HostUpgrade.UpgradeInfo.Name + ": You may suffer 1 damage to perform a white action");

            HostShip.BeforeFreeActionIsPerformed += RegisterSufferDamage;

            HostShip.AskPerformFreeAction(
                HostShip.GetAvailableActionsWhiteOnly(),
                FinishAbility
            );
        }

        private void RegisterSufferDamage(GenericAction action)
        {
            HostShip.BeforeFreeActionIsPerformed -= RegisterSufferDamage;

            RegisterAbilityTrigger(TriggerTypes.BeforeFreeActionIsPerformed, SufferDamage);
        }

        private void SufferDamage(object sender, System.EventArgs e)
        {
            Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + " causes " + HostShip.PilotInfo.PilotName + " to suffer 1 Hit.");

            DamageSourceEventArgs damageArgs = new DamageSourceEventArgs
            {
                DamageType = DamageTypes.CardAbility,
                Source = HostUpgrade
            };

            HostShip.Damage.TryResolveDamage(1, damageArgs, Triggers.FinishTrigger);
        }

        private void FinishAbility()
        {
            HostShip.BeforeFreeActionIsPerformed -= RegisterSufferDamage;
            Triggers.FinishTrigger();
        }
    }
}