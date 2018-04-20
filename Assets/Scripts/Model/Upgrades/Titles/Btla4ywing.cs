using Ship;
using Ship.YWing;
using Upgrade;
using Abilities;

namespace UpgradesList
{
    public class BTLA4 : GenericUpgrade
    {
        public BTLA4() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "BTL-A4 Y-wing";            
            Cost = 0;            

            UpgradeAbilities.Add(new BTLA4Ability());            
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is YWing;
        }
    }
}

//Special ops
namespace Abilities
{
    public class BTLA4Ability : GenericAbility
    {        
        public override void ActivateAbility() {
            Toggle360Arc(false);
            HostShip.OnShotStartAsAttacker += CheckConditions;
            Phases.OnRoundEnd += ClearAbility;
        }

        public override void DeactivateAbility() {
            Toggle360Arc(true);
            HostShip.OnShotStartAsAttacker -= CheckConditions;
            Phases.OnRoundEnd -= ClearAbility;
        }

        private void Toggle360Arc(bool isActive)
        {
            GenericSecondaryWeapon turret = (GenericSecondaryWeapon)HostShip.UpgradeBar.GetUpgradesAll().Find(n => n.hasType(UpgradeType.Turret));
            if (turret != null)
            {
                HostShip.ArcInfo.OutOfArcShotPermissions.CanShootTurret = isActive;
                turret.CanShootOutsideArc = isActive;
            }
        }

        private void CheckConditions()
        {
            if (IsAbilityUsed) return;
            if (Combat.ChosenWeapon.GetType() != typeof(PrimaryWeaponClass)) return;
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
            Combat.StartAdditionalAttack(
                HostShip,
                delegate {
                    Selection.ThisShip.IsAttackPerformed = true;                    
                    Triggers.FinishTrigger();
                },
                IsSecondaryShot,
                HostUpgrade.Name,
                "You can perform an addition attack with a turret againts target inside your firing arc.",
                HostUpgrade.ImageUrl
            );
        }

        private bool IsSecondaryShot(GenericShip defender, IShipWeapon weapon)
        {
            bool result = false;
            if (weapon.GetType() != typeof(PrimaryWeaponClass))
            {
                result = true;
            }
            else
            {
                Messages.ShowError("Attack must be performed from secondary weapon");
            }
            return result;
        }

        private void ClearAbility()
        {
            IsAbilityUsed = false;

        }
    }
}