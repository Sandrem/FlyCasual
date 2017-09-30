using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace Firespray31
    {
        public class KrassisTrelix : Firespray31
        {
            public KrassisTrelix() : base()
            {
                PilotName = "Krassis Trelix";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Galactic%20Empire/Firespray-31/krassis-trelix.png";
                PilotSkill = 5;
                Cost = 36;

                SkinName = "Krassis Trelix";

                faction = Faction.Empire;
            }

            public override void InitializePilot()
            {
                base.InitializePilot();
                AfterGenerateAvailableActionEffectsList += KrassisTrelixPilotAbility;
            }

            public void KrassisTrelixPilotAbility(GenericShip ship)
            {
                ship.AddAvailableActionEffect(new PilotAbilities.KrassisTrelixAction());
            }
        }
    }
}

namespace PilotAbilities
{
    public class KrassisTrelixAction : ActionsList.GenericAction
    {
        public KrassisTrelixAction()
        {
            Name = EffectName = "Krassis Trelix's ability";
            IsReroll = true;
        }

        public override void ActionEffect(System.Action callBack)
        {
            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                NumberOfDiceCanBeRerolled = 1,
                CallBack = callBack
            };
            diceRerollManager.Start();
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;
            if (Combat.AttackStep == CombatStep.Attack && (Combat.ChosenWeapon as Upgrade.GenericSecondaryWeapon) != null)
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