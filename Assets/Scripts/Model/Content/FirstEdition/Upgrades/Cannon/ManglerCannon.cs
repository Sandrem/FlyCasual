using ActionsList;
using Ship;
using System.Linq;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class ManglerCannon : GenericSpecialWeapon
    {
        public ManglerCannon() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Mangler Cannon",
                UpgradeType.Cannon,
                cost: 4,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 3,
                    minRange: 1,
                    maxRange: 3
                ),
                abilityType: typeof(Abilities.FirstEdition.ManglerCannonAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class ManglerCannonAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += ManglerCannonAddDiceModification;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= ManglerCannonAddDiceModification;
        }

        private void ManglerCannonAddDiceModification(GenericShip ship)
        {
            ship.AddAvailableDiceModification(new ManglerCannonAction()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                HostShip = HostShip,
                Source = HostUpgrade
            });
        }
    }
}

namespace ActionsList
{
    public class ManglerCannonAction : GenericAction
    {

        public ManglerCannonAction()
        {
            Name = DiceModificationName = "Mangler Cannon";
        }

        public override bool IsDiceModificationAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack && Combat.ChosenWeapon == Source;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                int attackSuccesses = Combat.DiceRollAttack.RegularSuccesses;
                if (attackSuccesses > 0) result = 100;
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.ChangeOne(DieSide.Success, DieSide.Crit);
            callBack();
        }
    }
}