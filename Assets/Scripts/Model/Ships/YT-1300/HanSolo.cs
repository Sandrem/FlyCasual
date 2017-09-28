using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace YT1300
    {
        public class HanSolo : YT1300
        {
            public HanSolo() : base()
            {
                PilotName = "Han Solo";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Rebel%20Alliance/YT-1300/han-solo.png";
                PilotSkill = 9;
                Cost = 46;

                IsUnique = true;

                Firepower = 3;
                MaxHull = 8;
                MaxShields = 5;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }

            public override void InitializePilot()
            {
                base.InitializePilot();
                AfterGenerateAvailableActionEffectsList += HanSoloPilotAbility;
            }

            public void HanSoloPilotAbility(GenericShip ship)
            {
                ship.AddAvailableActionEffect(new PilotAbilities.HanSoloAction());
            }
        }
    }
}

namespace PilotAbilities
{
    public class HanSoloAction : ActionsList.GenericAction
    {
        public HanSoloAction()
        {
            Name = EffectName = "Han Solo's ability";
            IsReroll = true;
        }

        public override void ActionEffect(System.Action callBack)
        {
            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                CallBack = callBack
            };
            diceRerollManager.Start();
            SelectAllRerolableDices();
            diceRerollManager.ConfirmReroll();
        }

        private static void SelectAllRerolableDices()
        {
            Combat.CurentDiceRoll.SelectBySides
            (
                new List<DieSide>(){
                    DieSide.Blank,
                    DieSide.Focus,
                    DieSide.Success,
                    DieSide.Crit
                },
                int.MaxValue
            );
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Attack && Combat.DiceRollAttack.NotRerolled > 0)
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
