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
using UnityEngine;
using Upgrade;

namespace Ship.SecondEdition.NantexClassStarfighter
{
    public class NantexClassStarfighter : GenericShip
    {
        public NantexClassStarfighter() : base()
        {
            ShipInfo = new ShipCardInfo
            (
                "Nantex-class Starfighter",
                BaseSize.Small,
                Faction.Separatists,
                new ShipArcsInfo(ArcType.Bullseye, 3), 3, 4, 0,
                new ShipActionsInfo(
                    new ActionInfo(typeof(FocusAction)),
                    new ActionInfo(typeof(EvadeAction))
                ),
                new ShipUpgradesInfo(
                    UpgradeType.Title,
                    UpgradeType.Talent
                ),
                abilityText: "<b>Pinpoint Tractor Array:</b> You cannot rotate your turret indicator in your rear sector. After you execute a maneuver, you main gain 1 tractor token to perform a rotate turret indicator action."
            );

            ShipInfo.ArcInfo.Arcs.Add(new ShipArcInfo(ArcType.SingleTurret, 2));

            ShipAbilities.Add(new Abilities.SecondEdition.PinpointTractorArray());

            IconicPilots = new Dictionary<Faction, System.Type> {
                { Faction.Separatists, typeof(StalgasinHiveGuard) }
            };

            ModelInfo = new ShipModelInfo(
                "Nantex-class Starfighter",
                "Default",
                new Vector3(-3.8f, 7.9f, 5.55f),
                0.75f
            );

            DialInfo = new ShipDialInfo(
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
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

                new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed5, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
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
                "TIE-Fire", 2
            );

            // ManeuversImageUrl
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PinpointTractorArray : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGetAvailableArcFacings += RestrictArcFacings;
            HostShip.OnMovementFinish += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetAvailableArcFacings -= RestrictArcFacings;
            HostShip.OnMovementFinish -= RegisterAbility;
        }

        private void RestrictArcFacings(List<ArcFacing> facings)
        {
            facings.Remove(ArcFacing.Rear);
        }

        private void RegisterAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskToAssignTractor);
        }

        private void AskToAssignTractor(object sender, System.EventArgs e)
        {
            //make sure host ship didn't die from Loose Stabilizer, hitting rocks, etc.
            if (HostShip.IsDestroyed) Triggers.FinishTrigger();
            else
            {
                AskToUseAbility(
                    "Pinpoint Tractor Array",
                    NeverUseByDefault,
                    AgreeToAssignToken,
                    descriptionLong: "Do you want to gain Tractor Token to perform a Rotate action?"
                );
            }
        }

        private void AgreeToAssignToken(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostShip.Tokens.AssignToken(
                new Tokens.TractorBeamToken(HostShip, HostShip.Owner),
                RotateArc
            );
        }

        private void RotateArc()
        {
            HostShip.AskPerformFreeAction(
                new RotateArcAction(),
                Triggers.FinishTrigger,
                descriptionShort: "Pinpoint Tractor Array",
                descriptionLong: "You must perform Rotate action",
                isForced: true
            );
        }
    }
}