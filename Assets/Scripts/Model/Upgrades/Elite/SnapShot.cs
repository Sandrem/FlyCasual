using Upgrade;
using UnityEngine;
using Ship;
using System.Collections.Generic;
using Tokens;
using Abilities;
using BoardTools;
using SubPhases;
using UpgradesList;
using ActionsList;
using System;

namespace UpgradesList
{
    public class SnapShot : GenericSecondaryWeapon
    {
        public SnapShot() : base()
        {
            Types.Add(UpgradeType.Cannon);
            Name = "Snap Shot";
            MinRange = 1;
            MaxRange = 1;
            AttackValue = 2;
            Cost = 2;

            UpgradeAbilities.Add(new SnapShotAbility());
        }

    }
}

namespace Abilities
{
    public class SnapShotAbility : GenericAbility
    {
        private GenericShip snapShotTarget = null;

        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += AddSnapShotRestriction;
            GenericShip.OnMovementFinishGlobal += CheckSnapShotAbility;
            Phases.OnActivationPhaseEnd_Triggers += CleanUpSnapShotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += AddSnapShotRestriction;
            GenericShip.OnMovementFinishGlobal -= CheckSnapShotAbility;
            Phases.OnActivationPhaseEnd_Triggers -= CleanUpSnapShotAbility;
        }

        private void AddSnapShotRestriction() {
            GenericShip ship;
            if (Combat.Attacker.ShipId == HostShip.ShipId && snapShotTarget != null) {
                ship = Combat.Attacker;

                ship.OnTryAddAvailableActionEffect += UseSnapShotRestriction;
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
            ship.OnTryAddAvailableActionEffect -= UseSnapShotRestriction;
            ship.OnAttackFinish -= RemoveSnapShotRestriction;
        }

        private void CleanUpSnapShotAbility()
        {
            ClearIsAbilityUsedFlag();
            HostShip.IsAttackPerformed = false;
            HostShip.IsCannotAttackSecondTime = false;
        }

        public void AfterSnapShotAttackSubPhase()
        {
            HostShip.IsCannotAttackSecondTime = true;
            snapShotTarget = null;
            // Check this
            HostShip.OnAttackFinishAsAttacker -= SetIsAbilityIsUsed;
            Phases.FinishSubPhase(Phases.CurrentSubPhase.GetType());
            Triggers.FinishTrigger();
        }


        //Based on Dengar counterattack
        private void CheckSnapShotAbility(GenericShip ship)
        {
            if (!IsAbilityUsed && ship.Owner.PlayerNo != HostShip.Owner.PlayerNo)
            {
                snapShotTarget = ship;
                RegisterAbilityTrigger(TriggerTypes.OnShipMovementFinish, AskSnapShotAbility);
            }
        }

        private void AskSnapShotAbility(object sender, System.EventArgs e)
        {
            ShotInfo shotInfo = new ShotInfo(HostShip, snapShotTarget, HostShip.PrimaryWeapon);

            if (shotInfo.InArc && shotInfo.Range <= 1)
            {
                AskToUseAbility(AlwaysUseByDefault, PerformSnapShot);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private bool SnapShotAttackFilter(GenericShip defender, IShipWeapon weapon)
        {
            bool result = true;
            if (defender != snapShotTarget || !(weapon is UpgradesList.SnapShot))
            {
                Messages.ShowErrorToHuman(
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
                Combat.StartAdditionalAttack(
                    HostShip,
                    AfterSnapShotAttackSubPhase,
                    SnapShotAttackFilter,
                    HostShip.PilotName,
                    "You may perform an additional attack against " + snapShotTarget.PilotName + ".",
                    HostShip.ImageUrl
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
       
    }
}
