using Arcs;
using Ship;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class BarrageRockets : GenericSpecialWeapon
    {
        public BarrageRockets() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Barrage Rockets",
                types: new List<UpgradeType>(){
                    UpgradeType.Missile,
                    UpgradeType.Missile
                },
                cost: 6,
                charges: 5,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 3,
                    minRange: 2,
                    maxRange: 3,
                    requiresToken: typeof(FocusToken)
                ),
                abilityType: typeof(Abilities.SecondEdition.BarrageRocketsAbility),
                seImageNumber: 36
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class BarrageRocketsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostUpgrade.UpgradeInfo.Name,
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Reroll,
                GetRerollCount,
                payAbilityPostCost: PayAbilityCost
            );
        }

        private bool IsAvailable()
        {
            return Combat.ChosenWeapon == this
                && HostShip.SectorsInfo.IsShipInSector(Combat.Defender, ArcType.Bullseye)
                && HostUpgrade.State.Charges > 0
                && Combat.AttackStep == CombatStep.Attack;
        }

        private int GetAiPriority()
        {
            return 81; // Just a bit higher than TL
        }

        private int GetRerollCount()
        {
            return HostUpgrade.State.Charges;
        }

        private void PayAbilityCost()
        {
            for (int i = 0; i < DiceRoll.CurrentDiceRoll.WasSelectedCount; i++)
            {
                HostUpgrade.State.SpendCharge();
            }
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}