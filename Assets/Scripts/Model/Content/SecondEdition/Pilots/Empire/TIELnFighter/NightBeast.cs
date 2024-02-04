using Abilities.SecondEdition;
using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class NightBeast : TIELnFighter
        {
            public NightBeast() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Night Beast",
                    "Obsidian Two",
                    Faction.Imperial,
                    2,
                    3,
                    4,
                    isLimited: true,
                    abilityType: typeof(NightBeastAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    seImageNumber: 88
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class NightBeastAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += NightBeastPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= NightBeastPilotAbility;
        }

        protected void NightBeastPilotAbility(GenericShip ship)
        {
            if (BoardTools.Board.IsOffTheBoard(ship)) return;

            if (HostShip.AssignedManeuver.ColorComplexity == Movement.MovementComplexity.Easy)
            {
                Triggers.RegisterTrigger(
                    new Trigger()
                    {
                        Name = HostShip.PilotInfo.PilotName + ": Free Focus action",
                        TriggerOwner = ship.Owner.PlayerNo,
                        TriggerType = TriggerTypes.OnMovementFinish,
                        EventHandler = PerformFreeFocusAction
                    }
                );
            }
        }

        private void PerformFreeFocusAction(object sender, System.EventArgs e)
        {
            HostShip.AskPerformFreeAction(
                new ActionsList.FocusAction(),
                Triggers.FinishTrigger,
                HostShip.PilotInfo.PilotName,
                "After you fully execute a blue maneuver, you may perform a Focus action",
                HostShip
            );
        }
    }
}
