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
            HostShip.OnAttackFinish += CheckTIEDAbility;
            Phases.OnRoundEnd += Cleanup;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinish -= CheckTIEDAbility;
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
            if (Combat.Attacker.Owner.PlayerNo == HostShip.Owner.PlayerNo && Combat.Attacker.ShipId == HostShip.ShipId && HasCannonWeapon())
            {
                RegisterAbilityTrigger(TriggerTypes.OnExtraAttack, UseTIEDAbility);
            }
        }

        private bool HasCannonWeapon()
        {
            return HostShip.UpgradeBar.GetInstalledUpgrades().Count(n => n.Type == UpgradeType.Cannon && (n as IShipWeapon) != null && n.Cost <= 3) > 0;
        }

        private void UseTIEDAbility(object sender, System.EventArgs e)
        {
            Messages.ShowInfo(HostShip.PilotName + " can perform second attack from primary weapon");
            Combat.StartAdditionalAttack(HostShip, Triggers.FinishTrigger, IsPrimaryShot);
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