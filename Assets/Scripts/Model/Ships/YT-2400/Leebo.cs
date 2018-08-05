using Abilities.SecondEdition;
using ActionsList;
using RuleSets;
using Ship;
using System;

namespace Ship
{
    namespace YT2400
    {
        public class Leebo : YT2400, ISecondEditionPilot
        {
            public Leebo() : base()
            {
                PilotName = "\"Leebo\"";
                PilotSkill = 3;
                Cost = 98;

                IsUnique = true;
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                ActionBar.RemovePrintedAction(typeof(FocusAction));
                ActionBar.AddPrintedAction(new CalculateAction());

                PilotRuleType = typeof(SecondEdition);

                PilotAbilities.Add(new LeeboAbilitySE());
            }

            public void AdaptPilotToSecondEdition()
            {
                // Not required
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LeeboAbilitySE : GenericAbility
    {
        bool spentCalculate = false;

        public override void ActivateAbility()
        {
            HostShip.OnAttackFinish += CheckAssignCalculate;
            HostShip.OnTokenIsSpent += CheckCalculateSpent;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinish -= CheckAssignCalculate;
            HostShip.OnTokenIsSpent -= CheckCalculateSpent;
        }

        private void CheckCalculateSpent(GenericShip ship, System.Type type)
        {
            if (Combat.AttackStep == CombatStep.None)
                return;

            if (HostShip != Combat.Attacker && HostShip != Combat.Defender)
                return;

            if (type != typeof(Tokens.CalculateToken))
                return;

            spentCalculate = true;
        }

        private void CheckAssignCalculate(GenericShip ship)
        {
            if(spentCalculate)
            {
                spentCalculate = false;
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Assign calculate to Leebo.",
                    TriggerType = TriggerTypes.OnAttackFinish,
                    TriggerOwner = HostShip.Owner.PlayerNo,
                    EventHandler = delegate {
                        HostShip.Tokens.AssignToken(new Tokens.CalculateToken(HostShip), Triggers.FinishTrigger);
                    }
                });
            }
        }
    }
}