using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class LoneWolf : GenericUpgrade
    {

        public LoneWolf() : base()
        {
            isUnique = true;

            Type = UpgradeType.Elite;
            Name = "Lone Wolf";
            Cost = 2;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGenerateAvailableActionEffectsList += LoneWolfActionEffect;
        }

        private void LoneWolfActionEffect(Ship.GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.LoneWolfActionEffect()
            {
                ImageUrl = ImageUrl,
                Host = host
            };
            host.AddAvailableActionEffect(newAction);
        }

    }
}

namespace ActionsList
{

    public class LoneWolfActionEffect : GenericAction
    {
        public LoneWolfActionEffect()
        {
            Name = EffectName = "Lone Wolf";

            IsReroll = true;
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = true;

            foreach (var friendlyShip in Host.Owner.Ships)
            {
                if (friendlyShip.Value.ShipId != Host.ShipId)
                {
                    Board.ShipDistanceInformation distanceInfo = new Board.ShipDistanceInformation(Host, friendlyShip.Value);
                    if (distanceInfo.Range < 3)
                    {
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Defence)
            {
                if (Combat.DiceRollAttack.Successes > Combat.DiceRollDefence.Successes)
                {
                    if (Combat.CurrentDiceRoll.BlanksNotRerolled > 0) result = 95;
                }
            }

            if (Combat.AttackStep == CombatStep.Attack)
            {
                if (Combat.CurrentDiceRoll.BlanksNotRerolled > 0) result = 95;
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                NumberOfDiceCanBeRerolled = 1,
                SidesCanBeRerolled = new List<DieSide> { DieSide.Blank },
                CallBack = callBack
            };
            diceRerollManager.Start();
        }

    }

}

