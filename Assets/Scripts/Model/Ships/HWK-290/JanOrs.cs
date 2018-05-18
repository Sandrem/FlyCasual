using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System;

namespace Ship
{
    namespace HWK290
    {
        public class JanOrs : HWK290
        {
            public JanOrs() : base()
            {
                PilotName = "Jan Ors";
                PilotSkill = 8;
                Cost = 25;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                faction = Faction.Rebel;

                PilotAbilities.Add(new Abilities.JanOrsAbility());
            }
        }
    }
}

namespace Abilities
{
    public class JanOrsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnAttackStartAsAttackerGlobal += RegisterJanOrsAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnAttackStartAsAttackerGlobal -= RegisterJanOrsAbility;
        }

        private void RegisterJanOrsAbility()
        {
            if (Combat.Attacker.Owner.PlayerNo == HostShip.Owner.PlayerNo && Combat.Attacker.ShipId != HostShip.ShipId)
            {
                BoardTools.DistanceInfo distanceInfo = new BoardTools.DistanceInfo(Combat.Attacker, HostShip);
                if (distanceInfo.Range < 4)
                {
                    RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskJanOrsAbility);
                }
            }
        }

        private void AskJanOrsAbility(object sender, System.EventArgs e)
        {
            if (!HostShip.Tokens.HasToken(typeof(Tokens.StressToken)))
            {
                AskToUseAbility(AlwaysUseByDefault, UseJanOrsAbility);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void UseJanOrsAbility(object sender, System.EventArgs e)
        {
            HostShip.Tokens.AssignToken(new Tokens.StressToken(HostShip), AllowRollAdditionalDice);
        }

        private void AllowRollAdditionalDice()
        {
            Combat.Attacker.AfterGotNumberOfAttackDice += IncreaseByOne;
            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        private void IncreaseByOne(ref int value)
        {
            value++;
            Combat.Attacker.AfterGotNumberOfAttackDice -= IncreaseByOne;
        }
    }
}
