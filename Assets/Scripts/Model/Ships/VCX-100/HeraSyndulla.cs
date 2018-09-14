using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using GameModes;
using System;
using RuleSets;

namespace Ship
{
    namespace Vcx100
    {
        public class HeraSyndulla : Vcx100, ISecondEditionPilot
        {
            public HeraSyndulla() : base()
            {
                PilotName = "Hera Syndulla";
                PilotSkill = 7;
                Cost = 40;

                IsUnique = true;

                PilotAbilities.Add(new Abilities.HeraSyndullaAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 5;
                Cost = 76;

                SEImageNumber = 73;
            }
        }
    }
}

namespace Abilities
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
            Movement.MovementStruct movementStruct = new Movement.MovementStruct(maneuverString);
            if (movementStruct.ColorComplexity == HostShip.AssignedManeuver.ColorComplexity)
            {
                result = true;
            }
            return result;
        }
    }
}

