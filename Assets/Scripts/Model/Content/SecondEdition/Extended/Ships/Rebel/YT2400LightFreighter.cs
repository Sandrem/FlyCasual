using System.Collections;
using System.Collections.Generic;
using Actions;
using ActionsList;
using Arcs;
using Movement;
using Ship.CardInfo;
using UnityEngine;
using Upgrade;

namespace Ship.SecondEdition.YT2400LightFreighter
{
    public class YT2400LightFreighter : GenericShip
    {
        public YT2400LightFreighter() : base()
        {
            ShipInfo = new ShipCardInfo25
            (
                "YT-2400 Light Freighter",
                BaseSize.Large,
                new FactionData
                (
                    new Dictionary<Faction, System.Type>
                    {
                        { Faction.Rebel, typeof(DashRendar) }
                    }
                ),
                new ShipArcsInfo(ArcType.DoubleTurret, 4), 2, 6, 4,
                new ShipActionsInfo
                (
                    new ActionInfo(typeof(FocusAction)),
                    new ActionInfo(typeof(TargetLockAction)),
                    new ActionInfo(typeof(BarrelRollAction), ActionColor.Red),
                    new ActionInfo(typeof(RotateArcAction))
                ),
                new ShipUpgradesInfo(),
                legality: new List<Content.Legality>() { Content.Legality.ExtendedLegal }
            );

            ModelInfo = new ShipModelInfo
            (
                "YT-2400",
                "Red",
                new Vector3(-3.35f, 8.2f, 5.55f),
                3.25f
            );

            DialInfo = new ShipDialInfo
            (
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn, MovementComplexity.Complex)
            );

            SoundInfo = new ShipSoundInfo
            (
                new List<string>()
                {
                    "Falcon-Fly1",
                    "Falcon-Fly2",
                    "Falcon-Fly3"
                },
                "Falcon-Fire", 2
            );

            ShipIconLetter = 'o';

            ShipAbilities.Add(new Abilities.SecondEdition.SensorBlindspot());
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SensorBlindspot : GenericAbility
    {
        public override string Name { get { return "Sensor Blindspot"; } }

        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckSensorBlindspot;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= CheckSensorBlindspot;
        }

        private void CheckSensorBlindspot(ref int count)
        {
            if (Combat.ChosenWeapon.WeaponType == Ship.WeaponTypes.PrimaryWeapon && Combat.ShotInfo.Range < 2) count -= 2;
        }
    }
}
