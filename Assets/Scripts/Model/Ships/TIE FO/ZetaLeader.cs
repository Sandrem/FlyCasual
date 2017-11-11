using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;

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

                PilotAbilities.Add(new PilotAbilitiesNamespace.ZetaLeaderAbility());
            }
        }
    }
}

namespace PilotAbilitiesNamespace
{
    public class ZetaLeaderAbility : GenericPilotAbility
    {
        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            Host.OnCombatPhaseStart += RegisterEpsilonLeaderAbility;
            Host.OnCombatPhaseEnd += RemoveEpsilonLeaderAbility;
        }

        private void RegisterEpsilonLeaderAbility(GenericShip genericShip)
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackStart, ShowDecision);
        }

        private void ShowDecision(object sender, System.EventArgs e)
        {
            // check if this ship is stressed
            if (!Host.HasToken(typeof(Tokens.StressToken)))
            {
                // give user the option to use ability
                AskToUseAbility(ShouldUsePilotAbility, UseAbility);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private bool ShouldUsePilotAbility()
        {
            return Actions.HasTarget(Host);
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            // don't need to check stressed as done already
            // add an attack dice
            isAbilityUsed = true;
            Host.ChangeFirepowerBy(+1);
            Host.AssignToken(new Tokens.StressToken(), SubPhases.DecisionSubPhase.ConfirmDecision);
        }

        private void RemoveEpsilonLeaderAbility(GenericShip genericShip)
        {
            // At the end of combat phase, need to remove attack value increase
            if (isAbilityUsed)
            {
                Host.ChangeFirepowerBy(-1);
                isAbilityUsed = false;
            }
        }
    }
}