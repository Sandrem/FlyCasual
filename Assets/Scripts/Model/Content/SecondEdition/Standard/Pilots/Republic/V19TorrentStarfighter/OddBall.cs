using ActionsList;
using BoardTools;
using Content;
using Movement;
using Ship;
using SubPhases;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.V19TorrentStarfighter
{
    public class OddBall : V19TorrentStarfighter
    {
        public OddBall()
        {
            PilotInfo = new PilotCardInfo25
            (
                "\"Odd Ball\"",
                "CC-2237",
                Faction.Republic,
                5,
                4,
                16,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.OddBallAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Torpedo,
                    UpgradeType.Missile
                },
                tags: new List<Tags>
                {
                    Tags.Clone
                }
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/18/d2/18d2b2c2-482a-4b6f-8c53-c3f0f24bea4b/swz32_odd-ball.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you fully execute a red maneuver or perform a red action, if there is an enemy ship in your bullseye arc, 
    //you may acquire a lock on that ship.
    public class OddBallAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckConditions;
            HostShip.OnMovementFinishSuccessfully += RegisterMovementTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckConditions;
            HostShip.OnMovementFinishSuccessfully -= RegisterMovementTrigger;
        }

        protected void CheckConditions(GenericAction action)
        {
            if (action.IsRed && Board.GetShipsInBullseyeArc(HostShip, Team.Type.Enemy).Any())
            {
                HostShip.OnActionDecisionSubphaseEnd += RegisterActionTrigger;
            }
        }

        private void RegisterActionTrigger(GenericShip ship)
        {
            HostShip.OnActionDecisionSubphaseEnd -= RegisterActionTrigger;

            RegisterAbilityTrigger(TriggerTypes.OnFreeAction, AskAbility);
        }

        protected void RegisterMovementTrigger(GenericShip ship)
        {
            if (HostShip.GetLastManeuverColor() == MovementComplexity.Complex && Board.GetShipsInBullseyeArc(HostShip, Team.Type.Enemy).Any())
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskAbility);
            }
        }

        private void AskAbility(object sender, System.EventArgs e)
        {
            SelectTargetForAbility(
                    GrantFreeTargetLock,
                    FilterAbilityTargets,
                    GetAiAbilityPriority,
                    HostShip.Owner.PlayerNo,
                    HostName,
                    "You may aquire a lock on an enemy in your bullseye arc",
                    HostShip
                );
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            var result = 0;

            var range = new BoardTools.DistanceInfo(HostShip, ship).Range;

            result += (3 - range) * 100;

            result += ship.PilotInfo.Cost;

            return result;
        }

        private bool FilterAbilityTargets(GenericShip ship)
        {
            return ship.Owner != HostShip.Owner && HostShip.SectorsInfo.IsShipInSector(ship, Arcs.ArcType.Bullseye);
        }

        private void GrantFreeTargetLock()
        {
            if (TargetShip != null)
            {
                ActionsHolder.AcquireTargetLock(HostShip, TargetShip, SelectShipSubPhase.FinishSelection, SelectShipSubPhase.FinishSelection);
            }
            else
            {
                SelectShipSubPhase.FinishSelection();
            }
        }
    }
}
