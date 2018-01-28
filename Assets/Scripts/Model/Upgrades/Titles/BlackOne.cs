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
                if (nearbyShips == null ||
                    nearbyShips.Count <= 0 ||
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

        private List<Tokens.RedTargetLockToken> GetShipRedTargetLocks(GenericShip hostShip)
        {
            List<Tokens.RedTargetLockToken> redTargetLocks = new List<Tokens.RedTargetLockToken>();
            foreach (Tokens.GenericToken token in hostShip.GetAllTokens())
            {
                if (token is Tokens.RedTargetLockToken)
                {
                    redTargetLocks.Add(token as Tokens.RedTargetLockToken);
                }
            }
            return redTargetLocks;
        }

        private void RemoveEnemyTargetLock()
        {
            if (TargetShip.HasToken(typeof(Tokens.RedTargetLockToken), '*'))
            {
                if (GetShipRedTargetLocks(TargetShip).Count > 1)
                {
                    RegisterSubDecision();
                }
                else
                {
                    RemoveRedTargetLock('*', SelectShipSubPhase.FinishSelection);
                }
            }
            else
            {
                Messages.ShowErrorToHuman(string.Format("{0}'s ship does not have an enemy Target Lock.", TargetShip.PilotName));
            }
        }

        private void RegisterSubDecision()
        {
            DecisionSubPhase decisionPhase = (DecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(DecisionSubPhase),
                SelectShipSubPhase.FinishSelection
            );

            decisionPhase.InfoText = "Determine which Target Lock to remove:";

            foreach (Tokens.RedTargetLockToken token in GetShipRedTargetLocks(TargetShip))
            {
                decisionPhase.AddDecision(token.Letter.ToString(), delegate { RemoveTargetLockDecision(token.Letter); });
            }

            //AI's default behavior.
            decisionPhase.DefaultDecision = decisionPhase.GetDecisions().First().Key;
            decisionPhase.RequiredPlayer = HostShip.Owner.PlayerNo;

            decisionPhase.Start();
        }

        private void RemoveTargetLockDecision(char targetLockTokenLetter)
        {
            RemoveRedTargetLock(targetLockTokenLetter, DecisionSubPhase.ConfirmDecision);
        }

        private void RemoveRedTargetLock(char targetLockTokenLetter, Action callback)
        {
            Messages.ShowInfoToHuman(string.Format("Target Lock has been removed from {0}.", TargetShip.PilotName));
            TargetShip.RemoveToken(
                typeof(Tokens.RedTargetLockToken),
                callback,
                targetLockTokenLetter
            );
        }
    }
}