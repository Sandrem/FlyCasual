using GameModes;
using Movement;
using Ship;

namespace Ship
{
    namespace TIEDefender
    {
        public class CountessRyad : TIEDefender
        {
            public CountessRyad() : base()
            {
                PilotName = "Countess Ryad";
                PilotSkill = 5;
                Cost = 34;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.CountessRyadAbility());
            }
        }
    }
}

namespace Abilities
{
    public class CountessRyadAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsRevealed += RegisterAskChangeManeuver;
        }        

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsRevealed -= RegisterAskChangeManeuver;
        }

        private void RegisterAskChangeManeuver(GenericShip ship)
        {
            if (HostShip.AssignedManeuver.Bearing == ManeuverBearing.Straight)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Countess Ryad Ability",
                    TriggerType = TriggerTypes.OnManeuverIsRevealed,
                    TriggerOwner = ship.Owner.PlayerNo,
                    EventHandler = AskChangeManeuver
                });
            }
        }

        private void AskChangeManeuver(object sender, System.EventArgs e)
        {
            string key = HostShip.AssignedManeuver.Speed + ".F.R";
            ManeuverColor originalColor = HostShip.Maneuvers[key];
            HostShip.Maneuvers[key] = HostShip.AssignedManeuver.ColorComplexity;
            HostShip.Owner.ChangeManeuver((maneuverCode) => {                    
                GameMode.CurrentGameMode.AssignManeuver(maneuverCode);
                HostShip.Maneuvers[key] = originalColor;
            }, StraightOrKoiogran);
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
}
