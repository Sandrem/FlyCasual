using Ship;
using Ship.TIEDefender;
using Upgrade;
using System.Collections.Generic;
using System;
using UpgradesList;
using SubPhases;
using ActionsList;
using Abilities;
using System.Linq;
using UnityEngine;

namespace UpgradesList
{
    public class TIED : GenericUpgrade
    {
        public bool IsAlwaysUse;

        public TIED() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "TIE/D";
            Cost = 0;

            UpgradeAbilities.Add(new TIEDAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is TIEDefender;
        }
    }
}

namespace Abilities
{
    public class TIEDAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker += CheckTIEDAbility;
            Phases.OnRoundEnd += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker -= CheckTIEDAbility;
            Phases.OnRoundEnd -= ClearIsAbilityUsedFlag;
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

            GenericSecondaryWeapon secondaryWeapon = Combat.ChosenWeapon as GenericSecondaryWeapon;
            if (secondaryWeapon != null && secondaryWeapon.hasType(UpgradeType.Cannon) && secondaryWeapon.Cost <= 3)
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
                HostUpgrade.Name,
                "You may perfrom a primary weapon attack.",
                HostUpgrade.ImageUrl
            );
        }

        private void FinishAdditionalAttack()
        {
            // If attack is skipped, set this flag, otherwise regular attack can be performed second time
            Selection.ThisShip.IsAttackPerformed = true;

            Triggers.FinishTrigger();
        }

        private bool IsPrimaryShot(GenericShip defender, IShipWeapon weapon)
        {
            bool result = false;

            if (weapon.GetType() == typeof(PrimaryWeaponClass))

            {
                result = true;
            }
            else
            {
                Messages.ShowError("Attack must be performed from primary weapon");
            }

            return result;
        }
    }
}