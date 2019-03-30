using System.Collections;
using System.Collections.Generic;
using Movement;
using Actions;
using ActionsList;
using Arcs;
using Upgrade;
using Mods;
using Mods.ModsList;

namespace Ship.FirstEdition.UWing
{
    public class UWing : GenericShip, IMovableWings
    {
        public WingsPositions CurrentWingsPosition { get; set; }

        public UWing() : base()
        {
            ShipInfo = new ShipCardInfo
            (
                "U-wing",
                (ModsManager.Mods[typeof(UWingSmallBaseMod)].IsOn) ? BaseSize.Small : BaseSize.Large,
                Faction.Rebel,
                new ShipArcsInfo(ArcType.Front, 3), 1, 4, 4,
                new ShipActionsInfo(
                    new ActionInfo(typeof(FocusAction)),
                    new ActionInfo(typeof(TargetLockAction))
                ),
                new ShipUpgradesInfo(
                    UpgradeType.Title,
                    UpgradeType.Modification,
                    UpgradeType.System,
                    UpgradeType.Crew,
                    UpgradeType.Crew
                ),
                icon: 'u',
                description: "The UT-60D U-wing starfighter/support craft also known as the UT-60D, or U-wing, was a transport/gunship model manufactured by Incom Corporation and used by the Alliance to Restore the Republic during the Galactic Civil War. Used to drop troops into battle, and provide cover fire for them, U-wings were pivotal in transport and protection of the Rebel Alliance's ground forces during the Battle of Scarif."
            );

            IconicPilots = new Dictionary<Faction, System.Type> {
                { Faction.Rebel, typeof(BlueSquadronPathfinder) }
            };

            ModelInfo = new ShipModelInfo(
                "U-wing",
                "Blue Squadron",
                wingsPositions: WingsPositions.Closed
            );

            DialInfo = new ShipDialInfo(
                new ManeuverInfo(ManeuverSpeed.Speed0, ManeuverDirection.Stationary, ManeuverBearing.Stationary, MovementComplexity.Complex),

                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),

                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal)
            );

            SoundInfo = new ShipSoundInfo(
                new List<string>()
                {
                    "Falcon-Fly1",
                    "Falcon-Fly2",
                    "Falcon-Fly3"
                },
                "Falcon-Fire", 3
            );

            ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/c/c5/MR_U-WING.png";

            HotacManeuverTable = new AI.UWingTable();

            CurrentWingsPosition = WingsPositions.Closed;
        }
    }
}
