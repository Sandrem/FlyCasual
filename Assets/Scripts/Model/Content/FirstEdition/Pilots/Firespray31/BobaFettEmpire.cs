using GameModes;
using Movement;
using Ship;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.Firespray31
    {
        public class BobaFettEmpire : Firespray31
        {
            public BobaFettEmpire() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Boba Fett",
                    8,
                    39,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.BobaFettEmpireAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    factionOverride: Faction.Imperial
                );

                ModelInfo.SkinName = "Boba Fett";
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class BobaFettEmpireAbility : GenericAbility
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
            RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, AskChangeManeuver);
        }

        private void AskChangeManeuver(object sender, System.EventArgs e)
        {
            if (HostShip.AssignedManeuver.Bearing == ManeuverBearing.Bank)
            {
                HostShip.Owner.ChangeManeuver(GameMode.CurrentGameMode.AssignManeuver, IsBankManeuversSameSpeed);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private bool IsBankManeuversSameSpeed(string maneuverString)
        {
            bool result = false;
            ManeuverHolder movementStruct = new ManeuverHolder(maneuverString);
            if (movementStruct.Bearing == ManeuverBearing.Bank && movementStruct.Speed == HostShip.AssignedManeuver.ManeuverSpeed)
            {
                result = true;
            }
            return result;
        }
    }
}
