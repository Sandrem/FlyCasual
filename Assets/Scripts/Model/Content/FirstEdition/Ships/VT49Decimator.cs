using System.Collections;
using System.Collections.Generic;
using Movement;
using Actions;
using ActionsList;
using Arcs;
using Upgrade;

namespace Ship.FirstEdition.VT49Decimator
{
    public class VT49Decimator : GenericShip
    {
        public VT49Decimator() : base()
        {
            ShipInfo = new ShipCardInfo
            (
                "VT-49 Decimator",
                BaseSize.Large,
                Faction.Imperial,
                new ShipArcsInfo(ArcTypes.Primary, 3), 0, 12, 4,
                new ShipActionsInfo(
                    new ActionInfo(typeof(FocusAction)),
                    new ActionInfo(typeof(TargetLockAction))
                ),
                new ShipUpgradesInfo(
                    UpgradeType.Torpedo,
                    UpgradeType.Crew,
                    UpgradeType.Crew,
                    UpgradeType.Crew,
                    UpgradeType.Bomb
                )
            );

            IconicPilots = new Dictionary<Faction, System.Type> {
                { Faction.Imperial, typeof(PatrolLeader) }
            };

            ModelInfo = new ShipModelInfo(
                "VT-49 Decimator",
                "Gray"
            );

            DialInfo = new ShipDialInfo(
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal)
            );

            SoundInfo = new ShipSoundInfo(
                new List<string>()
                {
                    "Slave1-Fly1",
                    "Slave1-Fly2"
                },
                "Slave1-Fire", 3
            );

            // ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/c/c5/MR_U-WING.png";

            HotacManeuverTable = new AI.VT49DecimatorTable();
        }
    }
}
