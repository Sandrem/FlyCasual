using Ship;
using Upgrade;
using System.Collections.Generic;
using Arcs;

namespace UpgradesList.FirstEdition
{
    public class BTLA4YWing : GenericUpgrade
    {
        public BTLA4YWing() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "BTL-A4 Y-wing",
                UpgradeType.Title,
                cost: 0,          
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.YWing.YWing)),
                abilityType: typeof(Abilities.FirstEdition.BTLA4YWingAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class BTLA4YWingAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Toggle360Arc(false);
            HostShip.OnShotStartAsAttacker += CheckConditions;
            Phases.Events.OnRoundEnd += ClearAbility;
        }

        public override void DeactivateAbility()
        {
            Toggle360Arc(true);
            HostShip.OnShotStartAsAttacker -= CheckConditions;
            Phases.Events.OnRoundEnd -= ClearAbility;
        }

        private void Toggle360Arc(bool isActive)
        {
            GenericSpecialWeapon turret = (GenericSpecialWeapon) HostShip.UpgradeBar.GetUpgradesAll().Find(n => n.HasType(UpgradeType.Turret));
            if (turret != null)
            {
                // TODOREVERT
                //HostShip.ArcsInfo.GetArc<OutOfArc>().ShotPermissions.CanShootTurret = isActive;
                //turret.CanShootOutsideArc = isActive;
            }
        }

        private void CheckConditions()
        {
            if (IsAbilityUsed) return;
            if (Combat.ChosenWeapon.WeaponType != WeaponTypes.PrimaryWeapon) return;
            if (!Combat.ShotInfo.InPrimaryArc) return;

            IsAbilityUsed = true;
            HostShip.OnCombatCheckExtraAttack += RegisterBTLA4ExtraAttack;
        }

        private void RegisterBTLA4ExtraAttack(GenericShip ship)
        {
            HostShip.OnCombatCheckExtraAttack -= RegisterBTLA4ExtraAttack;
            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, DoBTL4AExtraAttack);
        }

        private void DoBTL4AExtraAttack(object sender, System.EventArgs e)
        {
            Combat.StartSelectAttackTarget(
                HostShip,
                delegate {
                    Selection.ThisShip.IsAttackPerformed = true;
                    Triggers.FinishTrigger();
                },
                IsSecondaryShot,
                HostUpgrade.UpgradeInfo.Name,
                "You can perform an addition attack with a turret againts target inside your firing arc.",
                HostUpgrade
            );
        }

        private bool IsSecondaryShot(GenericShip defender, IShipWeapon weapon, bool isSilent)
        {
            bool result = false;
            if (weapon.WeaponType != WeaponTypes.PrimaryWeapon)
            {
                result = true;
            }
            else
            {
                if (!isSilent) Messages.ShowError("This attack must be performed with a secondary weapon.");
            }
            return result;
        }

        private void ClearAbility()
        {
            IsAbilityUsed = false;
        }
    }
}