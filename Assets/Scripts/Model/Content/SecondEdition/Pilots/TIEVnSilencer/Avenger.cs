﻿using ActionsList;
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
                    57,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.AvengerAbility),
                    extraUpgradeIcon: UpgradeType.Talent //,
                                                         //seImageNumber: 120
                );

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/d90d3057ead18b5df5f6de55a199a4cd.png";
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
            GenericShip.OnShipIsDestroyedGlobal += RegisterOnDestroyedFriendly;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnShipIsDestroyedGlobal -= RegisterOnDestroyedFriendly;
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
            var ship = Selection.ThisShip;
            Roster.HighlightPlayer(HostShip.Owner.PlayerNo);
            Selection.ChangeActiveShip(HostShip);
            bool oldValue = HostShip.CanPerformActionsWhileStressed;
            HostShip.CanPerformActionsWhileStressed = true;
            //List<GenericAction> actions = Selection.ThisShip.ActionBar.AllActions;
            List<GenericAction> actions = Selection.ThisShip.GetAvailableActions();
            HostShip.CanPerformActionsWhileStressed = oldValue;

            foreach (GenericAction action in actions)
            {
                action.CanBePerformedWhileStressed = true;
            }

            HostShip.AskPerformFreeAction(
                actions,
                delegate {
                    Roster.HighlightPlayer(ship.Owner.PlayerNo);
                    Selection.ChangeActiveShip(ship);
                    Triggers.FinishTrigger();
                },
                HostShip.PilotInfo.PilotName,
                "After another friendly ship is destroyed, you may perform an action, even while stressed",
                HostShip
            );
        }
    }
}