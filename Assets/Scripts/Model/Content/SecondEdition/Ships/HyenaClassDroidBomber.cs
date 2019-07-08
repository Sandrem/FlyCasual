﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Actions;
using ActionsList;
using Arcs;
using Movement;
using Ship;
using SubPhases;
using Upgrade;
using UnityEngine;

namespace Ship.SecondEdition.HyenaClassDroidBomber
{
    public class HyenaClassDroidBomber : GenericShip
    {
        public HyenaClassDroidBomber() : base()
        {
            ShipInfo = new ShipCardInfo
            (
                "Hyena-class Droid Bomber",
                BaseSize.Small,
                Faction.Separatists,
                new ShipArcsInfo(ArcType.Front, 2), 2, 5, 0,
                new ShipActionsInfo(
                    new ActionInfo(typeof(CalculateAction)),
                    new ActionInfo(typeof(TargetLockAction)),
                    new ActionInfo(typeof(BarrelRollAction)),
                    new ActionInfo(typeof(ReloadAction), ActionColor.Red)
                ),
                new ShipUpgradesInfo(
                    UpgradeType.Modification,
                    UpgradeType.Configuration
                ),
                abilityText: "<b>Networked Calculations:</b> While you defend or perform an attack, you may spend 1 calculate token from a friendly ship at range 0-1 to change 1 eye result to evade or hit result."
            );

            ShipInfo.ActionIcons.AddLinkedAction(
                new LinkedActionInfo(typeof(BarrelRollAction), typeof(TargetLockAction), ActionColor.Red)
            );

            ShipAbilities.Add(new Abilities.SecondEdition.NetworkedCalculationsAbility());

            IconicPilots = new Dictionary<Faction, System.Type> {
                { Faction.Separatists, typeof(SeparatistBomber) }
            };

            ModelInfo = new ShipModelInfo(
                "Hyena-class Droid Bomber",
                "Gray",
                new Vector3(-3.75f, 7.95f, 5.55f),
                1f
            );

            DialInfo = new ShipDialInfo(
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.TallonRoll, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.TallonRoll, MovementComplexity.Complex),

                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed5, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Complex)
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
                "TIE-Fire", 2
            );

            // ManeuversImageUrl
        }
    }
}