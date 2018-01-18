using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;
using Ship;
using Upgrade;
using System.Linq;

namespace Ship
{
    namespace Aggressor
    {
        public class IG88B : Aggressor
        {
            public IG88B() : base()
            {
                PilotName = "IG-88B";
                PilotSkill = 6;
                Cost = 36;

                IsUnique = true;

                PilotAbilities.Add(new IG88BAbility());
            }
        }
    }
}

namespace Abilities
{
    public class IG88BAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackMissedAsAttacker += CheckIG88Ability;
            Phases.OnRoundEnd += Cleanup;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackMissedAsAttacker -= CheckIG88Ability;
            Phases.OnRoundEnd -= Cleanup;
        }

        private void Cleanup()
        {
            IsAbilityUsed = false;
        }

        private void CheckIG88Ability()
        {
            if (!IsAbilityUsed)
            {
                RegisterIG88BAbility();
                IsAbilityUsed = true;
            }
        }

        private void RegisterIG88BAbility()
        {
            if (HasCannonWeapon())
            {
                RegisterAbilityTrigger(TriggerTypes.OnExtraAttack, UseIG88BAbility);
            }
        }

        private bool HasCannonWeapon()
        {
            return HostShip.UpgradeBar.GetInstalledUpgrades().Count(n => n.Type == UpgradeType.Cannon && (n as IShipWeapon) != null) > 0;
        }

        private void UseIG88BAbility(object sender, System.EventArgs e)
        {
            Messages.ShowInfo(HostShip.PilotName + " can perform second attack\nfrom Cannon");
            Combat.StartAdditionalAttack(HostShip,
                delegate {
                    Selection.ThisShip.IsAttackPerformed = true;
                    Triggers.FinishTrigger();
                },
                IsCannonShot
            );
        }

        private bool IsCannonShot(GenericShip defender, IShipWeapon weapon)
        {
            bool result = false;

            GenericSecondaryWeapon upgradeWeapon = weapon as GenericSecondaryWeapon;
            if (upgradeWeapon != null && upgradeWeapon.Type == UpgradeType.Cannon)
            {
                result = true;
            }
            else
            {
                Messages.ShowError("Attack must be performed from Cannon");
            }

            return result;
        }
    }
}
