using GameModes;
using Movement;
using Ship;
using System.Collections;
using System.Collections.Generic;

namespace Ship
{
    namespace FirstEdition.VCX100
    {
        public class HeraSyndulla : VCX100
        {
            public HeraSyndulla() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Hera Syndulla",
                    7,
                    40,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.HeraSyndullaAbility)
                );
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
            if (HostShip.AssignedManeuver.ColorComplexity == MovementComplexity.Easy || HostShip.AssignedManeuver.ColorComplexity == MovementComplexity.Complex)
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