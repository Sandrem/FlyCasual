using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Alpha3BBesh : GenericUpgrade
    {
        public Alpha3BBesh() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Alpha-3B \"Besh\"",
                UpgradeType.Configuration,
                cost: 0,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.NimbusClassVWing.NimbusClassVWing)),
                abilityType: typeof(Abilities.SecondEdition.Alpha3BBeshAbility),
                addSlot: new UpgradeSlot(UpgradeType.Device)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/6d/43/6d434e52-68f4-4b8d-9166-4365fb920625/swz80_upgrade_alpha-3b.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class Alpha3BBeshAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                "Alpha-3B \"Besh\"",
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Change,
                count: 1,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Blank, DieSide.Focus },
                sideCanBeChangedTo: DieSide.Success,
                payAbilityCost: SpendLock
            );
        }

        private bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack
                && Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon
                && ActionsHolder.HasTargetLockOn(HostShip, Combat.Defender);
        }

        private int GetAiPriority()
        {
            return 0;
        }

        private void SpendLock(Action<bool> callback)
        {
            List<char> letters = ActionsHolder.GetTargetLocksLetterPairs(HostShip, Combat.Defender);
            if (letters.Count > 0)
            {
                HostShip.Tokens.SpendToken(typeof(BlueTargetLockToken), delegate { callback(true); }, letters.First());
            }
            else
            {
                Messages.ShowError("No lock to spend");
                callback(false);
            }
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}