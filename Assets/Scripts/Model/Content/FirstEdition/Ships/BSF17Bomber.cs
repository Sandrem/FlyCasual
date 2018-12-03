using System.Collections;
using System.Collections.Generic;
using Movement;
using Actions;
using ActionsList;
using Arcs;
using Upgrade;

namespace Ship.FirstEdition.BSF17Bomber
{
    public class BSF17Bomber : GenericShip
    {
        public BSF17Bomber() : base()
        {
            ShipInfo = new ShipCardInfo
            (
                "B/SF-17 Bomber",
                BaseSize.Large,
                Faction.Rebel,
                new ShipArcsInfo(
                    new ShipArcInfo(ArcType.Front, 2),
                    new ShipArcInfo(ArcType.TurretPrimaryWeapon, 2)
                ),
                1, 9, 3,
                new ShipActionsInfo(
                    new ActionInfo(typeof(FocusAction)),
                    new ActionInfo(typeof(TargetLockAction))
                ),
                new ShipUpgradesInfo(
                    UpgradeType.Title,
                    UpgradeType.Modification,
                    UpgradeType.System,
                    UpgradeType.Tech,
                    UpgradeType.Bomb,
                    UpgradeType.Bomb
                ),
                subFaction: Faction.Resistance
            );

            IconicPilots = new Dictionary<Faction, System.Type> {
                { Faction.Rebel, typeof(CrimsonSquadronPilot) }
            };

            ModelInfo = new ShipModelInfo(
                "B/SF-17 Bomber",
                "Crimson"
            );

            DialInfo = new ShipDialInfo(
                new ManeuverInfo(ManeuverSpeed.Speed0, ManeuverDirection.Stationary, ManeuverBearing.Stationary, MovementComplexity.Complex),

                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Complex),

                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal)
            );

            SoundInfo = new ShipSoundInfo(
                new List<string>()
                {
                    "Falcon-Fly1",
                    "Falcon-Fly2",
                    "Falcon-Fly3"
                },
                "Falcon-Fire", 2
            );

            ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/0/04/20171223_152404.jpg";

            HotacManeuverTable = new AI.BSF17BomberTable();
        }
    }
}
