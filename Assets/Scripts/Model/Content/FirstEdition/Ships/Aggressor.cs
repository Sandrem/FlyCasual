using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Actions;
using Arcs;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.Aggressor
    {
        public class Aggressor : GenericShip
        {
            public Aggressor() : base()
            {
                ShipInfo = new ShipCardInfo
                (
                    "Aggressor",
                    BaseSize.Large,
                    Faction.Scum,
                    new ShipArcsInfo(ArcTypes.Primary, 3), 3, 4, 4,
                    new ShipActionsInfo(
                        new ActionInfo(typeof(FocusAction)),
                        new ActionInfo(typeof(TargetLockAction)),
                        new ActionInfo(typeof(EvadeAction)),
                        new ActionInfo(typeof(BoostAction))
                    ),
                    new ShipUpgradesInfo(
                        UpgradeType.Title,
                        UpgradeType.Modification,
                        UpgradeType.System,
                        UpgradeType.Cannon,
                        UpgradeType.Cannon,
                        UpgradeType.Bomb,
                        UpgradeType.Illicit
                    )
                );

                IconicPilots = new Dictionary<Faction, System.Type> {
                    { Faction.Scum, typeof(IG88B) }
                };

                ModelInfo = new ShipModelInfo(
                    "Aggressor",
                    "Blue"
                );

                DialInfo = new ShipDialInfo(
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.SegnorsLoop, MovementComplexity.Complex),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.SegnorsLoop, MovementComplexity.Complex),

                    new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn, MovementComplexity.Complex)
                );

                SoundInfo = new ShipSoundInfo(
                    new List<string>()
                    {
                        "Slave1-Fly1",
                        "Slave1-Fly2"
                    },
                    "Slave1-Fire", 3
                );

                // ManeuversImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/fe/d9/fed9939b-8331-462b-a3b8-d8359d1342bd/swx75_a3_dial.png";

                /* HotacManeuverTable = new AI.TIEFighterTable(); */
            }
        }
    }
}
