using Abilities.SecondEdition;
using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class NightBeastSSP : TIELnFighter
        {
            public NightBeastSSP() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Night Beast",
                    "Obsidian Two",
                    Faction.Imperial,
                    2,
                    3,
                    0,
                    isLimited: true,
                    abilityType: typeof(NightBeastSSPAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    isStandardLayout: true
                );

                MustHaveUpgrades.Add(typeof(Disciplined));
                MustHaveUpgrades.Add(typeof(Predator));

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/nightbeast-swz105.png";

                PilotNameCanonical = "nightbeast-swz105";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class NightBeastSSPAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += NightBeastSSPPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= NightBeastSSPPilotAbility;
        }

        protected void NightBeastSSPPilotAbility(GenericShip ship)
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
