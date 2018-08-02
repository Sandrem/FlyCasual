using RuleSets;
using System;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using UnityEngine;

namespace Ship
{
    namespace TIEPhantom
    {
        public class Whisper : TIEPhantom, ISecondEditionPilot
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

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 5;
                Cost = 52;

                PilotAbilities.RemoveAll(ability => ability is Abilities.WhisperAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.WhisperAbilitySE());
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

        protected virtual Type GetTokenType()
        {
            return typeof(FocusToken);
        }

        private void AskAssignFocus(object sender, System.EventArgs e)
        {
            if (!alwaysUseAbility)
            {
                AskToUseAbility(AlwaysUseByDefault, AssignToken, null, null, true);
            }
            else
            {
                HostShip.Tokens.AssignToken(GetTokenType(), Triggers.FinishTrigger);
            }
        }

        private void AssignToken(object sender, System.EventArgs e)
        {
            HostShip.Tokens.AssignToken(GetTokenType(), SubPhases.DecisionSubPhase.ConfirmDecision);
        }
    }

}

namespace Abilities.SecondEdition
{
    public class WhisperAbilitySE : WhisperAbility
    {
        protected override Type GetTokenType()
        {
            return typeof(EvadeToken);
        }
    }
}