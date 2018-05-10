﻿using Ship;
using Upgrade;
using Abilities;
using Ship.XWing;
using System.Linq;
using ActionsList;
using Movement;
using GameModes;
using System.Collections.Generic;
using System;
using SubPhases;
using RuleSets;

namespace UpgradesList
{
    public class ServomotorSFoilsClosed : GenericDualUpgrade, ISecondEditionUpgrade
    {
        public ServomotorSFoilsClosed() : base()
        {
            Types.Add(UpgradeType.Modification);
            Name = "Servomotor S-Foils (Closed)";
            Cost = 0;                        
            UpgradeAbilities.Add(new ServomotorSFoilsClosedAbility());
            AnotherSide = typeof(ServomotorSFoilsAttack);
        }

        public void AdaptUpgradeToSecondEdition()
        {
            ImageUrl = "https://i.imgur.com/1BNoYDk.png";
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is XWing;
        }
    }

    public class ServomotorSFoilsAttack : GenericDualUpgrade, ISecondEditionUpgrade
    {
        public ServomotorSFoilsAttack() : base()
        {
            Types.Add(UpgradeType.Modification);
            Name = "Servomotor S-Foils (Attack)";
            Cost = 0;
            UpgradeAbilities.Add(new ServomotorSFoilsAttackAbility());
            AnotherSide = typeof(ServomotorSFoilsClosed);
        }

        public void AdaptUpgradeToSecondEdition()
        {
            ImageUrl = "https://i.imgur.com/IRkox13.png";
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is XWing;
        }
    }

}

namespace Abilities
{
    public abstract class ServomotorSFoilCommonAbility : GenericAbility
    {
        protected abstract bool AIWantsToFlip();

        protected abstract string FlipQuestion { get; }

        protected void TurnSFoilsToClosedPosition(GenericShip ship)
        {
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
            Phases.OnActivationPhaseStart += RegisterAskToUseFlip;
        }

        public override void DeactivateAbility()
        {
            Phases.OnActivationPhaseStart -= RegisterAskToUseFlip;
        }

    }

    public class ServomotorSFoilsClosedAbility : ServomotorSFoilCommonAbility
    {        
        public override void ActivateAbility()
        {
            base.ActivateAbility();
            TurnSFoilsToClosedPosition(HostShip);
            HostShip.ChangeFirepowerBy(-1);
            HostShip.AfterGenerateAvailableActionsList += AddActionIcon;
            HostShip.OnManeuverIsReadyToBeRevealed += CheckChangeManeuverComplexity;
        }
                
        public override void DeactivateAbility()
        {
            base.DeactivateAbility();
            TurnSFoilsToAttackPosition(HostShip);
            HostShip.ChangeFirepowerBy(+1);
            HostShip.AfterGenerateAvailableActionsList -= AddActionIcon;
            HostShip.OnManeuverIsReadyToBeRevealed -= CheckChangeManeuverComplexity;
        }

        protected void AddActionIcon(GenericShip host)
        {
            var alreadyHasBarrelRoll = host.PrintedActions.Any(n => n is BoostAction);
            if (!alreadyHasBarrelRoll)
            {
                host.AddAvailableAction(new BoostAction());
            }
        }

        private void CheckChangeManeuverComplexity(GenericShip ship)
        {
            if (HostShip.AssignedManeuver.Bearing == ManeuverBearing.Bank && HostShip.AssignedManeuver.Speed == 2)
            {
                HostShip.AssignedManeuver.ColorComplexity = ManeuverColor.Green;
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
                return string.Format("{0}: Open the S-Foils?", HostShip.PilotName);
            }
        }
    }

    public class ServomotorSFoilsAttackAbility : ServomotorSFoilCommonAbility
    {
        List<string> allowedMovements = new List<string>();

        public override void ActivateAbility()
        {
            base.ActivateAbility();
            HostShip.AfterGenerateAvailableActionsList += AddActionIcon;
            HostShip.OnManeuverIsRevealed += RegisterAskChangeManeuver;            
        }

        public override void DeactivateAbility()
        {
            base.DeactivateAbility();
            HostShip.AfterGenerateAvailableActionsList -= AddActionIcon;
            HostShip.OnManeuverIsRevealed -= RegisterAskChangeManeuver;            
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
            HostShip.Maneuvers[tallonCode] = ManeuverColor.Red;

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

            HostShip.Maneuvers[allowedMovements[1]] = ManeuverColor.None;
        }

        private bool TurnOrTallonRoll(string maneuverString)
        {            
            var result = allowedMovements.Contains(maneuverString);
            return result;
        }
        protected void AddActionIcon(GenericShip host)
        {
            var alreadyHasBarrelRoll = host.PrintedActions.Any(n => n is BarrelRollAction);
            if (!alreadyHasBarrelRoll)
            {
                host.AddAvailableAction(new BarrelRollAction());
            }
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
                return string.Format("{0}: Close the S-Foils?", HostShip.PilotName);
            }
        }
    }
}
