﻿using Movement;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace SecondEdition.ModifiedTIELnFighter
    {
        public class ModifiedTIELnFighter : FirstEdition.TIEFighter.TIEFighter
        {
            public ModifiedTIELnFighter() : base()
            {
                ShipInfo.ShipName = "Modified TIE/ln Fighter";

                ShipInfo.DefaultShipFaction = Faction.Scum;
                ShipInfo.FactionsAll = new List<Faction>() { Faction.Scum };

                DialInfo.RemoveManeuver(new ManeuverHolder(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn));
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed5, ManeuverDirection.Forward, ManeuverBearing.Straight), MovementComplexity.Complex);

                IconicPilots[Faction.Scum] = typeof(CaptainSeevor);

                ShipAbilities.Add(new Abilities.SecondEdition.ModifiedTIELnFighterAbility());

                ModelInfo = new ShipModelInfo(
                    "Modified TIE Fighter",
                    "Mining Guild",
                    new Vector3(-3.7f, 7.8f, 5.55f),
                    1.75f
                );

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/7/7a/Maneuver_modified_tie_ln_fighter.png";

                HotacManeuverTable = new AI.ModifiedTIELnFighterTable();
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ModifiedTIELnFighterAbility : GenericAbility
    {
        public override string Name { get { return "Notched Stabilizers"; } }

        public override void ActivateAbility()
        {
            HostShip.IgnoreObstacleTypes.Add(typeof(Obstacles.Asteroid));
            HostShip.IsIgnoreObstaclesDuringBarrelRoll = true;
            HostShip.IsIgnoreObstaclesDuringBoost = true;
        }

        public override void DeactivateAbility()
        {
            HostShip.IgnoreObstacleTypes.Remove(typeof(Obstacles.Asteroid));
            HostShip.IsIgnoreObstaclesDuringBarrelRoll = false;
            HostShip.IsIgnoreObstaclesDuringBoost = false;
        }
    }
}
