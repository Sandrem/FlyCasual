using System;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class ShatteringShot : GenericUpgrade
    {
        public ShatteringShot() : base()
        {
            UpgradeInfo = new UpgradeCardInfo
            (
                "Shattering Shot",
                UpgradeType.ForcePower,
                cost: 3,
                abilityType: typeof(Abilities.SecondEdition.ShatteringShotAbility)                
            );

            ImageUrl = "https://i.imgur.com/6NeEkwu.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ShatteringShotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification
            (
                HostUpgrade.UpgradeInfo.Name,
                IsAvailable,
                AiPriority,
                DiceModificationType.Add,
                1,
                sideCanBeChangedTo: DieSide.Focus, 
                payAbilityCost: SpendForce
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private bool IsAvailable()
        {
            if (HostShip.State.Force == 0) return false;

            if (Combat.AttackStep != CombatStep.Attack) return false;

            if (!Combat.ShotInfo.IsObstructedByObstacle && !Combat.Attacker.IsLandedOnObstacle) return false;

            return true;
        }

        private void SpendForce(Action<bool> callback)
        {
            if (HostShip.State.Force > 0)
            {
                HostShip.State.SpendForce(1, delegate { callback(true); });
            }
            else
            {
                callback(false);
            }
        }

        private int AiPriority()
        {
            if (HostShip.Tokens.HasToken<Tokens.FocusToken>() || HasEnoughForceToTurnAllNewEyesIntoHits()) return 110; else return 0;
        }

        private bool HasEnoughForceToTurnAllNewEyesIntoHits()
        {
            return ((HostShip.State.Force - 1) > (Combat.DiceRollAttack.FocusesNotRerolled + 1));
        }
    }
}