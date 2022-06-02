using Abilities.SecondEdition;
using ActionsList;
using Ship;
using SubPhases;
using System;
using Content;
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
                PilotInfo = new PilotCardInfo25
                (
                    "\"Dutch\" Vander",
                    "Gold Leader",
                    Faction.Rebel,
                    4,
                    4,
                    12,
                    isLimited: true,
                    abilityType: typeof(DutchVanderAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Turret,
                        UpgradeType.Torpedo,
                        UpgradeType.Missile,
                        UpgradeType.Astromech,
                        UpgradeType.Device,
                        UpgradeType.Device,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.YWing
                    },
                    seImageNumber: 14
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DutchVanderAbility : GenericAbility
    {
        private ITargetLockable LockedShip;

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
                Messages.ShowError(HostShip.PilotInfo.PilotName + " doesn't have any locked targets!");
                Triggers.FinishTrigger();
                return;
            }

            SelectTargetForAbility(
                GetTargetLockOnSameTarget,
                AnotherFriendlyShipInRange,
                AiPriority,
                HostShip.Owner.PlayerNo,
                HostShip.PilotInfo.PilotName,
                "Choose a ship, that ship will acquire a lock on the object you locked",
                HostShip
            );
        }

        private ITargetLockable GetLockedShip()
        {
            ITargetLockable result = null;

            BlueTargetLockToken blueTargetLock = HostShip.Tokens.GetTokens<BlueTargetLockToken>(letter: '*').Last();
            if (blueTargetLock != null)
            {
                result = blueTargetLock.OtherTargetLockTokenOwner;
            }

            return result;
        }

        private void GetTargetLockOnSameTarget()
        {
            if (LockedShip is GenericShip)
            {
                Messages.ShowInfo(TargetShip.PilotInfo.PilotName + " acquired a Target Lock on " + (LockedShip as GenericShip).PilotInfo.PilotName);
            }
            else
            {
                Messages.ShowInfo(TargetShip.PilotInfo.PilotName + " acquired a Target Lock on obstacle");
            }
            
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

            if (LockedShip is GenericShip)
            {
                BoardTools.ShotInfo shotInfo = new BoardTools.ShotInfo(ship, LockedShip as GenericShip, ship.PrimaryWeapons);
                if (shotInfo.IsShotAvailable) priority += 40;
            }

            priority += ship.State.Firepower * 5;

            return priority;
        }
    }
}