using BoardTools;
using Ship;
using System;
using System.Linq;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class SnapShot : GenericSpecialWeapon
    {
        public SnapShot() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Snap Shot",
                UpgradeType.Talent,
                cost: 2,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 2,
                    // Hacking the range to remove this as a possible weapon when ability
                    // is not triggered
                    minRange: 0,
                    maxRange: 0
                ),
                abilityType: typeof(Abilities.FirstEdition.SnapShotAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class SnapShotAbility : GenericAbility
    {

        private GenericShip snapShotTarget = null;

        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += AddSnapShotRestriction;
            GenericShip.OnMovementFinishGlobal += CheckSnapShotAbility;
            Phases.Events.OnActivationPhaseEnd_Triggers += CleanUpSnapShotAbility;
            Phases.Events.OnCombatPhaseEnd_Triggers += CleanUpSnapShotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= AddSnapShotRestriction;
            GenericShip.OnMovementFinishGlobal -= CheckSnapShotAbility;
            Phases.Events.OnActivationPhaseEnd_Triggers -= CleanUpSnapShotAbility;
            Phases.Events.OnCombatPhaseEnd_Triggers -= CleanUpSnapShotAbility;
        }

        private void AddSnapShotRestriction()
        {
            GenericShip ship;
            if (Combat.Attacker.ShipId == HostShip.ShipId && Combat.ChosenWeapon is UpgradesList.FirstEdition.SnapShot)
            {
                ship = Combat.Attacker;

                ship.OnTryAddAvailableDiceModification += UseSnapShotRestriction;
                ship.OnAttackFinish += RemoveSnapShotRestriction;
            }
        }

        private void UseSnapShotRestriction(GenericShip ship, ActionsList.GenericAction action, ref bool canBeUsed)
        {
            Messages.ShowErrorToHuman("SnapShot: Unable to modify dice.");
            canBeUsed = false;
        }

        private void RemoveSnapShotRestriction(GenericShip ship)
        {
            ship.OnTryAddAvailableDiceModification -= UseSnapShotRestriction;
            ship.OnAttackFinish -= RemoveSnapShotRestriction;
        }

        private void CleanUpSnapShotAbility()
        {
            ClearIsAbilityUsedFlag();
            snapShotTarget = null;
            HostShip.IsAttackPerformed = false;
            HostShip.IsCannotAttackSecondTime = false;

            // TODOREVERT
            /*((UpgradesList.FirstEdition.SnapShot)HostUpgrade).MaxRange = 0;
            ((UpgradesList.FirstEdition.SnapShot)HostUpgrade).MinRange = 0;*/
        }

        public void AfterSnapShotAttackSubPhase()
        {
            HostShip.IsAttackPerformed = true;
            HostShip.OnAttackFinishAsAttacker -= SetIsAbilityIsUsed;
            Selection.ChangeActiveShip(snapShotTarget);
            Phases.FinishSubPhase(Phases.CurrentSubPhase.GetType());
            Triggers.FinishTrigger();
        }


        //Based on Dengar counterattack
        private void CheckSnapShotAbility(GenericShip ship)
        {
            if (!IsAbilityUsed && ship.Owner.PlayerNo != HostShip.Owner.PlayerNo)
            {
                snapShotTarget = ship;
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskSnapShotAbility);
            }
        }

        private void AskSnapShotAbility(object sender, System.EventArgs e)
        {
            /*((UpgradesList.FirstEdition.SnapShot)HostUpgrade).MaxRange = 1;
            ((UpgradesList.FirstEdition.SnapShot)HostUpgrade).MinRange = 1;*/
            ShotInfo shotInfo = new ShotInfo(HostShip, snapShotTarget, ((UpgradesList.FirstEdition.SnapShot)HostUpgrade));

            if (shotInfo.InArc && shotInfo.Range <= 1)
            {
                AskToUseAbility(AlwaysUseByDefault, PerformSnapShot);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private bool SnapShotAttackFilter(GenericShip defender, IShipWeapon weapon, bool isSilent)
        {
            bool result = true;
            if (defender != snapShotTarget || !(weapon.GetType() == HostUpgrade.GetType()))
            {
                if (!isSilent) Messages.ShowErrorToHuman(
                    string.Format("Snap Shot target must be {0}, using Snap Shot weapon", snapShotTarget.PilotName));
                result = false;
            }

            return result;
        }

        public void SnapShotSetUsed()
        {
            IsAbilityUsed = true;
        }

        private void PerformSnapShot(object sender, EventArgs e)
        {
            if (!HostShip.IsCannotAttackSecondTime)
            {
                HostShip.OnAttackFinishAsAttacker += SetIsAbilityIsUsed;
                HostShip.IsCannotAttackSecondTime = true;
                Combat.StartAdditionalAttack(
                    HostShip,
                    AfterSnapShotAttackSubPhase,
                    SnapShotAttackFilter,
                    HostUpgrade.UpgradeInfo.Name,
                    "You may perform an additional attack against " + snapShotTarget.PilotName + ".",
                    HostUpgrade
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

    }
}