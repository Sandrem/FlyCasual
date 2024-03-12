﻿using ActionsList;
using BoardTools;
using Content;
using Ship;
using System.Collections.Generic;
using System.Linq;
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
                PilotInfo = new PilotCardInfo25
                (
                    "\"Ember\"",
                    "Dying Flame",
                    Faction.FirstOrder,
                    4,
                    4,
                    7,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.EmberAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Tech,
                        UpgradeType.Missile,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );
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
                if (ship.Damage.IsDamaged || Board.GetShipsAtRange(ship, new Vector2(0, 1), Team.Type.Friendly).Count(n => n.Damage.IsDamaged) > 0)
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