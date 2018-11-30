using GameModes;
using Movement;
using Ship;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIEDefender
    {
        public class CountessRyad : TIEDefender
        {
            public CountessRyad() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Countess Ryad",
                    5,
                    34,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.CountessRyadAbility),
                    extraUpgradeIcon: UpgradeType.Elite
                );

                ModelInfo.SkinName = "Crimson";
            }
        }
    }
}

namespace Abilities.FirstEdition
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
            originalColor = (HostShip.Maneuvers.ContainsKey(maneuverKey)) ? HostShip.Maneuvers[maneuverKey] : MovementComplexity.None;
            HostShip.Maneuvers[maneuverKey] = GetNewManeuverComplexity();
            HostShip.Owner.ChangeManeuver((maneuverCode) => {
                GameMode.CurrentGameMode.AssignManeuver(maneuverCode);
                HostShip.OnMovementFinish += RestoreManuvers;
            }, StraightOrKoiogran);
        }

        private void RestoreManuvers(GenericShip ship)
        {
            HostShip.OnMovementFinish -= RestoreManuvers;

            if (originalColor != MovementComplexity.None)
            {
                HostShip.Maneuvers[maneuverKey] = originalColor;
            }
            else
            {
                HostShip.Maneuvers.Remove(maneuverKey);
            }
        }

        private bool StraightOrKoiogran(string maneuverString)
        {
            bool result = false;
            ManeuverHolder movementStruct = new ManeuverHolder(maneuverString);
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
