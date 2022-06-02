using ActionsList;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T70XWing
    {
        public class TemminWexley : T70XWing
        {
            public TemminWexley() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Temmin Wexley",
                    "Black Two",
                    Faction.Resistance,
                    4,
                    6,
                    17,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.SnapWexleyAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Tech
                    },
                    tags: new List<Tags>
                    {
                        Tags.XWing
                    }
                );

                ImageUrl = "https://squadbuilder.fantasyflightgames.com/card_images/en/bb98b7ea3a580542b586a9999fd352c9.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you execute a 2-, 3-, or 4-speed maneuver, if you are not touching a ship, you may perform a free boost action.
    public class SnapWexleyAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinish += RegisterSnapAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinish -= RegisterSnapAbility;
        }

        private void RegisterSnapAbility(GenericShip hostShip)
        {
            if (Selection.ThisShip.IsBumped) return;

            int assignedSpeed = HostShip.AssignedManeuver.Speed;
            if (assignedSpeed < 2 || assignedSpeed > 4) return;

            if (BoardTools.Board.IsOffTheBoard(hostShip)) return;

            RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, PerformFreeBoost);
        }

        private void PerformFreeBoost(object sender, EventArgs e)
        {
            HostShip.AskPerformFreeAction(
                new List<GenericAction> { new BoostAction() },
                Triggers.FinishTrigger,
                HostShip.PilotInfo.PilotName,
                "After you execute a 2-, 3- or 4-speed maneuver, if you are not touching a ship, you may perform a free boost action",
                HostShip
            );
        }
    }
}
