using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;

// Second->First: Two same actions
// Triggers are empty

namespace Ship
{
    namespace TIEAdvanced
    {
        public class DarthVader : TIEAdvanced
        {
            public DarthVader() : base()
            {
                PilotName = "Darth Vader";
                PilotSkill = 9;
                Cost = 29;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Blue";

                PilotAbilities.Add(new AbilitiesNamespace.DarthVaderAbility());
            }
        }
    }
}

namespace AbilitiesNamespace
{
    public class DarthVaderAbility : GenericAbility
    {
        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            HostShip.OnActionDecisionSubphaseEnd += DoSecondAction;
        }

        private void DoSecondAction(GenericShip ship)
        {
            if (!HostShip.HasToken(typeof(Tokens.StressToken)))
            {
                RegisterAbilityTrigger(TriggerTypes.OnFreeActionPlanned, PerformFreeAction);
            }
        }

        private void PerformFreeAction(object sender, System.EventArgs e)
        {
            HostShip.GenerateAvailableActionsList();
            List<ActionsList.GenericAction> actions = Selection.ThisShip.GetAvailableActionsList();

            HostShip.AskPerformFreeAction(actions, Triggers.FinishTrigger);
        }
    }
}
