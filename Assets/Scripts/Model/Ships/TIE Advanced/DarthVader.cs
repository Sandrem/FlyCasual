﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                IsUnique = true;
                PilotSkill = 9;
                Cost = 29;

                SkinName = "Blue";

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }

            public override void InitializePilot()
            {
                base.InitializePilot();

                OnActionDecisionSubphaseEnd += DoSecondAction;
            }

            private void DoSecondAction(GenericShip ship)
            {
                if (!HasToken(typeof(Tokens.StressToken)))
                {
                    Triggers.RegisterTrigger(
                        new Trigger()
                        {
                            Name = "Darth Vader: Second action",
                            TriggerOwner = Owner.PlayerNo,
                            TriggerType = TriggerTypes.OnFreeActionPlanned,
                            EventHandler = PerformFreeAction
                        }
                    );
                }
            }

            private void PerformFreeAction(object sender, System.EventArgs e)
            {
                GenerateAvailableActionsList();
                List<ActionsList.GenericAction> actions = Selection.ThisShip.GetAvailableActionsList();

                AskPerformFreeAction(actions, Triggers.FinishTrigger);
            }

        }
    }
}
