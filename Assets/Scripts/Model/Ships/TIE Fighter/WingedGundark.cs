using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEFighter
    {
        public class WingedGundark : TIEFighter
        {
            public WingedGundark() : base()
            {
                PilotName = "\"Winged Gundark\"";
                ImageUrl = "https://vignette2.wikia.nocookie.net/xwing-miniatures/images/9/9d/Winged-gundark.png";
                IsUnique = true;
                PilotSkill = 5;
                Cost = 15;
            }

            public override void InitializePilot()
            {
                base.InitializePilot();
                AfterGenerateAvailableActionEffectsList += WingedGundarkPilotAbility;
            }

            public void WingedGundarkPilotAbility(GenericShip ship)
            {
                ship.AddAvailableActionEffect(new PilotAbilities.WingedGundarkAction());
            }
        }
    }
}

namespace PilotAbilities
{
    public class WingedGundarkAction : ActionsList.GenericAction
    {
        private Ship.GenericShip host;

        public WingedGundarkAction()
        {
            Name = EffectName = "\"Winged Gundark\"'s ability";
        }

        public override void ActionEffect()
        {
            Combat.CurentDiceRoll.ChangeOne(DiceSide.Success, DiceSide.Crit);
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;
            /*if ((Combat.AttackStep == CombatStep.Attack) && (Actions.GetRange(Selection.ThisShip, Selection.AnotherShip) == 1))
            {
                result = true;
            }*/
            return result;
        }

    }
}
