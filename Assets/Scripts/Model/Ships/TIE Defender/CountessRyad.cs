using GameModes;
using Movement;
using RuleSets;
using Ship;
using System;

namespace Ship
{
    namespace TIEDefender
    {
        public class CountessRyad : TIEDefender, ISecondEditionPilot
        {
            public CountessRyad() : base()
            {
                PilotName = "Countess Ryad";
                PilotSkill = 5;
                Cost = 34;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Crimson";

                PilotAbilities.Add(new Abilities.CountessRyadAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 4;
                Cost = 86;

                PilotAbilities.RemoveAll(ability => ability is Abilities.CountessRyadAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.CountessRyadAbility());
            }
        }
    }
}

namespace Abilities
{
    public class CountessRyadAbility : GenericAbility
    {
        string maneuverKey;
        MovementComplexity originalColor;

        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsRevealed += RegisterAskChangeManeuver;
        }        

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsRevealed -= RegisterAskChangeManeuver;
        }

        protected virtual void RegisterAskChangeManeuver(GenericShip ship)
        {
            if (HostShip.AssignedManeuver.Bearing == ManeuverBearing.Straight)
            {
                RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, AskChangeManeuver);                
            }
        }

        protected virtual MovementComplexity GetNewManeuverComplexity()
        {
            return HostShip.AssignedManeuver.ColorComplexity;
        }

        protected void AskChangeManeuver(object sender, System.EventArgs e)
        {
            Messages.ShowInfoToHuman("Countess Ryad: You can change your maneuver to Koiogran turn");
            maneuverKey = HostShip.AssignedManeuver.Speed + ".F.R";
            originalColor = HostShip.Maneuvers[maneuverKey];
            HostShip.Maneuvers[maneuverKey] = GetNewManeuverComplexity();
            HostShip.Owner.ChangeManeuver((maneuverCode) => {                    
                GameMode.CurrentGameMode.AssignManeuver(maneuverCode);
                HostShip.OnMovementFinish += RestoreManuvers;
            }, StraightOrKoiogran);
        }

        private void RestoreManuvers(GenericShip ship)
        {
            HostShip.OnMovementFinish -= RestoreManuvers;

            HostShip.Maneuvers[maneuverKey] = originalColor;
        }

        private bool StraightOrKoiogran(string maneuverString)
        {            
            bool result = false;
            MovementStruct movementStruct = new MovementStruct(maneuverString);
            if (movementStruct.Speed == Selection.ThisShip.AssignedManeuver.ManeuverSpeed && 
                (movementStruct.Bearing == ManeuverBearing.Straight ||
                movementStruct.Bearing == ManeuverBearing.KoiogranTurn))
            {
               result = true;              
            }
            return result;
        }
    }

    namespace SecondEdition
    {
        //While you would execute a straight maneuver, you may increase difficulty of the maeuver. If you do, execute it as a koiogran turn maneuver instead.
        public class CountessRyadAbility : Abilities.CountessRyadAbility
        {
            public override void ActivateAbility()
            {
                HostShip.BeforeMovementIsExecuted += RegisterAskChangeManeuver;
            }

            public override void DeactivateAbility()
            {
                HostShip.BeforeMovementIsExecuted -= RegisterAskChangeManeuver;
            }
            
            protected override void RegisterAskChangeManeuver(GenericShip ship)
            {
                //I have assumed that you can not use this ability if you execute a red maneuver
                if (HostShip.AssignedManeuver.ColorComplexity != MovementComplexity.Complex && HostShip.AssignedManeuver.Bearing == ManeuverBearing.Straight)
                {
                    RegisterAbilityTrigger(TriggerTypes.BeforeMovementIsExecuted, AskChangeManeuver);
                }
            }

            protected override MovementComplexity GetNewManeuverComplexity()
            {
                if (HostShip.AssignedManeuver.ColorComplexity == MovementComplexity.Complex)
                    throw new Exception("Can't increase difficulty of red maneuvers");

                return HostShip.AssignedManeuver.ColorComplexity + 1;
            }
        }
    }
}
