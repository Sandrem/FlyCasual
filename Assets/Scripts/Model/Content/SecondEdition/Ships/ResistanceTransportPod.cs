using System.Collections;
using System.Collections.Generic;
using Actions;
using ActionsList;
using Arcs;
using Movement;
using Upgrade;

namespace Ship.SecondEdition.ResistanceTransportPod
{
    public class ResistanceTransportPod : GenericShip
    {
        public ResistanceTransportPod() : base()
        {
            RequiredMods = new List<System.Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };

            ShipInfo = new ShipCardInfo
            (
                "Resistance Transport Pod",
                BaseSize.Small,
                Faction.Resistance,
                new ShipArcsInfo(ArcType.Front, 2), 2, 3, 1,
                new ShipActionsInfo(
                    new ActionInfo(typeof(FocusAction)),
                    new ActionInfo(typeof(TargetLockAction), ActionColor.Red),
                    new ActionInfo(typeof(BarrelRollAction), ActionColor.Red),
                    new ActionInfo(typeof(JamAction), ActionColor.Red)
                ),
                new ShipUpgradesInfo(
                    UpgradeType.Title,
                    UpgradeType.Crew,
                    UpgradeType.Astromech,
                    UpgradeType.Modification
                )
            );

            IconicPilots = new Dictionary<Faction, System.Type> {
                { Faction.Resistance, typeof(RoseTico) }
            };

            ModelInfo = new ShipModelInfo(
                "Resistance Transport Pod",
                "Default"
            );

            DialInfo = new ShipDialInfo(
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

                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Complex),

                new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Complex)
            );

            SoundInfo = new ShipSoundInfo(
                new List<string>()
                {
                    "XWing-Fly1",
                    "XWing-Fly2",
                    "XWing-Fly3"
                },
                "XWing-Laser", 3
            );

            //ManeuversImageUrl
        }
    }
}
