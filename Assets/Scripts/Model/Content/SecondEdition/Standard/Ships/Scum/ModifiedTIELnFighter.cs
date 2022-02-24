using Actions;
using ActionsList;
using Arcs;
using Movement;
using Ship.CardInfo;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace SecondEdition.ModifiedTIELnFighter
    {
        public class ModifiedTIELnFighter : GenericShip
        {
            public ModifiedTIELnFighter() : base()
            {
                ShipInfo = new ShipCardInfo25
                (
                    "Modified TIE/ln Fighter",
                    BaseSize.Small,
                    new FactionData
                    (
                        new Dictionary<Faction, Type>
                        {
                            { Faction.Scum, typeof(CaptainSeevor) }
                        }
                    ),
                    new ShipArcsInfo(ArcType.Front, 2), 3, 3, 0,
                    new ShipActionsInfo
                    (
                        new ActionInfo(typeof(FocusAction)),
                        new ActionInfo(typeof(EvadeAction)),
                        new ActionInfo(typeof(BarrelRollAction))
                    ),
                    new ShipUpgradesInfo()
                );

                ShipAbilities.Add(new Abilities.SecondEdition.ModifiedTIELnFighterAbility());

                ModelInfo = new ShipModelInfo
                (
                    "Modified TIE Fighter",
                    "Mining Guild Yellow",
                    new Vector3(-3.7f, 7.8f, 5.55f),
                    1.75f
                );

                DialInfo = new ShipDialInfo
                (
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

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
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn, MovementComplexity.Complex),

                    new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),

                    new ManeuverInfo(ManeuverSpeed.Speed5, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Complex)
                );

                SoundInfo = new ShipSoundInfo
                (
                    new List<string>()
                    {
                        "TIE-Fly1",
                        "TIE-Fly2",
                        "TIE-Fly3",
                        "TIE-Fly4",
                        "TIE-Fly5",
                        "TIE-Fly6",
                        "TIE-Fly7"
                    },
                    "TIE-Fire", 2
                );

                ShipIconLetter = 'C';
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ModifiedTIELnFighterAbility : GenericAbility
    {
        public override string Name { get { return "Notched Stabilizers"; } }

        public override void ActivateAbility()
        {
            HostShip.IgnoreObstacleTypes.Add(typeof(Obstacles.Asteroid));
        }

        public override void DeactivateAbility()
        {
            HostShip.IgnoreObstacleTypes.Remove(typeof(Obstacles.Asteroid));
        }
    }
}
