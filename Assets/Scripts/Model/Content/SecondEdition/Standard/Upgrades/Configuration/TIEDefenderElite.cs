using Upgrade;
using ActionsList;
using Actions;
using System.Collections.Generic;
using Ship;
using System;
using Arcs;
using System.Linq;
using Movement;
using BoardTools;

namespace UpgradesList.SecondEdition
{
    public class TIEDefenderElite : GenericUpgrade
    {
        public TIEDefenderElite() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "TIE Defender Elite",
                UpgradeType.Configuration,
                cost: 0,
                restrictions: new UpgradeCardRestrictions(
                    new FactionRestriction(Faction.Imperial),
                    new ShipRestriction(typeof(Ship.SecondEdition.TIEDDefender.TIEDDefender))
                ),
                abilityType: typeof(Abilities.SecondEdition.TIEDefenderEliteAbility)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/44/f5/44f50470-c0b2-41e8-9ee8-c24edab9d8e7/swz84_upgrade_tiedefenderelite.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TIEDefenderEliteAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            FullThrottleAbility oldAbility = (FullThrottleAbility) HostShip.ShipAbilities.First(n => n.GetType() == typeof(FullThrottleAbility));
            oldAbility.DeactivateAbility();
            HostShip.ShipAbilities.Remove(oldAbility);

            AdvancedFireControlAbility ability = new AdvancedFireControlAbility();
            ability.HostUpgrade = HostUpgrade;
            HostShip.ShipAbilities.Add(ability);
            ability.Initialize(HostShip);

            HostShip.AfterGetManeuverColorDecreaseComplexity += ApplyAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.ShipAbilities.RemoveAll(n => n.GetType() == typeof(AdvancedFireControlAbility));

            FullThrottleAbility ability = new FullThrottleAbility();
            ability.HostUpgrade = HostUpgrade;
            HostShip.ShipAbilities.Add(ability);
            ability.Initialize(HostShip);

            HostShip.AfterGetManeuverColorDecreaseComplexity -= ApplyAbility;
        }

        private void ApplyAbility(GenericShip ship, ref ManeuverHolder movement)
        {
            if (movement.Bearing == ManeuverBearing.Turn)
            {
                movement.ColorComplexity = GenericMovement.ReduceComplexity(movement.ColorComplexity);
            }
            else if (movement.Bearing == ManeuverBearing.KoiogranTurn)
            {
                movement.ColorComplexity = GenericMovement.IncreaseComplexity(movement.ColorComplexity);
            }

            // Update revealed dial in UI
            Roster.UpdateAssignedManeuverDial(HostShip, HostShip.AssignedManeuver);
        }
    }

    public class AdvancedFireControlAbility : GenericAbility
    {
        public GenericShip Defender { get; private set; }

        public override void ActivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (!ActionsHolder.HasTargetLockOn(HostShip, Combat.Defender)) return;

            if (HostShip.IsCannotAttackSecondTime) return;

            if (Combat.ChosenWeapon.WeaponType != WeaponTypes.Cannon && Combat.ChosenWeapon.WeaponType != WeaponTypes.Missile) return;

            Defender = Combat.Defender;
            HostShip.OnCombatCheckExtraAttack += RegisterSecondAttackTrigger;
        }

        private void RegisterSecondAttackTrigger(GenericShip ship)
        {
            HostShip.OnCombatCheckExtraAttack -= RegisterSecondAttackTrigger;

            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, PerformBonusAttack);
        }

        private void PerformBonusAttack(object sender, System.EventArgs e)
        {
            if (!HostShip.IsCannotAttackSecondTime)
            {
                HostShip.IsCannotAttackSecondTime = true;

                Combat.StartSelectAttackTarget(
                    HostShip,
                    FinishAdditionalAttack,
                    IsAllowedAttack,
                    HostUpgrade.UpgradeInfo.Name,
                    "You may perform a bonus primary attack against the same ship",
                    HostUpgrade
                );
            }
            else
            {
                Messages.ShowErrorToHuman(string.Format("{0} cannot attack an additional time", HostShip.PilotInfo.PilotName));
                Triggers.FinishTrigger();
            }
        }

        private void FinishAdditionalAttack()
        {
            // If attack is skipped, set this flag, otherwise regular attack can be performed second time
            HostShip.IsAttackPerformed = true;

            //if bonus attack was skipped, allow bonus attacks again
            if (HostShip.IsAttackSkipped) HostShip.IsCannotAttackSecondTime = false;

            Triggers.FinishTrigger();
        }

        private bool IsAllowedAttack(GenericShip defender, IShipWeapon weapon, bool isSilent)
        {
            if (defender != Defender)
            {
                if (!isSilent) Messages.ShowError("Your attack must be against the same ship");
                return false;
            }

            if (weapon.WeaponType != WeaponTypes.PrimaryWeapon)
            {
                if (!isSilent) Messages.ShowError("Your attack must use a primary weapon");
                return false;
            }

            return true;
        }
    }
}