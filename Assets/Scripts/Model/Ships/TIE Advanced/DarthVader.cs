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
                ImageUrl = "https://vignette1.wikia.nocookie.net/xwing-miniatures/images/f/f7/Darth_Vader.png";
                PilotSkill = 9;
                Cost = 29;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Blue";

                PilotAbilities.Add(new PilotAbilitiesNamespace.DarthVaderAbility());
            }
        }
    }
}

namespace PilotAbilitiesNamespace
{
    public class DarthVaderAbility : GenericPilotAbility
    {
        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            Host.OnActionDecisionSubphaseEnd += DoSecondAction;
        }

        private void DoSecondAction(GenericShip ship)
        {
            if (!Host.HasToken(typeof(Tokens.StressToken)))
            {
                RegisterAbilityTrigger(TriggerTypes.OnFreeActionPlanned, PerformFreeAction);
            }
        }

        private void PerformFreeAction(object sender, System.EventArgs e)
        {
            Host.GenerateAvailableActionsList();
            List<ActionsList.GenericAction> actions = Selection.ThisShip.GetAvailableActionsList();

            Host.AskPerformFreeAction(actions, Triggers.FinishTrigger);
        }
    }
}
