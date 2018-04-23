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
            Types.Add(UpgradeType.Title);
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
                if (TargetsForAbilityExist(FilterTargetsOfAbility))
                {
                    RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, SelectTargetForRemoveTargetLock);
                }
            }
        }

        private void SelectTargetForRemoveTargetLock(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                RemoveEnemyTargetLock,
                FilterTargetsOfAbility,
                GetAiPriorityOfTarget,
                HostShip.Owner.PlayerNo,
                true,
                null,
                HostUpgrade.Name,
                "Choose a ship to remove enemy Target Lock from it.",
                HostUpgrade.ImageUrl
            );
        }

        private bool FilterTargetsOfAbility(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.This, TargetTypes.OtherFriendly }) && FilterTargetsByRange(ship, 1, 1) && FilterTargetHasRedTargetLock(ship);
        }

        private bool FilterTargetHasRedTargetLock(GenericShip ship)
        {
            return ship.Tokens.HasToken(typeof(Tokens.RedTargetLockToken), '*');
        }

        private int GetAiPriorityOfTarget(GenericShip ship)
        {
            int priority = 50;

            int damagedPercent = 100 - (ship.Hull + ship.Shields) * 100 / (ship.MaxHull + ship.MaxShields);
            priority += damagedPercent;

            priority += ship.Agility * 5;

            priority -= ship.Tokens.GetAllTokens().Count(n => n is Tokens.FocusToken || n is Tokens.EvadeToken) * 10;

            return priority;
        }

        private bool DoNearbyShipsHaveRedTargetLock(List<GenericShip> nearbyShips)
        {
            return nearbyShips.Any(ship => ship.Tokens.HasToken(typeof(Tokens.RedTargetLockToken), '*'));
        }

        private List<Tokens.RedTargetLockToken> GetShipRedTargetLocks(GenericShip hostShip)
        {
            List<Tokens.RedTargetLockToken> redTargetLocks = new List<Tokens.RedTargetLockToken>();
            foreach (Tokens.GenericToken token in hostShip.Tokens.GetAllTokens())
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
            if (TargetShip.Tokens.HasToken(typeof(Tokens.RedTargetLockToken), '*'))
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
            decisionPhase.DefaultDecisionName = decisionPhase.GetDecisions().First().Name;
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
            TargetShip.Tokens.RemoveToken(
                typeof(Tokens.RedTargetLockToken),
                callback,
                targetLockTokenLetter
            );
        }
    }
}