using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Actions;
using Arcs;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.AlphaClassStarWing
    {
        public class AlphaClassStarWing : GenericShip
        {

            public AlphaClassStarWing() : base()
            {
                ShipInfo = new ShipCardInfo
                (
                    "Alpha-class Star Wing",
                    BaseSize.Small,
                    Faction.Imperial,
                    new ShipArcsInfo(ArcType.Front, 2), 2, 4, 3,
                    new ShipActionsInfo(
                        new ActionInfo(typeof(FocusAction)),
                        new ActionInfo(typeof(TargetLockAction)),
                        new ActionInfo(typeof(SlamAction)),
                        new ActionInfo(typeof(ReloadAction))
                    ),
                    new ShipUpgradesInfo(
                        UpgradeType.Title,
                        UpgradeType.Torpedo,
                        UpgradeType.Missile,
                        UpgradeType.Modification
                    )
                );

                IconicPilots = new Dictionary<Faction, System.Type> {
                    { Faction.Imperial, typeof(MajorVynder) }
                };

                ModelInfo = new ShipModelInfo(
                    "Alpha-class Star Wing",
                    "Gray"
                );

                DialInfo = new ShipDialInfo(
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),

                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                    new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Complex)
                );

                SoundInfo = new ShipSoundInfo(
                    new List<string>()
                    {
                        "Slave1-Fly1",
                        "Slave1-Fly2"
                    },
                    "Slave1-Fire", 2
                );

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/d/d9/20171223_151405.jpg";

                HotacManeuverTable = new AI.AlphaClassStarWingTable();
            }
        }
    }
}
