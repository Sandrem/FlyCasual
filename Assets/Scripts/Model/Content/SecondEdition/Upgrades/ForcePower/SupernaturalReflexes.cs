using ActionsList;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class SupernaturalReflexes : GenericUpgrade, IVariableCost
    {
        public SupernaturalReflexes() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Supernatural Reflexes",
                UpgradeType.ForcePower,
                cost: 12,
                abilityType: typeof(Abilities.SecondEdition.SupernaturalReflexesAbility),
                restriction: new BaseSizeRestriction(BaseSize.Small),
                seImageNumber: 22
            );
        }

        public void UpdateCost(GenericShip ship)
        {
            Dictionary<int, int> initiativeToCost = new Dictionary<int, int>()
            {
                {0, 4},
                {1, 4},
                {2, 4},
                {3, 8},
                {4, 16},
                {5, 24},
                {6, 32}
            };

            UpgradeInfo.Cost = initiativeToCost[ship.PilotInfo.Initiative];
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SupernaturalReflexesAbility : FirstEdition.SabineWrenPilotAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementActivationStart += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementActivationStart -= RegisterTrigger;
        }

        private void RegisterTrigger(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnMovementActivationStart, AskPerformFreeAction);
        }

        private void AskPerformFreeAction(object sender, System.EventArgs e)
        {
            if (HostShip.State.Force > 0)
            {
                HostShip.BeforeActionIsPerformed += PayForceToken;
                HostShip.OnActionIsPerformed += CheckSupernaturalReflexesDamage;
                HostShip.OnActionIsSkipped += DeregisterSupernaturalReflexesEvents;

                HostShip.AskPerformFreeAction
                (
                    new List<GenericAction>()
                    {
                        new BoostAction(),
                        new BarrelRollAction()
                    },
                    Triggers.FinishTrigger,
                    HostUpgrade.UpgradeInfo.Name,
                    "Before you activate, you may spend 1 Force to perform a Barrel Roll or Boost action. Then, if you performed an action you do not have on your action bar, suffer 1 damage.",
                    HostUpgrade
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void PayForceToken(GenericAction action, ref bool isFreeAction)
        {
            HostShip.BeforeActionIsPerformed -= PayForceToken;

            HostShip.State.Force--;
        }

        private void DeregisterSupernaturalReflexesEvents(GenericShip ship)
        {
            HostShip.BeforeActionIsPerformed -= PayForceToken;
            HostShip.OnActionIsPerformed -= CheckSupernaturalReflexesDamage;
            HostShip.OnActionIsSkipped -= DeregisterSupernaturalReflexesEvents;
        }

        public void CheckSupernaturalReflexesDamage(GenericAction action)
        {
            HostShip.OnActionIsPerformed -= CheckSupernaturalReflexesDamage;

            if (!HostShip.ActionBar.HasAction(action.GetType()))
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, SufferDamage);
            }
        }

        private void SufferDamage(object sender, System.EventArgs e)
        {
            Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + ": This ship suffers 1 damage for performing an action not on its action bar");

            DamageSourceEventArgs supernaturalReflexesDamage = new DamageSourceEventArgs()
            {
                Source = this.HostUpgrade,
                DamageType = DamageTypes.CardAbility
            };

            HostShip.Damage.TryResolveDamage(0, 1, supernaturalReflexesDamage, Triggers.FinishTrigger);
        }
    }
}