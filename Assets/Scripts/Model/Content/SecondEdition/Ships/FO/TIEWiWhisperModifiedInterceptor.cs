using Actions;
using ActionsList;
using Arcs;
using Movement;
using Ship.CardInfo;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ship.SecondEdition.TIEWiWhisperModifiedInterceptor
{
    public class TIEWiWhisperModifiedInterceptor : GenericShip
    {
        public TIEWiWhisperModifiedInterceptor() : base()
        {
            ShipInfo = new ShipCardInfo25
            (
                "TIE/wi Whisper Modified Interceptor",
                BaseSize.Small,
                new FactionData
                (
                    new Dictionary<Faction, Type>
                    {
                        { Faction.FirstOrder, typeof(P709thLegionAce) }
                    }
                ),
                new ShipArcsInfo
                (
                    new ShipArcInfo(ArcType.SingleTurret, 2),
                    new ShipArcInfo(ArcType.Bullseye, 3)
                ),
                2, 3, 2,
                new ShipActionsInfo
                (
                    new ActionInfo(typeof(FocusAction)),
                    new ActionInfo(typeof(EvadeAction)),
                    new ActionInfo(typeof(TargetLockAction)),
                    new ActionInfo(typeof(BarrelRollAction)),
                    new ActionInfo(typeof(BoostAction))
                ),
                new ShipUpgradesInfo(),
                linkedActions: new List<LinkedActionInfo>
                {
                    new LinkedActionInfo(typeof(FocusAction), typeof(RotateArcAction), ActionColor.White),
                    new LinkedActionInfo(typeof(BarrelRollAction), typeof(RotateArcAction), ActionColor.White),
                    new LinkedActionInfo(typeof(BoostAction), typeof(RotateArcAction), ActionColor.White)
                }
            );

            ShipAbilities.Add(new Abilities.SecondEdition.HeavyWeaponTurret());

            ModelInfo = new ShipModelInfo
            (
                "TIE Wi Whisper Modified Interceptor",
                "First Order",
                new Vector3(-3.4f, 7.5f, 5.55f),
                2f
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
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.SegnorsLoop, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.SegnorsLoop, MovementComplexity.Complex),

                new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn, MovementComplexity.Complex),

                new ManeuverInfo(ManeuverSpeed.Speed5, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed5, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn, MovementComplexity.Complex)
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
                "TIE-Fire", 3
            );

            ShipIconLetter = ' '; //TODO
        }
    }
}