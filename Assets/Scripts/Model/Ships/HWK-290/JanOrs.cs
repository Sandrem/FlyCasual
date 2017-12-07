using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;

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

                PilotAbilities.Add(new AbilitiesNamespace.JanOrsAbility());
            }
        }
    }
}

namespace AbilitiesNamespace
{
    public class JanOrsAbility : GenericAbility
    {
        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            GenericShip.OnAttackStartAsAttackerGlobal += RegisterJanOrsAbility;
            HostShip.OnDestroyed += RemoveAbility;
        }

        private void RemoveAbility(GenericShip ship)
        {
            GenericShip.OnAttackStartAsAttackerGlobal -= RegisterJanOrsAbility;
        }

        private void RegisterJanOrsAbility()
        {
            if (Combat.Attacker.Owner.PlayerNo == HostShip.Owner.PlayerNo && Combat.Attacker.ShipId != HostShip.ShipId)
            {
                Board.ShipDistanceInformation distanceInfo = new Board.ShipDistanceInformation(Combat.Attacker, HostShip);
                if (distanceInfo.Range < 4)
                {
                    RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskJanOrsAbility);
                }
            }
        }

        private void AskJanOrsAbility(object sender, System.EventArgs e)
        {
            if (!HostShip.HasToken(typeof(Tokens.StressToken)))
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
            HostShip.AssignToken(new Tokens.StressToken(), AllowRollAdditionalDice);
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
