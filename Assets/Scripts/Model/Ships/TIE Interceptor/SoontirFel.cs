using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;

namespace Ship
{
    namespace TIEInterceptor
    {
        public class SoontirFel : TIEInterceptor
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
        }
    }
}

namespace Abilities
{
    public class SoontirFelAbility : GenericAbility
    {
        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            HostShip.OnTokenIsAssigned += RegisterSoontirFelAbility;
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
                Selection.ThisShip.AssignToken(new Tokens.FocusToken(), Triggers.FinishTrigger);
            }
        }

        private void AssignToken(object sender, System.EventArgs e)
        {
            HostShip.AssignToken(new Tokens.FocusToken(), SubPhases.DecisionSubPhase.ConfirmDecision);
        }
    }
}
