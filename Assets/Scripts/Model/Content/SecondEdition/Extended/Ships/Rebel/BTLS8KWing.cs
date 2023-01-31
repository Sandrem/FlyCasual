using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Upgrade;
using Actions;
using Arcs;
using Ship.CardInfo;
using UnityEngine;

namespace Ship
{
    namespace SecondEdition.BTLS8KWing
    {
        public class BTLS8KWing : GenericShip
        {
            public BTLS8KWing() : base()
            {
                ShipInfo = new ShipCardInfo25
                (
                    "BTL-S8 K-wing",
                    BaseSize.Medium,
                    new FactionData
                    (
                        new Dictionary<Faction, System.Type>
                        {
                            { Faction.Rebel, typeof(MirandaDoni) }
                        }
                    ),
                    new ShipArcsInfo
                    (
                        new ShipArcInfo(ArcType.DoubleTurret, 2)
                    ),
                    1, 6, 3,
                    new ShipActionsInfo
                    (
                        new ActionInfo(typeof(FocusAction)),
                        new ActionInfo(typeof(TargetLockAction)),
                        new ActionInfo(typeof(SlamAction)),
                        new ActionInfo(typeof(ReloadAction)),
                        new ActionInfo(typeof(RotateArcAction))
                    ),
                    new ShipUpgradesInfo(),
                    legality: new List<Content.Legality>() { Content.Legality.ExtendedLegal }
                );

                ModelInfo = new ShipModelInfo
                (
                    "K-wing",
                    "Red",
                    new Vector3(-3.55f, 7.3f, 5.55f),
                    3f
                );

                DialInfo = new ShipDialInfo
                (
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),

                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal)
                );

                SoundInfo = new ShipSoundInfo
                (
                    new List<string>()
                    {
                        "YWing-Fly1",
                        "YWing-Fly2"
                    },
                    "XWing-Laser", 2
                );

                ShipIconLetter = 'k';
            }
        }
    }
}
