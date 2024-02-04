using Upgrade;
using ActionsList;
using Ship;
using System;

namespace UpgradesList.SecondEdition
{
    public class DeadeyeShot : GenericUpgrade
    {
        public DeadeyeShot() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Deadeye Shot",
                UpgradeType.Talent,
                cost: 1,
                abilityType: typeof(Abilities.SecondEdition.DeadeyeShotAbility),
                restriction: new BaseSizeRestriction(BaseSize.Small, BaseSize.Medium)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/4d/84/4d84d319-d6ad-425b-b05e-4e679a54a508/swz70_a1_deadeye-shot_upgrade.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DeadeyeShotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddDeadeyeShotModifications;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddDeadeyeShotModifications;
        }

        private void AddDeadeyeShotModifications(GenericShip ship)
        {
            ship.AddAvailableDiceModificationOwn(
                new DeadeyeShotCritToHitDiceModification()
                {
                    HostShip = HostShip,
                    ImageUrl = HostShip.ImageUrl
                }
            );

            ship.AddAvailableDiceModificationOwn(
                new DeadeyeShotSpendHitDiceModificationFocus()
                {
                    HostShip = HostShip,
                    ImageUrl = HostShip.ImageUrl
                }
            );
        }
    }
}

namespace ActionsList
{
    public class DeadeyeShotCritToHitDiceModification : GenericAction
    {
        public DeadeyeShotCritToHitDiceModification()
        {
            Name = DiceModificationName = "Deadeye Shot (Crit to Hit)";
        }

        public override int GetDiceModificationPriority()
        {
            return 0;
        }

        public override bool IsDiceModificationAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack
                && Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon
                && HostShip.SectorsInfo.IsShipInSector(Combat.Defender, Arcs.ArcType.Bullseye)
                && Combat.DiceRollAttack.HasResult(DieSide.Crit);
        }

        public override void ActionEffect(Action callBack)
        {
            HostShip.AddAlreadyUsedDiceModification(new DeadeyeShotSpendHitDiceModificationFocus() { HostShip = HostShip });

            Combat.DiceRollAttack.ChangeOne(DieSide.Crit, DieSide.Success);

            Combat.Defender.Damage.ExposeRandomFacedownCard(callBack);
        }
    }

    public class DeadeyeShotSpendHitDiceModificationFocus : GenericAction
    {
        public DeadeyeShotSpendHitDiceModificationFocus()
        {
            Name = DiceModificationName = "Deadeye Shot (Spend Hit)";
        }

        public override int GetDiceModificationPriority()
        {
            return 0;
        }

        public override bool IsDiceModificationAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack
                && Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon
                && HostShip.SectorsInfo.IsShipInSector(Combat.Defender, Arcs.ArcType.Bullseye)
                && Combat.DiceRollAttack.HasResult(DieSide.Success);
        }

        public override void ActionEffect(Action callBack)
        {
            HostShip.AddAlreadyUsedDiceModification(new DeadeyeShotCritToHitDiceModification() { HostShip = HostShip });

            Combat.DiceRollAttack.RemoveType(DieSide.Success);

            Combat.Defender.Damage.ExposeRandomFacedownCard(callBack);
        }
    }
}