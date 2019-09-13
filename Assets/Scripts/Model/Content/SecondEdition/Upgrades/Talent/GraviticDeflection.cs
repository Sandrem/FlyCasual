using Upgrade;
using System.Collections.Generic;
using Ship;
using BoardTools;

namespace UpgradesList.SecondEdition
{
    public class GraviticDeflection : GenericUpgrade
    {
        public GraviticDeflection() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Gravitic Deflection",
                UpgradeType.Talent,
                cost: 5,
                abilityType: typeof(Abilities.SecondEdition.GraviticDeflectionAbility),
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.NantexClassStarfighter.NantexClassStarfighter))
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/b0/6f/b06f34a5-7c10-4f97-a915-3b935b16d6ff/swz47_upgrade-gravitic-deflection.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class GraviticDeflectionAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                "Gravitic Deflection",
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Reroll,
                GetDiceCount
            );
        }

        private int GetDiceCount()
        {
            List<GenericShip> shipsInAttackArc = new List<GenericShip>();

            foreach (GenericShip ship in Roster.AllShips.Values)
            {
                ShotInfoArc inArcCheck = new ShotInfoArc(HostShip, ship, Combat.ArcForShot);
                if (inArcCheck.InArc) shipsInAttackArc.Add(ship);
            }

            return shipsInAttackArc.Count;
        }

        private int GetAiPriority()
        {
            return 90;
        }

        private bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Defence
                && GetDiceCount() > 0;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}