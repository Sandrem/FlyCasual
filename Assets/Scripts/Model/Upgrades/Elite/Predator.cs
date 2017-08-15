using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class Predator : GenericUpgrade
    {

        public Predator() : base()
        {
            Type = UpgradeSlot.Elite;
            Name = ShortName = "Predator";
            ImageUrl = "https://vignette2.wikia.nocookie.net/xwing-miniatures/images/e/e9/Predator.png";
            Cost = 3;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGenerateAvailableActionEffectsList += PredatorActionEffect;
        }

        private void PredatorActionEffect(Ship.GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.PredatorActionEffect();
            newAction.ImageUrl = ImageUrl;
            host.AddAvailableActionEffect(newAction);
        }

    }
}

namespace ActionsList
{

    public class PredatorActionEffect : GenericAction
    {
        private Ship.GenericShip host;

        public PredatorActionEffect()
        {
            Name = EffectName = "Predator";
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;
            if (Combat.AttackStep == CombatStep.Attack) result = true;
            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                NumberOfDicesCanBeRerolled = (Combat.Defender.PilotSkill > 2) ? 1 : 2,
                CallBack = callBack
            };
            diceRerollManager.Start();
        }

    }

}

