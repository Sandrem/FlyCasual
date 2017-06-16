using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEFighter
    {
        public class Howlrunner : TIEFighter
        {
            public Howlrunner() : base()
            {
                PilotName = "\"Howlrunner\"";
                ImageUrl = "https://vignette4.wikia.nocookie.net/xwing-miniatures/images/7/71/Howlrunner.png";
                IsUnique = true;
                PilotSkill = 8;
                Cost = 18;
                AddUpgradeSlot(Upgrade.UpgradeSlot.Elite);
            }

            public override void InitializePilot()
            {
                base.InitializePilot();
                // TODO
                // create global event on attack trigger
            }

            private void HowlrunnerAbility(GenericShip ship)
            {
                if (ship.Owner.PlayerNo == this.Owner.PlayerNo)
                {
                    if (Actions.GetRange(ship, this) == 1)
                    {
                        // TODO
                        // add action
                    }
                }
                
            }

        }
    }
}

namespace ActionsList
{

    public class HowlrunnerAction : GenericAction
    {
        private Ship.GenericShip host;

        public HowlrunnerAction()
        {
            Name = EffectName = "Howlrunner";
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;
            // TODO:
            // Check contitions?
            return result;
        }

        public override void ActionEffect()
        {
            // TODO:
            // Add interface
            // Combat.CurentDiceRoll.RerollOneSeleceted
        }

    }

}
