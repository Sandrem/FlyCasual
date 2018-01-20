using System.Collections;
using System.Collections.Generic;
using Ship;
using UnityEngine;
using SubPhases;
using Board;
using Abilities;

namespace Ship
{
    namespace LancerClassPursuitCraft
    {
        public class AsajjVentress : LancerClassPursuitCraft
        {
            public AsajjVentress() : base()
            {
                PilotName = "Asajj Ventress";
                PilotSkill = 6;
                Cost = 37;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new AsajjVentressPilotAbility());
            }
        }
    }
}

namespace Abilities
{
    public class AsajjVentressPilotAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            Phases.OnCombatPhaseStart += TryRegisterAsajjVentressPilotAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.OnCombatPhaseStart -= TryRegisterAsajjVentressPilotAbility;
        }

        private void TryRegisterAsajjVentressPilotAbility()
        {
            //TODO: Stop after at least one is found
            if (BoardManager.GetShipsAtRange(HostShip, new Vector2(1, 2), Team.Type.Enemy).Count > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskSelectShip);
            }
        }

        private void AskSelectShip(object sender, System.EventArgs e)
        {
            SelectTargetForAbility(CheckAssignStress, new List<TargetTypes>() { TargetTypes.Enemy }, new Vector2(1, 2), null, true);
        }

        private void CheckAssignStress()
        {
            ShipShotDistanceInformation shotInfo = new ShipShotDistanceInformation(HostShip, TargetShip);
            if (shotInfo.InMobileArc && shotInfo.Range >= 1 && shotInfo.Range <= 2)
            {
                Messages.ShowError(HostShip.PilotName + " assigns Stress Token\nto " + TargetShip.PilotName);
                TargetShip.AssignToken(new Tokens.StressToken(), SelectShipSubPhase.FinishSelection);
            }
            else
            {
                if (!shotInfo.InMobileArc) Messages.ShowError("Target is not inside Mobile Arc");
                else if (shotInfo.Distance >= 3) Messages.ShowError("Target is outside range 2");
            }
        }

    }
}
