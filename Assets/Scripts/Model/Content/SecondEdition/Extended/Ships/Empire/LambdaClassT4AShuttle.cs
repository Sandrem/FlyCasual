using System.Collections.Generic;
using ActionsList;
using Actions;
using Arcs;
using Ship;
using Ship.CardInfo;
using UnityEngine;
using Movement;

namespace Ship
{
    namespace SecondEdition.LambdaClassT4AShuttle
    {
        public class LambdaClassT4AShuttle : GenericShip
        {
            public LambdaClassT4AShuttle() : base()
            {
                ShipInfo = new ShipCardInfo25
                (
                    "Lambda-class T-4a Shuttle",
                    BaseSize.Large,
                    new FactionData
                    (
                        new Dictionary<Faction, System.Type>
                        {
                            { Faction.Imperial, typeof(CaptainKagi) }
                        }
                    ),
                    new ShipArcsInfo
                    (
                        new ShipArcInfo(ArcType.Front, 3),
                        new ShipArcInfo(ArcType.Rear, 2)
                    ),
                    1, 6, 4,
                    new ShipActionsInfo
                    (
                        new ActionInfo(typeof(FocusAction)),
                        new ActionInfo(typeof(ReinforceAction)),
                        new ActionInfo(typeof(CoordinateAction)),
                        new ActionInfo(typeof(JamAction), ActionColor.Red)
                    ),
                    new ShipUpgradesInfo(),
                    legality: new List<Content.Legality>() { Content.Legality.ExtendedLegal }
                );

                ModelInfo = new ShipModelInfo
                (
                    "Lambda-class Shuttle",
                    "Default",
                    new Vector3(-4.3f, 8.4f, 5.55f),
                    4f
                );

                DialInfo = new ShipDialInfo(
                    new ManeuverInfo(ManeuverSpeed.Speed0, ManeuverDirection.Stationary, ManeuverBearing.Stationary, MovementComplexity.Complex),

                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),

                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Complex),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Complex),

                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Complex),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Complex)
                );

                SoundInfo = new ShipSoundInfo(
                    new List<string>()
                    {
                        "Slave1-Fly1",
                        "Slave1-Fly2"
                    },
                    "Slave1-Fire", 3
                );

                ShipIconLetter = 'l';
            }
        }
    }
}