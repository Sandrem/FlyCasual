using Ship;
using Upgrade;
using Movement;
using Actions;
using ActionsList;
using System;
using BoardTools;
using Tokens;
using System.Linq;

namespace UpgradesList.SecondEdition
{
    public class HanSoloRebel : GenericUpgrade
    {
        public HanSoloRebel() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Han Solo",
                UpgradeType.Gunner,
                cost: 12,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.SecondEdition.HanSoloRebelGunnerAbility),
                seImageNumber: 97
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class HanSoloRebelGunnerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnEngagementInitiativeChanged += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnEngagementInitiativeChanged -= CheckAbility;
        }

        private void CheckAbility()
        {
            if (Phases.CurrentSubPhase.RequiredInitiative == 7)
            {
                RegisterAbilityTrigger(TriggerTypes.OnEngagementInitiativeChanged, PerformFreeAttack);
            }
        }

        private void PerformFreeAttack(object sender, EventArgs e)
        {
            // Plan to set arc as used only after attack that was successfully started
            HostShip.OnAttackStartAsAttacker += MarkArcAsUsed;

            Combat.StartSelectAttackTarget(
                HostShip,
                FinishExtraAttack,
                TurretPointerAttackFilter,
                HostUpgrade.UpgradeInfo.Name,
                "You may perform a turret attack (You cannot attack from that turret indicator again this round)",
                HostUpgrade
            );
        }

        private void FinishExtraAttack()
        {
            // Set arc as used only after attack that was successfully started
            HostShip.OnAttackStartAsAttacker -= MarkArcAsUsed;

            // Can attack as usual later
            HostShip.IsAttackPerformed = false;
            HostShip.IsAttackSkipped = false;

            Triggers.FinishTrigger();
        }

        private void MarkArcAsUsed()
        {
            Combat.ShotInfo.ShotAvailableFromArcs.First().CannotBeUsedForAttackThisRound = true;
        }

        private bool TurretPointerAttackFilter(GenericShip target, IShipWeapon weapon, bool isSilent)
        {
            bool result = true;

            if (!weapon.WeaponInfo.ArcRestrictions.Contains(Arcs.ArcType.SingleTurret))
            {
                if (!isSilent) Messages.ShowErrorToHuman(string.Format("{0} can be used only for turret attack", HostUpgrade.UpgradeInfo.Name));
                result = false;
            }

            return result;
        }
    }
}