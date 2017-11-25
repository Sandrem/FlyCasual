using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;

namespace Ship
{
    namespace AttackShuttle
    {
        public class EzraBridger : AttackShuttle
        {
            public EzraBridger() : base()
            {
                PilotName = "Ezra Bridger";
                PilotSkill = 4;
                Cost = 20;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new PilotAbilitiesNamespace.EzraBridgerPilotAbility());
            }
        }
    }
}

namespace PilotAbilitiesNamespace
{
    public class EzraBridgerPilotAbility : GenericPilotAbility
    {
        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            Host.AfterGenerateAvailableActionEffectsList += AddEzraBridgerPilotAbility;
        }

        private void AddEzraBridgerPilotAbility(GenericShip ship)
        {
            ship.AddAvailableActionEffect(new EzraBridgerAction());
        }

        private class EzraBridgerAction : ActionsList.GenericAction
        {
            public EzraBridgerAction()
            {
                Name = EffectName = "Ezra Bridger's ability";

                IsTurnsAllFocusIntoSuccess = true;
            }

            public override void ActionEffect(System.Action callBack)
            {
                Combat.CurrentDiceRoll.ChangeOne(DieSide.Focus, DieSide.Success);
                Combat.CurrentDiceRoll.ChangeOne(DieSide.Focus, DieSide.Success);
                callBack();
            }

            public override bool IsActionEffectAvailable()
            {
                bool result = false;

                if (Combat.AttackStep == CombatStep.Defence && Combat.Defender.HasToken(typeof(Tokens.StressToken)))
                {
                    result = true;
                }

                return result;
            }

            public override int GetActionEffectPriority()
            {
                int result = 0;

                if (Combat.AttackStep == CombatStep.Defence && Combat.Defender.HasToken(typeof(Tokens.StressToken)))
                {
                    if (Combat.DiceRollAttack.Successes > Combat.DiceRollDefence.Successes)
                    {
                        if (Combat.DiceRollDefence.Focuses > 0) result = 80;
                    }
                }

                return result;
            }
        }

    }
}
