using Upgrade;
using Ship;
using System.Linq;
using Tokens;
using System.Collections.Generic;
using UnityEngine;
using SquadBuilderNS;
using System;
using Abilities;
using Board;
using SubPhases;
using ActionsList;

namespace UpgradesList
{
    public class TacticalOfficer : GenericActionBarUpgrade<CoordinateAction>
    {
        public TacticalOfficer() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Tactical Officer";
            Cost = 2;

            //AvatarOffset = new Vector2(45, 1);
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Imperial;
        }
    }
}