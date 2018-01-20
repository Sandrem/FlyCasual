﻿using Ship;
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
            Type = UpgradeType.Title;
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
            Phases.OnRoundEnd += Cleanup;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker -= CheckTIEDAbility;
            Phases.OnRoundEnd -= Cleanup;
        }

        private void Cleanup()
        {
            IsAbilityUsed = false;
        }

        private void CheckTIEDAbility(GenericShip ship)
        {
            if (!IsAbilityUsed)
            {
                RegisterTIEDAbility();
                IsAbilityUsed = true;
            }
        }

        private void RegisterTIEDAbility()
        {
            if (IsAttackWithCannonUpgradeCost3OrFewer())
            {
                RegisterAbilityTrigger(TriggerTypes.OnExtraAttack, UseTIEDAbility);
            }
        }

        private bool IsAttackWithCannonUpgradeCost3OrFewer()
        {
            bool result = false;

            GenericSecondaryWeapon secondaryWeapon = Combat.ChosenWeapon as GenericSecondaryWeapon;
            if (secondaryWeapon != null && secondaryWeapon.Type == UpgradeType.Cannon && secondaryWeapon.Cost <= 3)
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
                delegate {
                    Selection.ThisShip.IsAttackPerformed = true;
                    Triggers.FinishTrigger();
                },
                IsPrimaryShot
            );
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