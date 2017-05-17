using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace XWing
    {
        public class LukeSkywalker : XWing
        {
            public LukeSkywalker(Players.GenericPlayer owner, int shipId, Vector3 position) : base(owner, shipId, position)
            {
                PilotName = "Luke Skywalker";
                isUnique = true;
                PilotSkill = 8;
                AddUpgradeSlot(Upgrade.UpgradeSlot.Elite);

                AfterGenerateDiceModifications += AddLukeSkywalkerPilotAbility;
            }

            public void AddLukeSkywalkerPilotAbility(ref List<Actions.GenericAction> list)
            {
                list.Add(new LukeSkywalkerAction());
            }

        }

        public class LukeSkywalkerAction : Actions.GenericAction
        {
            private Ship.GenericShip host;

            public LukeSkywalkerAction()
            {
                Name = EffectName = "Luke Skywalker's ability";
            }

            public override void ActionEffect()
            {
                Game.Combat.CurentDiceRoll.ChangeOne(DiceSide.Focus, DiceSide.Success);
            }

            public override bool IsActionEffectAvailable()
            {
                bool result = false;
                if (Game.Combat.AttackStep == CombatStep.Defence) result = true;
                return result;
            }

        }

    }
}
