using System.Collections;
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
                AddUpgradeSlot(Upgrade.UpgradeSlot.Elite);
            }

            public override void InitializePilot()
            {
                base.InitializePilot();

                OnActionSubPhaseStart += DoSecondAction;
            }

            private void DoSecondAction(GenericShip ship)
            {
                if (!IsSkipsActionSubPhase)
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

                    MovementTemplates.ReturnRangeRuler();

                    Triggers.ResolveTriggers(TriggerTypes.OnFreeActionPlanned, delegate { Triggers.FinishTrigger(); });
                }
            }

            private void PerformFreeAction(object sender, System.EventArgs e)
            {
                GenerateAvailableActionsList();
                List<ActionsList.GenericAction> actions = Selection.ThisShip.GetAvailableActionsList();

                AskPerformFreeAction(
                    actions,
                    delegate {
                        Phases.FinishSubPhase(typeof(SubPhases.FreeActionSubPhase));
                        Triggers.FinishTrigger();
                    }
                );
            }

        }
    }
}
