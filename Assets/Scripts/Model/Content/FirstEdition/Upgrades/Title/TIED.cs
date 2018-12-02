using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class TIED : GenericUpgrade
    {
        public TIED() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "TIE/D",
                UpgradeType.Title,
                cost: 0,
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.TIEDefender.TIEDefender)),
                abilityType: typeof(Abilities.FirstEdition.TIEDAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class TIEDAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker += CheckTIEDAbility;
            Phases.Events.OnRoundEnd += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker -= CheckTIEDAbility;
            Phases.Events.OnRoundEnd -= ClearIsAbilityUsedFlag;
        }

        private void CheckTIEDAbility(GenericShip ship)
        {
            // Attack must be from cannon with cost <=3
            if (!IsAbilityUsed && IsAttackWithCannonUpgradeCost3OrFewer())
            {
                IsAbilityUsed = true;

                // Trigger must be registered just before it's resolution
                HostShip.OnCombatCheckExtraAttack += RegisterTIEDAbility;
            }
        }

        private void RegisterTIEDAbility(GenericShip ship)
        {
            HostShip.OnCombatCheckExtraAttack -= RegisterTIEDAbility;

            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, UseTIEDAbility);
        }

        private bool IsAttackWithCannonUpgradeCost3OrFewer()
        {
            bool result = false;

            GenericSpecialWeapon secondaryWeapon = Combat.ChosenWeapon as GenericSpecialWeapon;
            if (secondaryWeapon != null && secondaryWeapon.HasType(UpgradeType.Cannon) && secondaryWeapon.UpgradeInfo.Cost <= 3)
            {
                result = true;
            }

            return result;
        }

        private void UseTIEDAbility(object sender, System.EventArgs e)
        {
            Messages.ShowInfo(HostShip.PilotName + " can perform second attack from primary weapon");

            Combat.StartAdditionalAttack(
                HostShip,
                FinishAdditionalAttack,
                IsPrimaryShot,
                HostUpgrade.UpgradeInfo.Name,
                "You may perform a primary weapon attack.",
                HostUpgrade
            );
        }

        private void FinishAdditionalAttack()
        {
            // If attack is skipped, set this flag, otherwise regular attack can be performed second time
            Selection.ThisShip.IsAttackPerformed = true;

            Triggers.FinishTrigger();
        }

        private bool IsPrimaryShot(GenericShip defender, IShipWeapon weapon, bool isSilent)
        {
            bool result = false;

            if (weapon.GetType() == typeof(PrimaryWeaponClass))
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