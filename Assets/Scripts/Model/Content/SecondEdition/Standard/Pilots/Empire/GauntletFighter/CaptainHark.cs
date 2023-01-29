using Content;
using Movement;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.GauntletFighter
    {
        public class CaptainHark : GauntletFighter
        {
            public CaptainHark() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Captain Hark",
                    "Obedient Underling",
                    Faction.Imperial,
                    3,
                    7,
                    15,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.CaptainHarkAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Modification,
                        UpgradeType.Configuration
                    },
                    tags: new List<Tags>()
                    {
                        Tags.Mandalorian 
                    },
                    skinName: "Gray"
                );

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/1/10/Captainhark.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CaptainHarkAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnSetupPlaced += ReplaceSwivelWing;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnSetupPlaced -= ReplaceSwivelWing;
        }

        private void ReplaceSwivelWing(GenericShip ship)
        {
            GenericUpgrade SwivelWingUpgrade = GetSwivelWing();
            if (SwivelWingUpgrade != null)
            {
                SwivelWingUpgrade.DeactivateAbility();
                GenericUpgrade captainHarkSwivelWing = new UpgradesList.SecondEdition.SwivelWingHarkDown();
                SwivelWingUpgrade.ReplaceUpgradeBy(captainHarkSwivelWing);
            }
        }

        private GenericUpgrade GetSwivelWing()
        {
            return HostShip.UpgradeBar.GetUpgradesAll().Find(n => n.UpgradeInfo.HasType(UpgradeType.Configuration));
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SwivelWingHarkDownAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsRevealed += RegisterAskToRotate;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsRevealed -= RegisterAskToRotate;
        }

        protected void RegisterAskToRotate(GenericShip ship)
        {
            if (ship.AssignedManeuver.Bearing == Movement.ManeuverBearing.Stationary)
            {
                RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, AskToRotate);
            }
        }

        protected void AskToRotate(object sender, EventArgs e)
        {
            SwivelWingDecisionSubphase subphase = Phases.StartTemporarySubPhaseNew<SwivelWingDecisionSubphase>(
                "Choose Wich Direction to Sideslip",
                delegate
                {

                    Triggers.FinishTrigger();
                }
             );

            subphase.DescriptionShort = "Sideslip";
            subphase.DescriptionLong = "Choose Wich Direction to Sideslip";
            subphase.ImageSource = HostShip;

            subphase.AddDecision("Left", DoSideSlipLeft);
            subphase.AddDecision("Right", DoSideSlipRight);
            //HostShip.WingsOpen();

            subphase.Start();
        }

        private void DoSideSlipRight(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            GenericMovement movement = new SideslipBankMovement(
                1,
                ManeuverDirection.Right,
                ManeuverBearing.SideslipBank,
                GenericMovement.IncreaseComplexity(HostShip.RevealedManeuver.ColorComplexity)
            );

            Messages.ShowInfo($"{HostShip.PilotInfo.PilotName}: Maneuver is changed to Sideslip");
            HostShip.SetAssignedManeuver(movement);

            (HostUpgrade as GenericDualUpgrade).Flip();

            Triggers.FinishTrigger();
        }

        private void DoSideSlipLeft(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            GenericMovement movement = new SideslipBankMovement(
                1,
                ManeuverDirection.Left,
                ManeuverBearing.SideslipBank,
                GenericMovement.IncreaseComplexity(HostShip.RevealedManeuver.ColorComplexity)
            );

            Messages.ShowInfo($"{HostShip.PilotInfo.PilotName}: Maneuver is changed to Sideslip");
            HostShip.SetAssignedManeuver(movement);

            (HostUpgrade as GenericDualUpgrade).Flip();

            Triggers.FinishTrigger();
        }

        private class SwivelWingDecisionSubphase : DecisionSubPhase { };
    }

    public class SwivelWingHarkUpAbility : SwivelWingUpAbility
    {


        protected override void RegisterAskToUseFlip(GenericShip ship)
        {
            if (BoardTools.Board.IsOffTheBoard(ship)) return;

            if (ship.AssignedManeuver.Bearing == Movement.ManeuverBearing.SideslipBank) return;

            RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskToFlip);
        }


    }
}

namespace UpgradesList.SecondEdition
{
    public class SwivelWingHarkDown : GenericDualUpgrade
    {
        public SwivelWingHarkDown() : base()
        {
            IsHidden = true;

            UpgradeInfo = new UpgradeCardInfo(
                "Swivel Wing (Down)",
                UpgradeType.Configuration,
                cost: 0,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.GauntletFighter.GauntletFighter)),
                abilityType: typeof(Abilities.SecondEdition.SwivelWingHarkDownAbility)
            );

            SelectSideOnSetup = false;

            NameCanonical = "swivelwingdownhark";

            ImageUrl = "https://infinitearenas.com/xw2/images/upgrades/swivelwing.png";

            AnotherSide = typeof(SwivelWingHarkUp);
        }
    }

    public class SwivelWingHarkUp : GenericDualUpgrade
    {
        public SwivelWingHarkUp() : base()
        {
            IsHidden = true;
            NameCanonical = "SwivelWing-anotherside";

            UpgradeInfo = new UpgradeCardInfo(
                "Swivel Wing (Up)",
                UpgradeType.Configuration,
                cost: 0,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.GauntletFighter.GauntletFighter)),
                abilityType: typeof(Abilities.SecondEdition.SwivelWingHarkUpAbility)
            );

            SelectSideOnSetup = false;

            NameCanonical = "swivelwinguphark";

            ImageUrl = "https://infinitearenas.com/xw2/images/upgrades/swivelwing-sideb.png";

            IsSecondSide = true;

            AnotherSide = typeof(SwivelWingHarkDown);
        }
    }
}