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
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Galactic%20Empire/TIE%20Phantom/whisper.png";
                PilotSkill = 7;
                Cost = 32;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new PilotAbilitiesNamespace.WhisperAbility());
            }
        }
    }
}

namespace PilotAbilitiesNamespace
{
    public class WhisperAbility : GenericPilotAbility
    {
        public override void Initialize(Ship.GenericShip host)
        {
            base.Initialize(host);

            Host.OnAttackHitAsAttacker += RegisterWhisperAbility;
        }

        public void RegisterWhisperAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackHit, AskAssignFocus);
        }

        private void AskAssignFocus(object sender, System.EventArgs e)
        {
            if (!alwaysUseAbility)
            {
                AskToUseAbility(AlwaysUseByDefault, AssignToken, null, true);
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
