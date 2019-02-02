using System.Collections;
using System.Collections.Generic;
using Movement;
using Actions;
using ActionsList;
using Arcs;
using Upgrade;

namespace Ship.FirstEdition.AuzituckGunship
{
    public class AuzituckGunship : GenericShip
    {
        public AuzituckGunship() : base()
        {
            ShipInfo = new ShipCardInfo
            (
                "Auzituck Gunship",
                BaseSize.Small,
                Faction.Rebel,
                new ShipArcsInfo(
                    new ShipArcInfo(ArcType.Front, 3),
                    new ShipArcInfo(ArcType.FullFront, 3)
                ),
                1, 6, 3,
                new ShipActionsInfo(
                    new ActionInfo(typeof(FocusAction)),
                    new ActionInfo(typeof(ReinforceAction))
                ),
                new ShipUpgradesInfo(
                    UpgradeType.Title,
                    UpgradeType.Modification,
                    UpgradeType.Crew,
                    UpgradeType.Crew
                )
            );

            IconicPilots = new Dictionary<Faction, System.Type> {
                { Faction.Rebel, typeof(Lowhhrick) }
            };

            ModelInfo = new ShipModelInfo(
                "Auzituck Gunship",
                "Kashyyyk Defender"
            );

            DialInfo = new ShipDialInfo(
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),

                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed5, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Complex)
            );

            SoundInfo = new ShipSoundInfo(
                new List<string>()
                {
                    "XWing-Fly1",
                    "XWing-Fly2",
                    "XWing-Fly3"
                },
                "XWing-Laser", 2
            );

            // ManeuversImageUrl = "https://vignette1.wikia.nocookie.net/xwing-miniatures/images/3/3d/MR_T65-X-WING.png";

            HotacManeuverTable = new AI.AuzituckGunshipTable();
        }
    }
}
