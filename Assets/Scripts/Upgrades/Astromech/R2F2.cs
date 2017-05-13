using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Upgrade
{

    public class R2F2 : GenericUpgrade
    {

        public R2F2(Ship.GenericShip host) : base(host)
        {
            Type = UpgradeSlot.Astromech;
            Name = ShortName = "R2-F2";
            isUnique = true;

            host.AfterAvailableActionListIsBuilt += R2F2AddAction;
        }

        private void R2F2AddAction(Ship.GenericShip host)
        {
            host.AddAvailableAction(new R2F2Action());
        }

    }

    public class R2F2Action : Actions.GenericAction
    {
        private Ship.GenericShip host;

        public R2F2Action()
        {
            Name = EffectName = "R2-F2: Increase Agility";
        }

        public override void ActionTake()
        {
            host = Game.Selection.ThisShip;
            host.ChangeAgility(+1);
            Game.PhaseManager.OnEndPhaseStart += R2F2DecreaseAgility;
        }

        private void R2F2DecreaseAgility()
        {
            host.ChangeAgility(-1);
        }

    }

}
