using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace XWing
    {
        public class LukeSkywalker : XWing
        {
            public LukeSkywalker() : base()
            {
                PilotName = "Luke Skywalker";
                ImageUrl = "https://vignette3.wikia.nocookie.net/xwing-miniatures/images/8/8c/Luke-skywalker.png";
                isUnique = true;
                PilotSkill = 8;
                Cost = 28;
                AddUpgradeSlot(Upgrade.UpgradeSlot.Elite);
            }

            public override void InitializePilot()
            {
                base.InitializePilot();
                AfterGenerateDiceModifications += AddLukeSkywalkerPilotAbility;
            }

            public void AddLukeSkywalkerPilotAbility(ref List<ActionsList.GenericAction> list)
            {
                list.Add(new PilotAbilities.LukeSkywalkerAction());
            }

        }

    }
}

namespace PilotAbilities
{
    public class LukeSkywalkerAction : ActionsList.GenericAction
    {
        private Ship.GenericShip host;

        public LukeSkywalkerAction()
        {
            Name = EffectName = "Luke Skywalker's ability";
        }

        public override void ActionEffect()
        {
            Combat.CurentDiceRoll.ChangeOne(DiceSide.Focus, DiceSide.Success);
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;
            if (Combat.AttackStep == CombatStep.Defence) result = true;
            return result;
        }

    }
}
