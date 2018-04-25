using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mods.ModsList;
using Abilities;
using ActionsList;
using Tokens;
using SubPhases;

namespace Ship
{
    namespace XWing
    {
        public class LeevanTenza : XWing
        {
            public LeevanTenza() : base()
            {
                PilotName = "Leevan Tenza";
                PilotSkill = 5;
                Cost = 25;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Partisan";

                PilotAbilities.Add(new LeevanTenzaAbility());
            }
        }
    }
}

namespace Abilities
{
    public class LeevanTenzaAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckLeevanTenzaAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckLeevanTenzaAbility;
        }

        private void CheckLeevanTenzaAbility(GenericAction action)
        {
            if (action is BoostAction)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AskToUseLeevanTenzaAbility);
            }
        }

        private void AskToUseLeevanTenzaAbility(object sender, System.EventArgs e)
        {
            AskToUseAbility(NeverUseByDefault, UseLeevanTenzaAbility);
        }

        private void UseLeevanTenzaAbility(object sender, System.EventArgs e)
        {
            HostShip.Tokens.AssignToken(new StressToken(HostShip), GetFocusTokenAndFinish);
        }

        private void GetFocusTokenAndFinish()
        {
            HostShip.Tokens.AssignToken(new FocusToken(HostShip), DecisionSubPhase.ConfirmDecision);
        }
    }
}
