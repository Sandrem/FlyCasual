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
                PilotSkill = 8;
                Cost = 25;

                IsUnique = true;

                SkinName = "Gray";
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
            Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(Combat.Attacker, Combat.Defender, Combat.ChosenWeapon);
            if ((Combat.AttackStep == CombatStep.Attack) && (shotInfo.Range > 1))
            {
                result = true;
            }
            return result;
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                if (Combat.DiceRollAttack.Blanks > 0) result = 95;
            }

            return result;
        }

    }
}
