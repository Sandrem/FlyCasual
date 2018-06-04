using Upgrade;
using UnityEngine;
using Ship;
using System.Collections.Generic;
using Tokens;
using Abilities;
using BoardTools;

namespace UpgradesList
{
    public class SnapShot : GenericUpgrade
    {
        public SnapShot() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Snap Shot";
            Cost = 2;

            UpgradeAbilities.Add(new SnapShotAbility());
        }

    }
}

namespace Abilities
{
    public class SnapShotAbility : GenericAbility
    {
        private GenericShip snapShotTarget;

        public override void ActivateAbility()
        {
            // mix Kanan Jarrus, TIE Defender x7, Quickdraw and Fenn Rau
            GenericShip.OnMovementFinishGlobal += CheckSnapShotAbility;
            Phases.OnActivationPhaseEnd_Triggers += CleanUpSnapShotAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnMovementFinishGlobal -= CheckSnapShotAbility;
            Phases.OnActivationPhaseEnd_Triggers -= CleanUpSnapShotAbility;
        }

        private void CheckSnapShotAbility(GenericShip ship)
        {
            if (ship.Owner != HostShip.Owner && !IsAbilityUsed)
            {
                ShotInfo shotInfo = new ShotInfo(HostShip, ship, HostShip.PrimaryWeapon);
                if (shotInfo.InArc && shotInfo.Range <= 1)
                {
                    snapShotTarget = ship;
                    AskToUseAbility(NeverUseByDefault, PerformSnapShotAbility);
                }

            } else {
                Triggers.FinishTrigger();
            }
        }

        private void PerformSnapShotAbility(object sender, System.EventArgs e)
        {
            IsAbilityUsed = true;
            Combat.StartAdditionalAttack(
                HostShip, 
                AfterSnapShotAttackSubPhase, 
                SnapShotAttackFilter, 
                HostUpgrade.Name,
                "You may perform an attack",
                HostUpgrade.ImageUrl
            );
        }

        private bool SnapShotAttackFilter(GenericShip defender, IShipWeapon weapon) {
            // Stuck here. Snapshot weapon?
            return defender == snapShotTarget && weapon == HostShip.PrimaryWeapon;
        }

        private void CleanUpSnapShotAbility() {
            ClearIsAbilityUsedFlag();
            HostShip.IsCannotAttackSecondTime = false;
        }

        private void AfterSnapShotAttackSubPhase() {
            HostShip.IsCannotAttackSecondTime = true;
            Triggers.FinishTrigger();
        }
    }
}
