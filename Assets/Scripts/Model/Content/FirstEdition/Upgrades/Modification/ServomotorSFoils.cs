using Upgrade;
using Ship;
using ActionsList;
using System;
using SubPhases;
using Movement;
using System.Collections.Generic;
using GameModes;

namespace UpgradesList.FirstEdition
{
    public class ServomotorSFoilsClosed : GenericDualUpgrade
    {
        public ServomotorSFoilsClosed() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Servomotor S-Foils (Closed)",
                UpgradeType.Modification,
                cost: 0,
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.XWing.XWing)),
                abilityType: typeof(Abilities.FirstEdition.ServomotorSFoilsClosedAbility)
            );

            AnotherSide = typeof(ServomotorSFoilsAttack);
        }
    }

    public class ServomotorSFoilsAttack : GenericDualUpgrade
    {
        public ServomotorSFoilsAttack() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Servomotor S-Foils (Attack)",
                UpgradeType.Modification,
                cost: 0,
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.XWing.XWing)),
                abilityType: typeof(Abilities.FirstEdition.ServomotorSFoilsAttackAbility)
            );

            AnotherSide = typeof(ServomotorSFoilsClosed);
        }
    }
}

namespace Abilities.FirstEdition
{
    public abstract class ServomotorSFoilCommonAbility : GenericAbility
    {
        protected abstract bool AIWantsToFlip();

        protected abstract string FlipQuestion { get; }

        protected void TurnSFoilsToClosedPosition()
        {
            Phases.Events.OnGameStart -= TurnSFoilsToClosedPosition;

            HostShip.WingsClose();
        }

        protected void TurnSFoilsToAttackPosition(GenericShip ship)
        {
            HostShip.WingsOpen();
        }

        private void RegisterAskToUseFlip()
        {
            RegisterAbilityTrigger(TriggerTypes.OnActivationPhaseStart, AskToFlip);
        }

        protected void AskToFlip(object sender, EventArgs e)
        {
            AskToUseAbility(AIWantsToFlip, DoFlipSide, null, null, false, FlipQuestion);
        }

        protected void DoFlipSide(object sender, EventArgs e)
        {
            (HostUpgrade as GenericDualUpgrade).Flip();
            DecisionSubPhase.ConfirmDecision();
        }

        public override void ActivateAbility()
        {
            Phases.Events.OnActivationPhaseStart += RegisterAskToUseFlip;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnActivationPhaseStart -= RegisterAskToUseFlip;
        }

    }

    public class ServomotorSFoilsClosedAbility : ServomotorSFoilCommonAbility
    {
        public override void ActivateAbility()
        {
            base.ActivateAbility();
            Phases.Events.OnGameStart += TurnSFoilsToClosedPosition;
            HostShip.ChangeFirepowerBy(-1);
            HostShip.OnManeuverIsReadyToBeRevealed += CheckChangeManeuverComplexity;
        }

        public override void ActivateAbilityForSquadBuilder()
        {
            HostShip.ActionBar.AddGrantedAction(new BoostAction(), HostUpgrade);
        }

        public override void DeactivateAbility()
        {
            base.DeactivateAbility();
            Phases.Events.OnGameStart -= TurnSFoilsToClosedPosition;
            TurnSFoilsToAttackPosition(HostShip);
            HostShip.ChangeFirepowerBy(+1);
            HostShip.OnManeuverIsReadyToBeRevealed -= CheckChangeManeuverComplexity;
        }

        public override void DeactivateAbilityForSquadBuilder()
        {
            HostShip.ActionBar.RemoveGrantedAction(typeof(BoostAction), HostUpgrade);
        }

        private void CheckChangeManeuverComplexity(GenericShip ship)
        {
            if (HostShip.AssignedManeuver.Bearing == ManeuverBearing.Bank && HostShip.AssignedManeuver.Speed == 2)
            {
                HostShip.AssignedManeuver.ColorComplexity = MovementComplexity.Easy;
                Roster.UpdateAssignedManeuverDial(HostShip, HostShip.AssignedManeuver);
            }
        }

        protected override bool AIWantsToFlip()
        {
            /// TODO: Add more inteligence to this decision
            return true;
        }

        protected override string FlipQuestion
        {
            get
            {
                return string.Format("{0}: Open the S-Foils?", HostShip.PilotInfo.PilotName);
            }
        }
    }

    public class ServomotorSFoilsAttackAbility : ServomotorSFoilCommonAbility
    {
        List<string> allowedMovements = new List<string>();

        public override void ActivateAbility()
        {
            base.ActivateAbility();
            HostShip.OnManeuverIsRevealed += RegisterAskChangeManeuver;
        }

        public override void ActivateAbilityForSquadBuilder()
        {
            HostShip.ActionBar.AddGrantedAction(new BarrelRollAction(), HostUpgrade);
        }

        public override void DeactivateAbility()
        {
            TurnSFoilsToClosedPosition();
            base.DeactivateAbility();
            HostShip.OnManeuverIsRevealed -= RegisterAskChangeManeuver;
        }

        public override void DeactivateAbilityForSquadBuilder()
        {
            HostShip.ActionBar.RemoveGrantedAction(typeof(BarrelRollAction), HostUpgrade);
        }

        private void RegisterAskChangeManeuver(GenericShip ship)
        {
            if (HostShip.AssignedManeuver.Bearing == ManeuverBearing.Turn && HostShip.AssignedManeuver.Speed == 3 && HostShip.Tokens.CountTokensByType(typeof(Tokens.StressToken)) == 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, AskChangeManeuver);
            }
        }

        private void AskChangeManeuver(object sender, System.EventArgs e)
        {
            Messages.ShowInfoToHuman("Servomotor S-foils: You can change your maneuver to red Tallon roll");

            string tallonCode = HostShip.AssignedManeuver.ToString().Replace('T', 'E');
            HostShip.Maneuvers[tallonCode] = MovementComplexity.Complex;

            allowedMovements.Clear();
            allowedMovements.Add(HostShip.AssignedManeuver.ToString());
            allowedMovements.Add(tallonCode);

            HostShip.Owner.ChangeManeuver(
                (maneuverCode) => {
                    GameMode.CurrentGameMode.AssignManeuver(maneuverCode);
                    HostShip.OnMovementFinish += RestoreManuvers;
                },
                TurnOrTallonRoll
            );
        }

        private void RestoreManuvers(GenericShip ship)
        {
            HostShip.OnMovementFinish -= RestoreManuvers;

            HostShip.Maneuvers[allowedMovements[1]] = MovementComplexity.None;
        }

        private bool TurnOrTallonRoll(string maneuverString)
        {
            var result = allowedMovements.Contains(maneuverString);
            return result;
        }

        protected override bool AIWantsToFlip()
        {
            /// TODO: Add more inteligence to this decision
            return false;
        }

        protected override string FlipQuestion
        {
            get
            {
                return string.Format("{0}: Close the S-Foils?", HostShip.PilotInfo.PilotName);
            }
        }
    }
}