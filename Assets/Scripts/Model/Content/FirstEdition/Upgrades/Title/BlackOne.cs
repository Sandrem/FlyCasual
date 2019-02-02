using ActionsList;
using Ship;
using SquadBuilderNS;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class BlackOne : GenericUpgrade
    {
        public BlackOne() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Black One",
                UpgradeType.Title,
                cost: 1,
                isLimited: true,
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.T70XWing.T70XWing)),
                abilityType: typeof(Abilities.FirstEdition.BlackOneAbility)
            );
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.PilotInfo.Initiative > 6;
        }

        public override bool IsAllowedForSquadBuilderPostCheck(SquadList squadList)
        {
            bool result = HostShip.PilotInfo.Initiative > 6;
            if (!result) Messages.ShowError("You cannot equip \"Black One\" if pilot's skill is \"6\" or lower");
            return result;
        }
    }
}

namespace Abilities.FirstEdition
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
                HostUpgrade.UpgradeInfo.Name,
                "Choose a ship to remove enemy Target Lock from it.",
                HostUpgrade
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

            int damagedPercent = 100 - (ship.State.HullCurrent + ship.State.ShieldsCurrent) * 100 / (ship.State.HullCurrent + ship.State.ShieldsCurrent);
            priority += damagedPercent;

            priority += ship.State.Agility * 5;

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
                Messages.ShowErrorToHuman(string.Format("{0}'s ship does not have an enemy Target Lock.", TargetShip.PilotInfo.PilotName));
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
            Messages.ShowInfoToHuman(string.Format("Target Lock has been removed from {0}.", TargetShip.PilotInfo.PilotName));
            TargetShip.Tokens.RemoveToken(
                typeof(Tokens.RedTargetLockToken),
                callback,
                targetLockTokenLetter
            );
        }
    }
}