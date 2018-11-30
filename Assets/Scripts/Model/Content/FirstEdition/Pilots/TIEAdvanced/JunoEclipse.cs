using GameModes;
using Movement;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIEAdvanced
    {
        public class JunoEclipse : TIEAdvanced
        {
            public JunoEclipse() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Juno Eclipse",
                    8,
                    28,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.JunoEclipseAbility),
                    extraUpgradeIcon: UpgradeType.Elite
                );
            }
        }
    }
}


namespace Abilities.FirstEdition
{
    public class JunoEclipseAbility : GenericAbility
    {
        List<string> allowedMovements = new List<string>();

        string keyPlus;
        MovementComplexity plusColor;
        string keyMinus;
        MovementComplexity minusColor;

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
            RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, AskChangeManeuver);
        }

        private void AskChangeManeuver(object sender, System.EventArgs e)
        {
            Messages.ShowInfoToHuman(HostShip.PilotName + " : You can increase or decrease your maneuver speed");
            allowedMovements.Clear();
            string key = HostShip.AssignedManeuver.ToString();
            int speed = HostShip.AssignedManeuver.Speed;
            allowedMovements.Add(key);

            //Generate key for maneuvre + 1. If exist backups old color, and change to actual
            keyPlus = key.Replace(speed.ToString()[0], (speed + 1).ToString()[0]);
            plusColor = MovementComplexity.None;
            if (HostShip.Maneuvers.ContainsKey(keyPlus))
            {
                allowedMovements.Add(keyPlus);
                plusColor = HostShip.Maneuvers[keyPlus];
                HostShip.Maneuvers[keyPlus] = HostShip.AssignedManeuver.ColorComplexity;
            }

            //Generate key for maneuvre - 1. If exist backups old color, and change to actual
            keyMinus = key.Replace(speed.ToString()[0], (speed - 1).ToString()[0]);
            minusColor = MovementComplexity.None;
            if (HostShip.Maneuvers.ContainsKey(keyMinus))
            {
                allowedMovements.Add(keyMinus);
                minusColor = HostShip.Maneuvers[keyMinus];
                HostShip.Maneuvers[keyMinus] = HostShip.AssignedManeuver.ColorComplexity;
            }

            HostShip.Owner.ChangeManeuver((maneuverCode) => {
                GameMode.CurrentGameMode.AssignManeuver(maneuverCode);
                HostShip.OnMovementFinish += RestoreManuvers;
            }, StraightOrKoiogran);
        }

        private void RestoreManuvers(GenericShip ship)
        {
            HostShip.OnMovementFinish -= RestoreManuvers;

            if (HostShip.Maneuvers.ContainsKey(keyPlus)) { HostShip.Maneuvers[keyPlus] = plusColor; }
            if (HostShip.Maneuvers.ContainsKey(keyMinus)) { HostShip.Maneuvers[keyMinus] = minusColor; }
        }

        private bool StraightOrKoiogran(string maneuverString)
        {
            return allowedMovements.Contains(maneuverString);
        }

    }
}

