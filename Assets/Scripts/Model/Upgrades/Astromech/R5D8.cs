using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class R5D8 : GenericUpgrade
    {

        public R5D8() : base()
        {
            IsHidden = true;

            Type = UpgradeSlot.Astromech;
            Name = ShortName = "R5-D8";
            ImageUrl = "https://vignette3.wikia.nocookie.net/xwing-miniatures/images/f/ff/R5-D8.jpg";
            isUnique = true;
            Cost = 3;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGenerateAvailableActionsList += R5D8AddAction;
        }

        private void R5D8AddAction(Ship.GenericShip host)
        {
            ActionsList.GenericAction action = new ActionsList.R5D8Action();
            action.ImageUrl = ImageUrl;
            host.AddAvailableAction(action);
        }

    }

}

namespace ActionsList
{

    public class R5D8Action : GenericAction
    {
        private Ship.GenericShip host;

        public R5D8Action()
        {
            Name = EffectName = "R5-D8: Try to repair";
        }

        public override void ActionTake()
        {
            // TODO:
            // Astromech sound
            // Visual dice throwing
            int randomValue = Random.Range(0, 8);
            // Notification about result
            if (randomValue > 2)
            {
                // Remove random damage card
            }
            Phases.CurrentSubPhase.callBack();
        }

    }

}
