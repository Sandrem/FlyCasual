using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Ship;
using Abilities;

namespace Ship
{
    namespace StarViper
    {
        public class Guri : StarViper
        {
            public Guri() : base()
            {
                PilotName = "Guri";
                PilotSkill = 5;
                Cost = 30;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new GuriAbility());
            }
        }
    }
}

namespace Abilities
{
    public class GuriAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            Phases.OnCombatPhaseStart_Triggers += RegisterGuriAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.OnCombatPhaseStart_Triggers -= RegisterGuriAbility;
        }

        private void RegisterGuriAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskGuriAbility);
        }

        private void AskGuriAbility(object sender, EventArgs e)
        {
            if (Board.BoardManager.GetShipsAtRange(HostShip, new Vector2(1, 1), Team.Type.Enemy).Count > 0)
            {
                if (!alwaysUseAbility)
                {
                    AskToUseAbility(AlwaysUseByDefault, UseAbility, null, null, true);
                }
                else
                {
                    AssignFocus(Triggers.FinishTrigger);
                }
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void UseAbility(object sender, EventArgs e)
        {
            AssignFocus(SubPhases.DecisionSubPhase.ConfirmDecision);
        }

        private void AssignFocus(Action callback)
        {
            HostShip.Tokens.AssignToken(new Tokens.FocusToken(HostShip), callback);
        }

    }
}

