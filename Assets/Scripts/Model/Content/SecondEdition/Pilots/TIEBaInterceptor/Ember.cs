using System;
using System.Linq;
using ActionsList;
using BoardTools;
using Ship;
using SubPhases;
using Tokens;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEBaInterceptor
    {
        public class Ember : TIEBaInterceptor
        {
            public Ember() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Ember\"",
                    4,
                    52,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.EmberAbility)
                );

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/4de956edddeacb92ef3e4f94e0a63db3.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class EmberAbility : GenericAbility
    {
        // While you perform an attack, if there is a damaged ship friendly to the defender at range 0-1 of the defender,
        // the defender cannot spend focus or calculate tokens.

        public override void ActivateAbility()
        {
            GenericShip.OnTryAddAvailableDiceModificationGlobal += CheckRestriction;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnTryAddAvailableDiceModificationGlobal -= CheckRestriction;
        }

        private void CheckRestriction(GenericShip ship, GenericAction action, ref bool isAllowed)
        {
            if (ship == Combat.Defender &&  Combat.Attacker != null && Combat.Attacker.ShipId == HostShip.ShipId)
            {
                if (ship.Damage.IsDamaged() || Board.GetShipsAtRange(ship, new Vector2(0, 1), Team.Type.Friendly).Count(n => n.Damage.IsDamaged()) > 0)
                {
                    if (action.TokensSpend.Contains(typeof(FocusToken)) || action.TokensSpend.Contains(typeof(CalculateToken)))
                    {
                        Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": You cannot use " + action.Name + " action");
                        isAllowed = false;
                    }
                }
            }
        }
    }
}