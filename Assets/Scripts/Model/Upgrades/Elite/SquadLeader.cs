using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class SquadLeader : GenericUpgrade
    {

        public SquadLeader() : base()
        {
            IsHidden = true;

            Type = UpgradeSlot.Elite;
            Name = ShortName = "Squad Leader";
            ImageUrl = "https://vignette2.wikia.nocookie.net/xwing-miniatures/images/c/cd/Squad_Leader.png";
            isUnique = true;
            Cost = 2;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGenerateAvailableActionsList += SquadLeaderAddAction;
        }

        private void SquadLeaderAddAction(Ship.GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.SquadLeaderAction();
            newAction.ImageUrl = ImageUrl;
            host.AddAvailableAction(newAction);
        }

    }

}

namespace ActionsList
{

    public class SquadLeaderAction : GenericAction
    {
        private Ship.GenericShip host;

        public SquadLeaderAction()
        {
            Name = EffectName = "Squad Leader";
        }

        public override void ActionTake()
        {
            // Select ally and distance 1-2 to give it free action
            Phases.CurrentSubPhase.callBack();
        }

    }

}
