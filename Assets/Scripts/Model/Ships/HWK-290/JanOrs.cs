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
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Rebel%20Alliance/HWK-290/jan-ors.png";
                PilotSkill = 8;
                Cost = 25;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                faction = Faction.Rebels;

                PilotAbilities.Add(new PilotAbilitiesNamespace.JanOrsAbility());
            }
        }
    }
}

namespace PilotAbilitiesNamespace
{
    public class JanOrsAbility : GenericPilotAbility
    {
        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            GenericShip.OnAttackAsAttackerGlobal += RegisterJanOrsAbility;
            Host.OnDestroyed += RemoveAbility;
        }

        private void RemoveAbility(GenericShip ship)
        {
            GenericShip.OnAttackAsAttackerGlobal -= RegisterJanOrsAbility;
        }

        private void RegisterJanOrsAbility()
        {
            if (Combat.Attacker.Owner.PlayerNo == Host.Owner.PlayerNo && Combat.Attacker.ShipId != Host.ShipId)
            {
                Board.ShipDistanceInformation distanceInfo = new Board.ShipDistanceInformation(Combat.Attacker, Host);
                if (distanceInfo.Range < 4)
                {
                    RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskJanOrsAbility);
                }
            }
        }

        private void AskJanOrsAbility(object sender, System.EventArgs e)
        {
            if (!Host.HasToken(typeof(Tokens.StressToken)))
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
            Host.AssignToken(new Tokens.StressToken(), AllowRollAdditionalDice);
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
