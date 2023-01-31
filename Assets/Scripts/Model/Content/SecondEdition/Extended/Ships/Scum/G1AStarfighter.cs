using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using Editions;
using Upgrade;
using Actions;
using Arcs;
using Ship.CardInfo;

namespace Ship
{
    namespace SecondEdition.G1AStarfighter
    {
        public class G1AStarfighter : GenericShip
        {
            public G1AStarfighter() : base()
            {
                ShipInfo = new ShipCardInfo25
                (
                    "G-1A Starfighter",
                    BaseSize.Medium,
                    new FactionData
                    (
                        new Dictionary<Faction, System.Type>
                        {
                            { Faction.Scum, typeof(Zuckuss) }
                        }
                    ),
                    new ShipArcsInfo(ArcType.Front, 3), 1, 5, 4,
                    new ShipActionsInfo
                    (
                        new ActionInfo(typeof(FocusAction)),
                        new ActionInfo(typeof(TargetLockAction)),
                        new ActionInfo(typeof(JamAction))
                    ),
                    new ShipUpgradesInfo(),
                    legality: new List<Content.Legality>() { Content.Legality.ExtendedLegal }
                );

                ModelInfo = new ShipModelInfo
                (
                    "G-1A Starfighter",
                    "G-1A Starfighter",
                    new Vector3(-3.7f, 7.84f, 5.55f),
                    1.75f
                );

                DialInfo = new ShipDialInfo
                (
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
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn, MovementComplexity.Complex),

                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Complex),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Complex),

                    new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Complex),
                    new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn, MovementComplexity.Complex)
                );

                SoundInfo = new ShipSoundInfo
                (
                    new List<string>()
                    {
                        "XWing-Fly1",
                        "XWing-Fly2",
                        "XWing-Fly3"
                    },
                    "XWing-Laser", 3
                );

                ShipIconLetter = 'n';
            }
        }
    }
}