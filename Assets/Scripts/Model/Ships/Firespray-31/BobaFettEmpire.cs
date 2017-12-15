using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using GameModes;

namespace Ship
{
    namespace Firespray31
    {
        public class BobaFettEmpire : Firespray31
        {
            public BobaFettEmpire() : base()
            {
                PilotName = "Boba Fett";
                PilotSkill = 8;
                Cost = 39;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                faction = Faction.Imperial;

                SkinName = "Boba Fett";

                PilotAbilities.Add(new Abilities.BobaFettEmpireAbility());
            }
        }
    }
}

namespace Abilities
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
            if (HostShip.AssignedManeuver.Bearing == Movement.ManeuverBearing.Bank)
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
            Movement.MovementStruct movementStruct = new Movement.MovementStruct(maneuverString);
            if (movementStruct.Bearing == Movement.ManeuverBearing.Bank && movementStruct.Speed == HostShip.AssignedManeuver.ManeuverSpeed)
            {
                result = true;
            }
            return result;
        }
    }
}
