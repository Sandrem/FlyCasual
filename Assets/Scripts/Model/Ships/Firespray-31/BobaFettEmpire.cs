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

                PilotAbilities.Add(new PilotAbilitiesNamespace.BobaFettEmpireAbility());
            }
        }
    }
}

namespace PilotAbilitiesNamespace
{
    public class BobaFettEmpireAbility : GenericPilotAbility
    {
        public override void Initialize(Ship.GenericShip host)
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
            if (Host.AssignedManeuver.Bearing == Movement.ManeuverBearing.Bank)
            {
                DirectionsMenu.Show(GameMode.CurrentGameMode.AssignManeuver, IsBankManeuversSameSpeed);
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
            if (movementStruct.Bearing == Movement.ManeuverBearing.Bank && movementStruct.Speed == Host.AssignedManeuver.ManeuverSpeed)
            {
                result = true;
            }
            return result;
        }
    }
}
