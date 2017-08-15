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

            Type = UpgradeSlot.Elite;
            Name = ShortName = "Lone Wolf";
            ImageUrl = "https://vignette3.wikia.nocookie.net/xwing-miniatures/images/6/60/Lone-wolf.png";
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
        public Ship.GenericShip Host;

        public LoneWolfActionEffect()
        {
            Name = EffectName = "Lone Wolf";
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

            if (Combat.CurentDiceRoll.Blanks > 0) result = 95;

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                NumberOfDicesCanBeRerolled = 1,
                SidesCanBeRerolled = new List<DiceSide> { DiceSide.Blank },
                CallBack = callBack
            };
            diceRerollManager.Start();
        }

    }

}

