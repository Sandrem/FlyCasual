using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System;
using Tokens;
using RuleSets;

namespace Ship
{
    namespace TIEInterceptor
    {
        public class SoontirFel : TIEInterceptor, ISecondEditionPilot
        {
            public bool alwaysUseAbility;

            public SoontirFel() : base()
            {
                PilotName = "Soontir Fel";
                PilotSkill = 9;
                Cost = 27;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Red Stripes";

                PilotAbilities.Add(new Abilities.SoontirFelAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 6;
                Cost = 52;

                PilotAbilities.RemoveAll(ability => ability is Abilities.SoontirFelAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.SoontirFelAbilitySE());
            }
        }
    }
}

namespace Abilities
{
    public class SoontirFelAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnTokenIsAssigned += RegisterSoontirFelAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTokenIsAssigned -= RegisterSoontirFelAbility;
        }

        private void RegisterSoontirFelAbility(GenericShip ship, System.Type tokenType)
        {
            if (tokenType == typeof(Tokens.StressToken))
            {
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsAssigned, AskAssignFocus);
            }
        }

        private void AskAssignFocus(object sender, System.EventArgs e)
        {
            if (!alwaysUseAbility)
            {
                AskToUseAbility(AlwaysUseByDefault, AssignToken, null, null, true);
            }
            else
            {
                HostShip.Tokens.AssignToken(typeof(FocusToken), Triggers.FinishTrigger);
            }
        }

        private void AssignToken(object sender, System.EventArgs e)
        {
            HostShip.Tokens.AssignToken(typeof(FocusToken), SubPhases.DecisionSubPhase.ConfirmDecision);
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SoontirFelAbilitySE : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterSoontirFelAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterSoontirFelAbility;
        }

        private void RegisterSoontirFelAbility()
        {
            if (BoardTools.Board.GetShipsInBullseyeArc(HostShip, Team.Type.Enemy).Count > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, OnCombatAssignFocus);
            }
        }

        private void OnCombatAssignFocus(object sender, EventArgs e)
        {
            HostShip.Tokens.AssignToken(typeof(FocusToken), Triggers.FinishTrigger);
        }
    }

}