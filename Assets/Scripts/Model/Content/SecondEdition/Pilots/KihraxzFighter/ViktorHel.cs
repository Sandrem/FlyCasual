using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.KihraxzFighter
    {
        public class ViktorHel : KihraxzFighter
        {
            public ViktorHel() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Viktor Hel",
                    4,
                    45,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.ViktorHelAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 193
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ViktorHelAbility : GenericAbility
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

