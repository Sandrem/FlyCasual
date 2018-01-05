using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEPhantom
    {
        public class Whisper : TIEPhantom
        {
            public bool alwaysUseAbility;

            public Whisper() : base()
            {
                PilotName = "\"Whisper\"";
                PilotSkill = 7;
                Cost = 32;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.WhisperAbility());
            }
        }
    }
}

namespace Abilities
{
    public class WhisperAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackHitAsAttacker += RegisterWhisperAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackHitAsAttacker -= RegisterWhisperAbility;
        }

        public void RegisterWhisperAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackHit, AskAssignFocus);
        }

        private void AskAssignFocus(object sender, System.EventArgs e)
        {
            if (!alwaysUseAbility)
            {
                AskToUseAbility(AlwaysUseByDefault, AssignToken, null, null, true);
            }
            else
            {
                Selection.ThisShip.AssignToken(new Tokens.FocusToken(), Triggers.FinishTrigger);
            }
        }

        private void AssignToken(object sender, System.EventArgs e)
        {
            Selection.ThisShip.AssignToken(new Tokens.FocusToken(), SubPhases.DecisionSubPhase.ConfirmDecision);
        }
    }

}
