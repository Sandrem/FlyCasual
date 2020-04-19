using Upgrade;
using UnityEngine;
using Ship;
using System;
using ActionsList;

namespace UpgradesList.SecondEdition
{
    public class M9G8 : GenericUpgrade
    {
        public M9G8() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "M9-G8",
                UpgradeType.Astromech,
                cost: 7,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.M9G8Ability),
                restriction: new FactionRestriction(Faction.Resistance)
            );

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/cd67f33e8aa52d2aeb07f432125a8c73.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class M9G8Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal += AddFriendlyDiceModification;
            GenericShip.OnGenerateDiceModificationsOppositeGlobal += AddEnemyDiceModification;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal -= AddFriendlyDiceModification;
            GenericShip.OnGenerateDiceModificationsOppositeGlobal -= AddEnemyDiceModification;
        }

        private void AddEnemyDiceModification(GenericShip ship)
        {
            Combat.Defender.AddAvailableDiceModification(
                new M9G8EnemyDiceModification()
                {
                    GrantedBy = HostShip
                },
                Combat.Attacker
            );
        }

        private void AddFriendlyDiceModification(GenericShip ship)
        {
            Combat.Attacker.AddAvailableDiceModification(
                new M9G8FriendlyDiceModification() {
                    GrantedBy = HostShip
                },
                Combat.Attacker
            );
        }

    }
}

namespace ActionsList
{

    public class M9G8FriendlyDiceModification : GenericAction
    {
        public GenericShip GrantedBy;

        public M9G8FriendlyDiceModification()
        {
            Name = DiceModificationName = "M9-G8";

            IsReroll = true;
        }

        public override bool IsDiceModificationAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack
                && ActionsHolder.HasTargetLockOn(GrantedBy, HostShip)
                && Combat.Attacker.Owner.PlayerNo == GrantedBy.Owner.PlayerNo;
        }

        public override int GetDiceModificationPriority()
        {
            return 90;
        }

        public override void ActionEffect(System.Action callBack)
        {
            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                NumberOfDiceCanBeRerolled = 1,
                CallBack = callBack
            };

            diceRerollManager.Start();
        }

    }

    public class M9G8EnemyDiceModification : GenericAction
    {
        public GenericShip GrantedBy;

        public M9G8EnemyDiceModification()
        {
            Name = DiceModificationName = "M9-G8";

            IsReroll = true;
            DiceModificationTiming = DiceModificationTimingType.Opposite;
        }

        public override bool IsDiceModificationAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack
                && ActionsHolder.HasTargetLockOn(GrantedBy, HostShip)
                && Combat.Defender.Owner.PlayerNo == GrantedBy.Owner.PlayerNo;
        }

        public override int GetDiceModificationPriority()
        {
            return 90;
        }

        public override void ActionEffect(System.Action callBack)
        {
            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                NumberOfDiceCanBeRerolled = 1,
                IsOpposite = true,
                CallBack = callBack
            };

            diceRerollManager.Start();
        }

    }

}