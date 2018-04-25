using Ship;
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

namespace UpgradesList
{
    public class ServomotorSFoilsClosed : GenericDualUpgrade
    {
        public ServomotorSFoilsClosed() : base()
        {
            Types.Add(UpgradeType.Modification);
            Name = "Servomotor S-Foils (Closed)";
            Cost = 0;                        
            UpgradeAbilities.Add(new ServomotorSFoilsClosedAbility());
            AnotherSide = typeof(ServomotorSFoilsAttack);
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is XWing;
        }
    }

    public class ServomotorSFoilsAttack : GenericDualUpgrade
    {
        public ServomotorSFoilsAttack() : base()
        {
            Types.Add(UpgradeType.Modification);
            Name = "Servomotor S-Foils (Attack)";
            Cost = 0;
            UpgradeAbilities.Add(new ServomotorSFoilsAttackAbility());
            AnotherSide = typeof(ServomotorSFoilsClosed);
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
            Messages.ShowInfo(string.Format("{0} may flip {1}", HostShip.PilotName, HostUpgrade.Name));
            AskToUseAbility(NeverUseByDefault, DoFlipSide);            
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
            HostShip.AfterGetManeuverColorDecreaseComplexity += CheckServomotorManeuverAbility;
        }
                
        public override void DeactivateAbility()
        {
            base.DeactivateAbility();
            TurnSFoilsToAttackPosition(HostShip);
            HostShip.ChangeFirepowerBy(+1);
            HostShip.AfterGenerateAvailableActionsList -= AddActionIcon;
            HostShip.AfterGetManeuverColorDecreaseComplexity -= CheckServomotorManeuverAbility;
        }

        protected void CheckServomotorManeuverAbility(GenericShip ship, ref MovementStruct movement)
        {
            if (movement.ColorComplexity != ManeuverColor.None)
            {
                if ((movement.Speed == ManeuverSpeed.Speed2) && (movement.Bearing == ManeuverBearing.Bank))
                {
                    movement.ColorComplexity = ManeuverColor.Green;
                }
            }
        }

        protected void AddActionIcon(GenericShip host)
        {
            var alreadyHasBarrelRoll = host.PrintedActions.Any(n => n is BoostAction);
            if (!alreadyHasBarrelRoll)
            {
                host.AddAvailableAction(new BoostAction());
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
            allowedMovements.Clear();
            var direction = HostShip.AssignedManeuver.Direction == ManeuverDirection.Left ? "L" : "R";
            var turnCode = "3." + direction + ".T";
            var tallonCode = "3." + direction + ".E";
            allowedMovements.Add(turnCode);
            allowedMovements.Add(tallonCode);
            var hadTallonBefore = false;
            var originalTallonColor = ManeuverColor.None;
            if (HostShip.Maneuvers.ContainsKey(tallonCode))
            {
                hadTallonBefore = true;
                originalTallonColor = HostShip.Maneuvers[tallonCode];
            }            
            HostShip.Maneuvers[tallonCode] = ManeuverColor.Red;
            HostShip.Owner.ChangeManeuver((maneuverCode) => 
            {
                GameMode.CurrentGameMode.AssignManeuver(maneuverCode);
                if (hadTallonBefore)
                {
                    HostShip.Maneuvers[tallonCode] = originalTallonColor;
                }
                else
                {
                    HostShip.Maneuvers.Remove(tallonCode);
                }
            }, TurnOrTallonRoll);
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
    }
}
