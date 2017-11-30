using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using GameModes;

namespace Ship
{
    namespace Vcx100
    {
        public class HeraSyndulla : Vcx100
        {
            public HeraSyndulla() : base()
            {
                PilotName = "Hera Syndulla";
                PilotSkill = 7;
                Cost = 40;

                IsUnique = true;

                PilotAbilities.Add(new PilotAbilitiesNamespace.HeraSyndullaAbility());
            }
        }
    }
}

namespace PilotAbilitiesNamespace
{
    public class HeraSyndullaAbility : GenericPilotAbility
    {
        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            Host.OnManeuverIsRevealed += RegisterAskChangeManeuver;
        }

        private void RegisterAskChangeManeuver(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, AskChangeManeuver);
        }

        private void AskChangeManeuver(object sender, System.EventArgs e)
        {
            if (Host.AssignedManeuver.ColorComplexity == Movement.ManeuverColor.Green || Host.AssignedManeuver.ColorComplexity == Movement.ManeuverColor.Red)
            {
                Host.Owner.ChangeManeuver(GameMode.CurrentGameMode.AssignManeuver, IsSameComplexity);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private bool IsSameComplexity(string maneuverString)
        {
            bool result = false;
            Movement.MovementStruct movementStruct = new Movement.MovementStruct(maneuverString);
            if (movementStruct.ColorComplexity == Host.AssignedManeuver.ColorComplexity)
            {
                result = true;
            }
            return result;
        }
    }
}

