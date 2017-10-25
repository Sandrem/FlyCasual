using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEFO
    {
        public class ZetaLeader : TIEFO
        {
            public ZetaLeader () : base ()
            {
                PilotName = "Zeta Leader";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/First%20Order/TIE-fo%20Fighter/zeta-leader.png";
                PilotSkill = 7;
                Cost = 20;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }

            public override void InitializePilot ()
            {
                base.InitializePilot ();

                setupDecisionPilotAbility (TriggerTypes.OnAttackStart);
                OnCombatPhaseStart += this.RegisterPilotDecisionAbility;
                OnCombatPhaseEnd += this.RemovePilotDecisionAbility;
            }

            // ==== Pilot Ability ==== //
            protected override bool ShouldShowDecision(object sender)
            {
                Selection.ThisShip = this;
                // check if this ship is stressed
                if (!this.HasToken(typeof(Tokens.StressToken))) {
                    return true;
                }
                return false;
            }

            public override void UsePilotAbility(SubPhases.PilotDecisionSubPhase subPhase)
            {
                base.UsePilotAbility (subPhase);
                this.ChangeFirepowerBy (1);
                this.AssignToken(new Tokens.StressToken(), subPhase.ConfirmDecision);
            }

            protected override void RemovePilotDecisionAbility ( GenericShip genericShip)
            { 
                // At the end of combat phase, need to remove attack value increase
                if (this.abilityUsed)
                {
                    this.ChangeFirepowerBy(-1);
                }
                base.RemovePilotDecisionAbility (genericShip);
            }
        }
    }
}
