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
                    abilityType: typeof(Abilities.SecondEdition.AvengerAbility),
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
            if (ship.Owner == HostShip.Owner)
            {
                RegisterAbilityTrigger(TriggerTypes.OnShipIsDestroyed, PerformAction);
            }
        }

        private void PerformAction(object sender, System.EventArgs e)
        {
            Messages.ShowInfoToHuman("\"Avenger\": a friendly ship was destroyed you may to perform an action");

            var ship = Selection.ThisShip;
            Selection.ChangeActiveShip(HostShip);
            List<GenericAction> actions = Selection.ThisShip.GetAvailableActions();
            foreach (GenericAction action in actions) { action.CanBePerformedWhileStressed = true; }
            HostShip.AskPerformFreeAction(actions, delegate {
                Selection.ChangeActiveShip(ship);
                Triggers.FinishTrigger();
                });
        }
    }
}