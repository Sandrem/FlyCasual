using ActionsList;
using Ship;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class ProtonTorpedoes : GenericSpecialWeapon
    {
        public ProtonTorpedoes() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Proton Torpedoes",
                UpgradeType.Torpedo,
                cost: 9,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 4,
                    minRange: 2,
                    maxRange: 3,
                    charges: 2,
                    requiresToken: typeof(BlueTargetLockToken)
                ),
                abilityType: typeof(Abilities.SecondEdition.ProtonTorpedoesAbility),
                seImageNumber: 35
            );
        }        
    }
}

namespace Abilities
{
    namespace SecondEdition
    {
        public class ProtonTorpedoesAbility : FirstEdition.ProtonTorpedoesAbility
        {
            protected override void AddProtonTorpedoesDiceMofification(GenericShip host)
            {
                ProtonTorpedoesDiceModificationSE action = new ProtonTorpedoesDiceModificationSE()
                {
                    Host = host,
                    ImageUrl = HostUpgrade.ImageUrl,
                    Source = HostUpgrade
                };

                host.AddAvailableDiceModification(action);
            }
        }
    }
}

namespace ActionsList
{
    public class ProtonTorpedoesDiceModificationSE : ProtonTorpedoesDiceModification
    {
        public ProtonTorpedoesDiceModificationSE()
        {
            IsTurnsOneFocusIntoSuccess = false;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                if (Combat.DiceRollAttack.RegularSuccesses > 0) result = 100;
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