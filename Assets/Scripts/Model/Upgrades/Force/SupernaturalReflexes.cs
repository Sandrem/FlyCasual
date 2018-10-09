using Upgrade;
using RuleSets;
using System.Collections.Generic;
using Ship;
using ActionsList;

namespace UpgradesList
{
    public class SupernaturalReflexes : GenericUpgrade
    {
        public SupernaturalReflexes() : base()
        {
            Types.Add(UpgradeType.Force);
            Name = "Supernatural Reflexes";
            Cost = 12;

            UpgradeRuleType = typeof(SecondEdition);

            UpgradeAbilities.Add(new Abilities.SecondEdition.SupernaturalReflexesAbility());

            SEImageNumber = 22;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.ShipBaseSize == BaseSize.Small;
        }
    }
}

namespace Abilities
{
    namespace SecondEdition
    {
        public class SupernaturalReflexesAbility : SabineWrenPilotAbility
        {
            public override void ActivateAbility()
            {
                HostShip.OnMovementActivation += RegisterTrigger;
            }

            public override void DeactivateAbility()
            {
                HostShip.OnMovementActivation -= RegisterTrigger;
            }

            private void RegisterTrigger(GenericShip ship)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementActivation, AskPerformFreeAction);
            }

            private void AskPerformFreeAction(object sender, System.EventArgs e)
            {
                if (HostShip.Force > 0)
                {
                    Messages.ShowInfoToHuman(HostUpgrade.Name + ": You may spend a force token to perform free action");

                    HostShip.BeforeFreeActionIsPerformed += PayForceToken;
                    HostShip.OnActionIsPerformed += CheckSupernaturalReflexesDamage;
                    HostShip.OnActionDecisionSubphaseEndNoAction += DeregisterSupernaturalReflexesEvents;

                    HostShip.AskPerformFreeAction
                    (
                        new List<GenericAction>()
                        {
                            new BoostAction(),
                            new BarrelRollAction()
                        },
                        Triggers.FinishTrigger
                    );
                }
                else
                {
                    Triggers.FinishTrigger();
                }
            }

            private void PayForceToken(GenericAction action)
            {
                HostShip.BeforeFreeActionIsPerformed -= PayForceToken;

                HostShip.Force--;
            }

            private void DeregisterSupernaturalReflexesEvents(GenericShip ship)
            {
                HostShip.BeforeFreeActionIsPerformed -= PayForceToken;
                HostShip.OnActionIsPerformed -= CheckSupernaturalReflexesDamage;
                HostShip.OnActionDecisionSubphaseEndNoAction -= DeregisterSupernaturalReflexesEvents;
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
                Messages.ShowError(HostUpgrade.Name + ": Damage is suffered");

                DamageSourceEventArgs supernaturalReflexesDamage = new DamageSourceEventArgs()
                {
                    Source = this.HostUpgrade,
                    DamageType = DamageTypes.CardAbility
                };

                HostShip.Damage.TryResolveDamage(0, 1, supernaturalReflexesDamage, Triggers.FinishTrigger);
            }
        }
    }
}