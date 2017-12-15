using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System;

namespace Ship
{
    namespace XWing
    {
        public class LukeSkywalker : XWing
        {
            public LukeSkywalker() : base()
            {
                PilotName = "Luke Skywalker";
                PilotSkill = 8;
                Cost = 28;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.LukeSkywalkerAbility());
            }
        }
    }
}

namespace Abilities
{
    public class LukeSkywalkerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList += AddLukeSkywalkerPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList -= AddLukeSkywalkerPilotAbility;
        }

        private void AddLukeSkywalkerPilotAbility(GenericShip ship)
        {
            ship.AddAvailableActionEffect(new LukeSkywalkerAction());
        }

        private class LukeSkywalkerAction : ActionsList.GenericAction
        {
            public LukeSkywalkerAction()
            {
                Name = EffectName = "Luke Skywalker's ability";

                IsTurnsOneFocusIntoSuccess = true;
            }

            public override void ActionEffect(System.Action callBack)
            {
                Combat.CurrentDiceRoll.ChangeOne(DieSide.Focus, DieSide.Success);
                callBack();
            }

            public override bool IsActionEffectAvailable()
            {
                bool result = false;
                if (Combat.AttackStep == CombatStep.Defence) result = true;
                return result;
            }

            public override int GetActionEffectPriority()
            {
                int result = 0;

                if (Combat.AttackStep == CombatStep.Defence)
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