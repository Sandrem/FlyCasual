using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEAdvanced
    {
        public class DarthVader : TIEAdvanced
        {
            private bool IsAbilityUsed;

            public DarthVader() : base()
            {
                PilotName = "Darth Vader";
                ImageUrl = "https://vignette1.wikia.nocookie.net/xwing-miniatures/images/f/f7/Darth_Vader.png";
                IsUnique = true;
                PilotSkill = 9;
                Cost = 29;
                AddUpgradeSlot(Upgrade.UpgradeSlot.Elite);
            }

            public override void InitializePilot()
            {
                base.InitializePilot();

                AfterActionIsPerformed += DoSecondAction;
                Phases.OnEndPhaseStart += ReactivateAbility;
            }

            private void DoSecondAction(GenericShip ship, System.Type type)
            {
                if (!IsAbilityUsed)
                {
                    IsAbilityUsed = true;
                    List<ActionsList.GenericAction> actions = new List<ActionsList.GenericAction>() { new ActionsList.FocusAction() };
                    AskPerformFreeAction(actions);
                }
            }

            private void ReactivateAbility()
            {
                IsAbilityUsed = false;
            }

        }
    }
}
