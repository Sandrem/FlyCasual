using Upgrade;
using Ship;
using ActionsList;
using Ship.T70XWing;
using System;
using Abilities;
using System.Collections.Generic;
using SubPhases;
using UnityEngine;
using System.Linq;

namespace UpgradesList
{
    public class BlackOne : GenericUpgradeSlotUpgrade
    {
        public BlackOne() : base()
        {
            Type = UpgradeType.Title;
            Name = "Black One";
            Cost = 1;

            isUnique = true;

            UpgradeAbilities.Add(new BlackOneAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return (ship is T70XWing) && (ship.PilotSkill > 6);
        }
    }
}

namespace Abilities
{
    public class BlackOneAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += RegisterBlackOneAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= RegisterBlackOneAbility;
        }

        private void RegisterBlackOneAbility(GenericAction shipAction)
        {
            //If the ship doesn't perform a barrel roll or boost action we want to exit out of the method.
            if (shipAction is BoostAction || shipAction is BarrelRollAction)
            {
                List<GenericShip> nearbyShips = Board.BoardManager.GetShipsAtRange(HostShip, new Vector2(1, 1), Team.Type.Friendly);
                if (nearbyShips == null &&
                    nearbyShips.Count <= 0 &&
                    !DoNearbyShipsHaveRedTargetLock(nearbyShips)) //Verifies that at least one nearby ship has a target lock before continuing.
                {
                    return;
                }

                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, SelectTargetForRemoveTargetLock);
            }
        }

        private void SelectTargetForRemoveTargetLock(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                RemoveEnemyTargetLock,
                new List<TargetTypes> { TargetTypes.This, TargetTypes.OtherFriendly },
                new Vector2(1, 1));
        }

        private bool DoNearbyShipsHaveRedTargetLock(List<GenericShip> nearbyShips)
        {
            return nearbyShips.Any(ship => ship.HasToken(typeof(Tokens.RedTargetLockToken), '*'));
        }

        private void RemoveEnemyTargetLock()
        {
            if (TargetShip.HasToken(typeof(Tokens.RedTargetLockToken), '*'))
            {
                TargetShip.RemoveToken(typeof(Tokens.RedTargetLockToken), '*');
                SelectShipSubPhase.FinishSelection();
                Messages.ShowInfoToHuman(string.Format("Target Lock has been removed from {0}.", HostShip.PilotName));
            }
            else
            {
                Messages.ShowErrorToHuman(string.Format("{0}'s ship does not have an enemy Target Lock.", HostShip.PilotName));
            }
        }
    }
}