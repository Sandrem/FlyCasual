using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEFO
    {
        public class ZetaLeader : TIEFO
        {
            public bool abilityUsed = false;
            public ZetaLeader () : base ()
            {
                PilotName = "Zeta Leader";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/First%20Order/TIE-fo%20Fighter/zeta-leader.png";
                IsUnique = true;
                PilotSkill = 7;
                Cost = 20;
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }

            public override void InitializePilot ()
            {
                base.InitializePilot ();

                OnCombatPhaseStart += RegisterEpsilonLeaderAbility;
                OnCombatPhaseEnd += DeregisterEpsilonLeaderAbility;
            }

            private void RegisterEpsilonLeaderAbility (GenericShip genericShip)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Zeta Leader Ability",
                    TriggerOwner = this.Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnAttackStart,
                    EventHandler = ShowDecision
                });
            }

            private void ShowDecision(object sender, System.EventArgs e)
            {
                Selection.ThisShip = this;
                Ship.GenericShip ZetaLeader = Selection.ThisShip;
                // check if this ship is stressed
                if (!ZetaLeader.HasToken (typeof(Tokens.StressToken))) {
                    // give user the option to use ability
                    Phases.StartTemporarySubPhase (
                        "Ability of Zeta Leader",
                        typeof(SubPhases.AddAttackDiceDecisionSubPhase),
                        delegate () { Triggers.FinishTrigger(); }
                    );
                } else {
                    Triggers.FinishTrigger ();
                }
            }

            private void DeregisterEpsilonLeaderAbility ( GenericShip genericShip)
            {
                // At the end of combat phase, need to remove attack value increase
                Triggers.RegisterTrigger (new Trigger () {
                    Name = "Zeta Leader Ability Expired",
                    TriggerOwner = this.Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnCombatPhaseEnd,
                    EventHandler = RemoveAbility
                });
            }

            private void RemoveAbility(object sender, System.EventArgs e)
            {
                Selection.ThisShip = this;
                Ship.TIEFO.ZetaLeader ZetaLeader = (Ship.TIEFO.ZetaLeader)Selection.ThisShip;
                if (ZetaLeader.abilityUsed) {
                    ZetaLeader.ChangeFirepowerBy (-1);
                    ZetaLeader.abilityUsed = false;
                }
                Triggers.FinishTrigger ();
            }

        }
    }
}

namespace SubPhases
{
    public class AddAttackDiceDecisionSubPhase : DecisionSubPhase
    {
        public override void Prepare()
        {
            infoText = "Use Zeta Leaders Ability?";

            AddDecision( "Use Pilot Ability", UseAbility);
            AddDecision ("Cancel", DoNotUseAbility );
           /* AddTooltip(
                ZetaLeader.PilotName,
                ZetaLeader.ImageUrl
            );*/

            //defaultDecision = none;
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            // don't need to check stressed as done already
            // add an attack dice
            Ship.TIEFO.ZetaLeader ZetaLeader = (Ship.TIEFO.ZetaLeader)Selection.ThisShip;
            ZetaLeader.abilityUsed = true;
            Selection.ThisShip.ChangeFirepowerBy (1);
            Selection.ThisShip.AssignToken(new Tokens.StressToken(), delegate {} );

            ConfirmDecision();
        }

        private void DoNotUseAbility(object sender, System.EventArgs e)
        {
            ConfirmDecision();
        }

        private void ConfirmDecision()
        {
            Tooltips.EndTooltip();
            Phases.FinishSubPhase(this.GetType());
            CallBack();
        }
    }
}