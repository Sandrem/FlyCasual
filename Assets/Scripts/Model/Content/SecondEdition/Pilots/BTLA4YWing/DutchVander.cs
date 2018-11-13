using Abilities.SecondEdition;
using ActionsList;
using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLA4YWing
    {
        public class DutchVander : BTLA4YWing
        {
            public DutchVander() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Dutch\" Vander",
                    4,
                    42,
                    limited: 1,
                    abilityType: typeof(DutchVanderAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                SEImageNumber = 14;
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DutchVanderAbility : GenericAbility
    {
        private GenericShip LockedShip;

        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckConditions;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckConditions;
        }

        private void CheckConditions(GenericAction action)
        {
            if (action is TargetLockAction && HasPossibleTargets())
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, StartAbility);
            }
        }

        private bool HasPossibleTargets()
        {
            return BoardTools.Board.GetShipsAtRange(HostShip, new Vector2(1, 3), Team.Type.Friendly).Count > 0;
        }

        private void StartAbility(object sender, EventArgs e)
        {
            LockedShip = GetLockedShip();
            if (LockedShip == null)
            {
                Messages.ShowError("\"Dutch\" Vander: No Locked Object!");
                Triggers.FinishTrigger();
                return;
            }

            SelectTargetForAbility(
                GetTargetLockOnSameTarget,
                AnotherFriendlyShipInRange,
                AiPriority,
                HostShip.Owner.PlayerNo,
                HostShip.PilotName,
                "Choose ship. That ship will acquire a lock on the object you locked.",
                HostShip
            );
        }

        private GenericShip GetLockedShip()
        {
            GenericShip result = null;

            BlueTargetLockToken blueTargetLock = HostShip.Tokens.GetToken<BlueTargetLockToken>(letter: '*');
            if (blueTargetLock != null)
            {
                result = blueTargetLock.OtherTokenOwner;
            }

            return result;
        }

        private void GetTargetLockOnSameTarget()
        {
            Messages.ShowInfo(TargetShip.PilotName + " acquired Target Lock on " + LockedShip.PilotName);
            ActionsHolder.AcquireTargetLock(TargetShip, LockedShip, SelectShipSubPhase.FinishSelection, SelectShipSubPhase.FinishSelection, ignoreRange: true);
        }

        private bool AnotherFriendlyShipInRange(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly }) && FilterTargetsByRange(ship, 1, 3);
        }

        private int AiPriority(GenericShip ship)
        {
            int priority = 0;

            if (!ship.Tokens.HasToken(typeof(BlueTargetLockToken))) priority += 50;

            BoardTools.ShotInfo shotInfo = new BoardTools.ShotInfo(ship, LockedShip, ship.PrimaryWeapon);
            if (shotInfo.IsShotAvailable) priority += 40;

            priority += ship.State.Firepower * 5;

            return priority;
        }
    }
}