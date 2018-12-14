using ActionsList;
using Ship;
using System;
using Tokens;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class PlasmaTorpedoes : GenericSpecialWeapon
    {
        public PlasmaTorpedoes() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Plasma Torpedoes",
                UpgradeType.Torpedo,
                cost: 3,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 4,
                    minRange: 2,
                    maxRange: 3,
                    requiresToken: typeof(BlueTargetLockToken),
                    spendsToken: typeof(BlueTargetLockToken),
                    discard: true
                ),
                abilityType: typeof(Abilities.FirstEdition.PlasmaTorpedoAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class PlasmaTorpedoAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotHitAsAttacker += PlanPlasmaTorpedoes;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCombatDeactivation += DeactivateAbilityPlanned;
        }

        private void DeactivateAbilityPlanned(GenericShip ship)
        {
            HostShip.OnCombatDeactivation -= DeactivateAbilityPlanned;

            HostShip.OnCombatDeactivation -= PlanShieldRemove;

            HostShip.OnShotHitAsAttacker -= PlanPlasmaTorpedoes;
        }

        private void PlanPlasmaTorpedoes()
        {
            if (Combat.ChosenWeapon is UpgradesList.FirstEdition.PlasmaTorpedoes)
            {
                HostShip.OnAttackFinishAsAttacker += PlanShieldRemove;

                HostShip.OnShotHitAsAttacker -= PlanPlasmaTorpedoes;
            }
        }

        private void PlanShieldRemove(GenericShip hostShip)
        {
            if (Combat.Defender.State.ShieldsCurrent != 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, ShieldRemove);
            }
        }

        private void ShieldRemove(object sender, EventArgs e)
        {
            Messages.ShowInfoToHuman(string.Format("{0} had a Shield removed by Plasma Torpedo", Combat.Defender.PilotInfo.PilotName));

            Combat.Defender.Damage.SufferRegularDamage(
                new DamageSourceEventArgs()
                {
                    Source = Combat.Attacker,
                    DamageType = DamageTypes.CardAbility
                },
                Triggers.FinishTrigger
            );
        }
    }
}