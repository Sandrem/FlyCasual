using System.Collections.Generic;
using Actions;
using ActionsList;
using Arcs;
using Movement;
using UnityEngine;
using Upgrade;

namespace Ship.SecondEdition.TIEWiWhisperModifiedInterceptor
{
    public class TIEWiWhisperModifiedInterceptor : GenericShip, TIE
    {
        public TIEWiWhisperModifiedInterceptor() : base()
        {
            ShipInfo = new ShipCardInfo
            (
                "TIE/wi Whisper Modified Interceptor",
                BaseSize.Small,
                Faction.FirstOrder,
                new ShipArcsInfo(ArcType.SingleTurret, 2), 2, 3, 2,
                new ShipActionsInfo
                (
                    new ActionInfo(typeof(FocusAction)),
                    new ActionInfo(typeof(EvadeAction)),
                    new ActionInfo(typeof(TargetLockAction)),
                    new ActionInfo(typeof(BarrelRollAction)),
                    new ActionInfo(typeof(BoostAction))
                ),
                new ShipUpgradesInfo
                (
                    UpgradeType.Missile,
                    UpgradeType.Tech,
                    UpgradeType.Configuration     
                )
            );

            ShipInfo.ArcInfo.Arcs.Add(new ShipArcInfo(ArcType.Bullseye, 3));

            ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(FocusAction),      typeof(RotateArcAction), ActionColor.White));
            ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(BarrelRollAction), typeof(RotateArcAction), ActionColor.White));
            ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(BoostAction),      typeof(RotateArcAction), ActionColor.White));

            ShipAbilities.Add(new Abilities.SecondEdition.HeavyWeaponTurret());

            IconicPilots = new Dictionary<Faction, System.Type>
            {
                { Faction.FirstOrder, typeof(P709thLegionAce) }
            };

            ModelInfo = new ShipModelInfo(
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

            SoundInfo = new ShipSoundInfo(
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