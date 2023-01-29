using ActionsList;
using Arcs;
using BoardTools;
using Content;
using Ship;
using SubPhases;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Movement;
using System.Linq;

namespace Ship
{
    namespace SecondEdition.FangFighter
    {
        public class DirkUllodin : FangFighter
        {
            public DirkUllodin() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Dirk Ullodin",
                    "Aspiring Commando",
                    Faction.Rebel,
                    3,
                    5,
                    10,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.DirkUllodinAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Torpedo
                    },
                    tags: new List<Tags>()
                    {
                        Tags.Mandalorian 
                    },
                    skinName: "Dirk Ullodin"
                );

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/dirkullodin.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you fully execute a red maneuver or perform a red action, if there is an enemy ship in your bullseye arc, 
    //you may acquire a lock on that ship.
    public class DirkUllodinAbility : GenericAbility
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
            if (action.IsRed && Board.GetShipsInArcAtRange(HostShip, ArcType.Front, new Vector2(0, 1), Team.Type.Enemy).Any())
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
            if (HostShip.GetLastManeuverColor() == MovementComplexity.Complex && Board.GetShipsInArcAtRange(HostShip, ArcType.Front, new Vector2(0, 1), Team.Type.Enemy).Any())
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
                "You may aquire a lock on an enemy in your front arc at range 1",
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
            var range = new BoardTools.DistanceInfo(HostShip, ship).Range;
            return ship.Owner != HostShip.Owner && HostShip.SectorsInfo.IsShipInSector(ship, Arcs.ArcType.Front) && range <= 1;
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