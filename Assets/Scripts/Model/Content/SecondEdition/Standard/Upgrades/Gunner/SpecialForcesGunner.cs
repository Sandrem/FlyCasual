using Upgrade;
using Ship;
using Arcs;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class SpecialForcesGunner : GenericUpgrade
    {
        public SpecialForcesGunner() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Special Forces Gunner",
                UpgradeType.Gunner,
                cost: 5,
                restrictions: new UpgradeCardRestrictions(
                    new FactionRestriction(Faction.FirstOrder),
                    new ShipRestriction(typeof(Ship.SecondEdition.TIESfFighter.TIESfFighter))
                ),
                abilityType: typeof(Abilities.SecondEdition.SpecialForcesGunnerAbility)
            );

            Avatar = new AvatarInfo(
                Faction.FirstOrder,
                new Vector2(336, 1)
            );

            ImageUrl = "https://squadbuilder.fantasyflightgames.com/card_images/en/d3aed19c7eb6f9ebc2352ac49cdd6b87.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SpecialForcesGunnerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckExtraDice;
            HostShip.OnAttackFinishAsAttacker += CheckExtraAttack;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= CheckExtraDice;
            HostShip.OnAttackFinishAsAttacker -= CheckExtraAttack;
        }

        private void CheckExtraDice(ref int count)
        {
            if (Combat.ShotInfo.Weapon.WeaponType == WeaponTypes.PrimaryWeapon
                && Combat.ArcForShot.ArcType == ArcType.Front
                && HostShip.ArcsInfo.GetArc<ArcSingleTurret>().Facing == ArcFacing.Front)
            {
                Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + "gains +1 attack die");
                count++;
            }
        }

        private void CheckExtraAttack(GenericShip ship)
        {
            if (Combat.ShotInfo.Weapon.WeaponType == WeaponTypes.PrimaryWeapon
                && Combat.ArcForShot.ArcType == ArcType.Front
                && HostShip.ArcsInfo.GetArc<ArcSingleTurret>().Facing == ArcFacing.Rear
                && !HostShip.IsCannotAttackSecondTime)
            {
                HostShip.OnCombatCheckExtraAttack += RegisterExtraAttack;
            }
        }

        private void RegisterExtraAttack(GenericShip ship)
        {
            HostShip.OnCombatCheckExtraAttack -= RegisterExtraAttack;
            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, DoExtraPrimaryTurretAttack);
        }

        private void DoExtraPrimaryTurretAttack(object sender, System.EventArgs e)
        {
            Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + "'s bonus attack");

            HostShip.IsCannotAttackSecondTime = true;

            Combat.StartSelectAttackTarget(
                HostShip,
                delegate {
                    Selection.ThisShip.IsAttackPerformed = true;
                    //if bonus attack was skipped, allow bonus attacks again
                    if (Selection.ThisShip.IsAttackSkipped) Selection.ThisShip.IsCannotAttackSecondTime = false;
                    Triggers.FinishTrigger();
                },
                IsPrimaryTurretAttack,
                HostUpgrade.UpgradeInfo.Name,
                "You may perform an bonus attack from your primary turret arc",
                HostUpgrade
            );
        }

        private bool IsPrimaryTurretAttack(GenericShip ship, IShipWeapon weapon, bool isSilent)
        {
            bool result = true;

            if (weapon.WeaponType != WeaponTypes.PrimaryWeapon || !weapon.WeaponInfo.ArcRestrictions.Contains(ArcType.SingleTurret))
            {
                if (!isSilent) Messages.ShowErrorToHuman("You can only perform primary weapon turret attacks");
                result = false;
            }

            return result;
        }
    }
}