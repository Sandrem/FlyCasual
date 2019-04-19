using System;
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

namespace Ship.SecondEdition.NantexClassStarfighter
{
    public class NantexClassStarfighter : GenericShip
    {
        public NantexClassStarfighter() : base()
        {
            RequiredMods = new List<System.Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };

            ShipInfo = new ShipCardInfo
            (
                "Nantex-class Starfighter",
                BaseSize.Small,
                Faction.Separatists,
                new ShipArcsInfo(ArcType.Bullseye, 3), 2, 5, 0,
                new ShipActionsInfo(
                    new ActionInfo(typeof(FocusAction)),
                    new ActionInfo(typeof(EvadeAction))
                ),
                new ShipUpgradesInfo(
                    UpgradeType.Title,
                    UpgradeType.Modification,
                    UpgradeType.Configuration
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
                "Default"
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
            AskToUseAbility(
                NeverUseByDefault,
                AgreeToAssignToken,
                infoText: "Do you want to gain Tractor token to rotate your mobile arc?"
            );
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
            new RotateArcAction().DoOnlyEffect(Triggers.FinishTrigger);
        }
    }
}