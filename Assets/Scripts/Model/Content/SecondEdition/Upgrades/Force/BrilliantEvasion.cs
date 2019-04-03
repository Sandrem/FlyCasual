using Arcs;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class BrilliantEvasion : GenericUpgrade, IVariableCost
    {
        public BrilliantEvasion() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Brilliant Evasion",
                UpgradeType.Force,
                cost: 6,
                abilityType: typeof(Abilities.SecondEdition.BrilliantEvasion)
                //seImageNumber: 19
                
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/d0/a4/d0a49094-b246-4345-9f65-846b070e9fc6/swz34_brilliant-evasion.png";
        }

        public void UpdateCost(GenericShip ship)
        {
            Dictionary<int, int> agilityToCost = new Dictionary<int, int>()
            {
                {0, 0},
                {1, 2},
                {2, 4},
                {3, 6}
            };

            UpgradeInfo.Cost = agilityToCost[ship.ShipInfo.Agility];
        }
    }
}

namespace Abilities.SecondEdition
{
    //While you defend, if you are not in the attacker's bullseye arc, you may spend 1 force to change two of your focus results to evade results.
    public class BrilliantEvasion : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostName,
                IsAvailable,
                AiPriority,
                DiceModificationType.Change,
                2,
                new List<DieSide> { DieSide.Focus },
                DieSide.Success, 
                payAbilityCost: SpendForce);
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private bool IsAvailable()
        {
            if (HostShip.State.Force == 0) return false;

            if (Combat.AttackStep != CombatStep.Defence) return false;

            if (Combat.Defender != HostShip) return false;

            if (Combat.Attacker.SectorsInfo.IsShipInSector(Combat.Defender, ArcType.Bullseye)) return false;

            return true;
        }

        private void SpendForce(Action<bool> callback)
        {
            if (HostShip.State.Force > 0)
            {
                HostShip.State.Force--;
                callback(true);
            }
            else
            {
                callback(false);
            }
        }

        private int AiPriority()
        {
            int focuses = Combat.DiceRollAttack.FocusesNotRerolled;

            if (focuses == 1) return 45;

            if (focuses > 1) return 35;

            return 0;
        }
    }
}