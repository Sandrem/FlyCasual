using Ship;
using Upgrade;
using UnityEngine;
using System.Collections.Generic;

namespace UpgradesList.FirstEdition
{
    public class Gunner : GenericUpgrade
    {
        public Gunner() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Gunner",
                UpgradeType.Crew,
                cost: 5,
                abilityType: typeof(Abilities.FirstEdition.GunnerAbility)
            );

            Avatar = new AvatarInfo(Faction.None, new Vector2(61, 1));
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class GunnerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackMissedAsAttacker += CheckGunnerAbility;
            Phases.Events.OnRoundEnd += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackMissedAsAttacker -= CheckGunnerAbility;
            Phases.Events.OnRoundEnd -= ClearIsAbilityUsedFlag;
        }

        private void CheckGunnerAbility()
        {
            if (!IsAbilityUsed && !HostShip.IsCannotAttackSecondTime)
            {
                IsAbilityUsed = true;

                // Trigger must be registered just before it's resolution
                HostShip.OnCombatCheckExtraAttack += RegisterSecondAttackTrigger;
            }
        }

        private void RegisterSecondAttackTrigger(GenericShip ship)
        {
            HostShip.OnCombatCheckExtraAttack -= RegisterSecondAttackTrigger;

            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, UseGunnerAbility);
        }

        private void UseGunnerAbility(object sender, System.EventArgs e)
        {
            if (!HostShip.IsCannotAttackSecondTime)
            {
                HostShip.IsCannotAttackSecondTime = true;

                Combat.StartAdditionalAttack(
                    HostShip,
                    FinishAdditionalAttack,
                    IsPrimaryWeaponShot,
                    HostUpgrade.UpgradeInfo.Name,
                    "You may perform a primary weapon attack.",
                    HostUpgrade
                );
            }
            else
            {
                Messages.ShowErrorToHuman(string.Format("{0} cannot attack one more time", HostShip.PilotName));
                Triggers.FinishTrigger();
            }
        }

        private void FinishAdditionalAttack()
        {
            // If attack is skipped, set this flag, otherwise regular attack can be performed second time
            HostShip.IsAttackPerformed = true;

            Triggers.FinishTrigger();
        }

        private bool IsPrimaryWeaponShot(GenericShip defender, IShipWeapon weapon, bool isSilent)
        {
            bool result = false;

            if (Combat.ChosenWeapon is PrimaryWeaponClass)
            {
                result = true;
            }
            else
            {
                if (!isSilent) Messages.ShowError("Attack must be performed from primary weapon");
            }

            return result;
        }
    }
}