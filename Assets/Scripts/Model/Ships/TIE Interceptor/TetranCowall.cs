using Ship;
using Movement;
using GameModes;
using System.Linq;

namespace Ship
{
    namespace TIEInterceptor
    {
        public class TetranCowall : TIEInterceptor
        {
            public TetranCowall() : base()
            {
                PilotName = "Tetran Cowall";
                PilotSkill = 7;
                Cost = 24;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Red Stripes";

                PilotAbilities.Add(new Abilities.TetranCowallAbility());
            }
        }
    }
}

namespace Abilities
{
    public class TetranCowallAbility : GenericAbility
    {
        public string[] allowedKoiograns = new string[] { "1.F.R", "3.F.R", "5.F.R" };

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
            if (HostShip.AssignedManeuver.Bearing == ManeuverBearing.KoiogranTurn)
            {
                RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, AskChangeManeuver);
            }
        }

        private void AskChangeManeuver(object sender, System.EventArgs e)
        {
            Messages.ShowInfoToHuman("Tetran Cowall: Select speed of Koiogran turn");

            //To mantain Adrenalin Rush color change
            HostShip.Maneuvers["1.F.R"] = HostShip.AssignedManeuver.ColorComplexity;
            HostShip.Maneuvers["3.F.R"] = HostShip.AssignedManeuver.ColorComplexity;
            HostShip.Maneuvers["5.F.R"] = HostShip.AssignedManeuver.ColorComplexity;

            HostShip.Owner.ChangeManeuver((maneuverCode) => {
                GameMode.CurrentGameMode.AssignManeuver(maneuverCode);
                HostShip.OnMovementFinish -= RestoreManuvers;
            }, allowedKoiogranFilter);
        }

        private void RestoreManuvers(GenericShip ship)
        {
            HostShip.OnMovementFinish -= RestoreManuvers;

            HostShip.Maneuvers["1.F.R"] = MovementComplexity.None;
            HostShip.Maneuvers["3.F.R"] = MovementComplexity.Complex;
            HostShip.Maneuvers["5.F.R"] = MovementComplexity.Complex;
        }

        private bool allowedKoiogranFilter(string maneuverString)
        {
            return allowedKoiograns.Contains(maneuverString);
        }


    }
}


