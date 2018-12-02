using ActionsList;
using Ship;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class ConcussionMissiles : GenericSpecialWeapon
    {
        public ConcussionMissiles() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Concussion Missiles",
                UpgradeType.Missile,
                cost: 4,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 4,
                    minRange: 2,
                    maxRange: 3,
                    requiresToken: typeof(BlueTargetLockToken),
                    spendsToken: typeof(BlueTargetLockToken),
                    discard: true
                ),
                abilityType: typeof(Abilities.FirstEdition.ConcussionMissilesAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class ConcussionMissilesAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddConcussionMissilesDiceModification;
        }

        public override void DeactivateAbility()
        {
            // Ability is turned off only after full attack is finished
            HostShip.OnCombatDeactivation += DeactivateAbilityPlanned;
        }

        private void DeactivateAbilityPlanned(GenericShip ship)
        {
            HostShip.OnCombatDeactivation -= DeactivateAbilityPlanned;
            HostShip.OnGenerateDiceModifications -= AddConcussionMissilesDiceModification;
        }

        private void AddConcussionMissilesDiceModification(GenericShip host)
        {
            ConcussionMissilesAction action = new ConcussionMissilesAction()
            {
                Host = host,
                ImageUrl = HostUpgrade.ImageUrl,
                Source = HostUpgrade
            };

            host.AddAvailableDiceModification(action);
        }
    }
}

namespace ActionsList
{

    public class ConcussionMissilesAction : GenericAction
    {

        public ConcussionMissilesAction()
        {
            Name = DiceModificationName = "Concussion Missiles";
        }

        public void AddDiceModification()
        {
            Host.OnGenerateDiceModifications += ConcussionMissilesAddDiceModification;
        }

        private void ConcussionMissilesAddDiceModification(GenericShip ship)
        {
            ship.AddAvailableDiceModification(this);
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = true;

            if (Combat.AttackStep != CombatStep.Attack) result = false;

            if (Combat.ChosenWeapon != Source) result = false;

            return result;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                int attackBlanks = Combat.DiceRollAttack.Blanks;
                if (attackBlanks > 0)
                {
                    if ((attackBlanks == 1) && (Combat.Attacker.GetDiceModificationsGenerated().Count(n => n.IsTurnsAllFocusIntoSuccess) == 0))
                    {
                        result = 100;
                    }
                    else
                    {
                        result = 55;
                    }
                }
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.ChangeOne(DieSide.Blank, DieSide.Success);
            callBack();
        }

    }

}
