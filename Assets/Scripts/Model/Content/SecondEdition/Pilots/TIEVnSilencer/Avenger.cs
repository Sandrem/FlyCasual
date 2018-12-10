using ActionsList;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEVnSilencer
    {
        public class Avenger : TIEVnSilencer
        {
            public Avenger() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Avenger\"",
                    3,
                    62,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.TestPilotBlackoutAbility),
                    extraUpgradeIcon: UpgradeType.Talent //,
                                                         //seImageNumber: 120
                );

                ImageUrl = "http://infinitearenas.com/xw2browse/images/first-order/avenger.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AvengerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnDestroyedGlobal += RegisterOnDestroyedFriendly;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnDestroyedGlobal -= RegisterOnDestroyedFriendly;
        }

        protected void RegisterOnDestroyedFriendly(GenericShip ship, bool isFled)
        {
            Console.Write("Checking if Destroyed Ship was friendly");
            if (ship.Owner == HostShip.Owner)
            {
                Console.Write("Registering Perform Action");
                RegisterAbilityTrigger(TriggerTypes.OnShipIsDestroyed, PerformAction);
            }
        }

        private void PerformAction(object sender, System.EventArgs e)
        {
            Messages.ShowInfoToHuman("\"Avenger\": a friendly ship was destroyed you may to perform an action");

            List<GenericAction> actions = Selection.ThisShip.GetAvailableActions();
            foreach (GenericAction action in actions) { action.CanBePerformedWhileStressed = true; }
            HostShip.AskPerformFreeAction(actions, Triggers.FinishTrigger);
        }
    }
}