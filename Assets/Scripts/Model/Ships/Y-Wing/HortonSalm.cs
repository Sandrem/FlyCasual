using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace YWing
    {
        public class HortonSalm : YWing
        {
            public HortonSalm() : base()
            {
                PilotName = "Horton Salm";
                ImageUrl = "https://vignette4.wikia.nocookie.net/xwing-miniatures/images/5/56/Horton_Salm.png";
                IsUnique = true;
                PilotSkill = 8;
                Cost = 25;
            }

            public override void InitializePilot()
            {
                base.InitializePilot();
                AfterGenerateAvailableActionEffectsList += WingedGundarkPilotAbility;
            }

            public void WingedGundarkPilotAbility(GenericShip ship)
            {
                ship.AddAvailableActionEffect(new PilotAbilities.HortonSalmAction());
            }
        }
    }
}

namespace PilotAbilities
{
    public class HortonSalmAction : ActionsList.GenericAction
    {
        public HortonSalmAction()
        {
            Name = EffectName = "Horton Salm's ability";
            IsReroll = true;
        }

        public override void ActionEffect(System.Action callBack)
        {
            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                SidesCanBeRerolled = new List<DiceSide> { DiceSide.Blank },
                CallBack = callBack
            };
            diceRerollManager.Start();
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;
            Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(Selection.ThisShip, Selection.AnotherShip);
            if ((Combat.AttackStep == CombatStep.Attack) && (shotInfo.Range > 1))
            {
                result = true;
            }
            return result;
        }

    }
}
