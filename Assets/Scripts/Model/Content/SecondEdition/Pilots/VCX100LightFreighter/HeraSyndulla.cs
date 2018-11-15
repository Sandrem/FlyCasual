using GameModes;
using Movement;
using Ship;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.VCX100LightFreighter
    {
        public class HeraSyndulla : VCX100LightFreighter
        {
            public HeraSyndulla() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Hera Syndulla",
                    5,
                    76,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.KananJarrusPilotAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                SEImageNumber = 73;
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class HeraSyndullaAbility : GenericAbility
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
            if (HostShip.AssignedManeuver.ColorComplexity == Movement.MovementComplexity.Easy || HostShip.AssignedManeuver.ColorComplexity == Movement.MovementComplexity.Complex)
            {
                HostShip.Owner.ChangeManeuver(GameMode.CurrentGameMode.AssignManeuver, IsSameComplexity);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private bool IsSameComplexity(string maneuverString)
        {
            bool result = false;
            ManeuverHolder movementStruct = new ManeuverHolder(maneuverString);
            if (movementStruct.ColorComplexity == HostShip.AssignedManeuver.ColorComplexity)
            {
                result = true;
            }
            return result;
        }
    }
}