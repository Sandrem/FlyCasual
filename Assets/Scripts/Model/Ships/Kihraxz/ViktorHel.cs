using RuleSets;
using Ship;
using System;
using Tokens;

namespace Ship
{
    namespace Kihraxz
    {
        public class ViktorHel : Kihraxz, ISecondEditionPilot
        {
            public ViktorHel()
            {
                PilotName = "Viktor Hel";
                PilotSkill = 4;
                Cost = 45;

                IsUnique = true;

                PilotRuleType = typeof(SecondEdition);
                PilotAbilities.Add(new Abilities.SecondEdition.ViktorHelAbilitySE());
            }

            public void AdaptPilotToSecondEdition()
            {
                // nope
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ViktorHelAbilitySE : GenericAbility
    {
        GenericShip attacker;
        int defenseDiceRolled;

        public override void ActivateAbility()
        {
            HostShip.AfterNumberOfDefenceDiceConfirmed += StoreDiceRolled;
            HostShip.OnAttackFinishAsDefender += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterNumberOfDefenceDiceConfirmed -= StoreDiceRolled;
            HostShip.OnAttackFinishAsDefender -= RegisterAbility;
        }

        public void StoreDiceRolled(ref int dice)
        {
            attacker = Combat.Attacker;
            defenseDiceRolled = dice;
        }

        public void RegisterAbility(GenericShip ship)
        {
            if (defenseDiceRolled == 2)
                return;

            RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, AssignStress);
        }

        public void AssignStress(object sender, EventArgs e)
        {
            Messages.ShowError("Viktor Hel assigns the attacker stress!");
            attacker.Tokens.AssignToken(typeof(StressToken), Triggers.FinishTrigger);
        }
    }
}
