﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Abilities;
using Ship;

namespace UpgradesList
{
    public class LoneWolf : GenericUpgrade
    {
        public LoneWolf() : base()
        {
            isUnique = true;

            Types.Add(UpgradeType.Elite);
            Name = "Lone Wolf";
            Cost = 2;

            UpgradeAbilities.Add(new LoneWolfAbility());
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

