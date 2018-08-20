using System.Collections.Generic;
using Upgrade;
using Abilities;
using Ship;
using RuleSets;
using System;

namespace UpgradesList
{
    public class LoneWolf : GenericUpgrade, ISecondEditionUpgrade
    {
        public LoneWolf() : base()
        {
            isUnique = true;

            Types.Add(UpgradeType.Elite);
            Name = "Lone Wolf";
            Cost = 2;

            UpgradeAbilities.Add(new LoneWolfAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 4;
            MaxCharges = 1;
            UsesCharges = true;
            RegensCharges = true;

            UpgradeAbilities.RemoveAll(a => a is LoneWolfAbility);
            UpgradeAbilities.Add(new Abilities.SecondEdition.LoneWolfAbility());
        }
    }
}

namespace Abilities
{
    public class LoneWolfAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += LoneWolfActionEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= LoneWolfActionEffect;
        }

        private void LoneWolfActionEffect(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.LoneWolfActionEffect()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = host
            };
            host.AddAvailableDiceModification(newAction);
        }
    }
}

namespace ActionsList
{

    public class LoneWolfActionEffect : GenericAction
    {
        public LoneWolfActionEffect()
        {
            Name = DiceModificationName = "Lone Wolf";

            IsReroll = true;
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = true;

            foreach (var friendlyShip in Host.Owner.Ships)
            {
                if (friendlyShip.Value.ShipId != Host.ShipId)
                {
                    BoardTools.DistanceInfo distanceInfo = new BoardTools.DistanceInfo(Host, friendlyShip.Value);
                    if (distanceInfo.Range < 3)
                    {
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Defence)
            {
                if (Combat.DiceRollAttack.Successes > Combat.DiceRollDefence.Successes)
                {
                    if (Combat.CurrentDiceRoll.BlanksNotRerolled > 0) result = 95;
                }
            }

            if (Combat.AttackStep == CombatStep.Attack)
            {
                if (Combat.CurrentDiceRoll.BlanksNotRerolled > 0) result = 95;
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                NumberOfDiceCanBeRerolled = 1,
                SidesCanBeRerolled = new List<DieSide> { DieSide.Blank },
                CallBack = callBack
            };
            diceRerollManager.Start();
        }

    }

}

namespace Abilities.SecondEdition
{
    //While you defend or perform an attack, if there are no other friendly ships at range 0-2, you may spend 1 charge to reroll 1 of your dice.
    public class LoneWolfAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostUpgrade.Name,
                IsDiceModificationAvailable,
                GetDiceModificationAiPriority,
                DiceModificationType.Reroll,
                1, 
                payAbilityCost: PayAbilityCost
            );
        }

        private void PayAbilityCost(Action<bool> callback)
        {
            if (HostUpgrade.Charges > 0)
            {
                HostUpgrade.SpendCharge(() => callback(true));
            }
            else callback(false);
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        public bool IsDiceModificationAvailable()
        {
            var noFriendlyShipsInRange0to2 = true;

            foreach (var friendlyShip in HostShip.Owner.Ships)
            {
                if (friendlyShip.Value != HostShip)
                {
                    BoardTools.DistanceInfo distanceInfo = new BoardTools.DistanceInfo(HostShip, friendlyShip.Value);
                    if (distanceInfo.Range < 3)
                    {
                        noFriendlyShipsInRange0to2 = false;
                        break;
                    }
                }
            }

            return ((HostShip.IsAttacking || HostShip.IsDefending) && noFriendlyShipsInRange0to2 && HostUpgrade.Charges > 0);
        }

        public int GetDiceModificationAiPriority()
        {
            if (Combat.AttackStep == CombatStep.Attack)
            {
                return 80;
            }

            if (Combat.AttackStep == CombatStep.Defence)
            {
                return 85;
            }

            else return 0;
        }
    }
}