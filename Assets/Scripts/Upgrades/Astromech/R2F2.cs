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

        private void R2F2AddAction(Ship.GenericShip host, bool afterMovement)
        {
            if (host.CanPerformFreeAction("R2-F2: Increase Agility", afterMovement)) host.AvailableActionsList.Add("R2-F2: Increase Agility", R2F2IncreaseAgility);
            Game.PhaseManager.OnEndPhaseStart += R2F2DecreaseAgility;
        }

        private void R2F2IncreaseAgility()
        {
            Host.ChangeAgility(+1);
        }

        private void R2F2DecreaseAgility()
        {
            Host.ChangeAgility(-1);
            Game.PhaseManager.OnEndPhaseStart -= R2F2DecreaseAgility;
        }


    }

}
