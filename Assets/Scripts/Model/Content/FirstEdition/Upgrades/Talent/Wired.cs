﻿using Upgrade;
using System.Collections.Generic;
using ActionsList;
using Ship;
using System.Linq;

namespace UpgradesList.FirstEdition
{
    public class Wired : GenericUpgrade
    {
        public Wired() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Wired",
                UpgradeType.Talent,
                cost: 1,
                abilityType: typeof(Abilities.FirstEdition.WiredAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class WiredAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += WiredActionEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= WiredActionEffect;
        }

        private void WiredActionEffect(GenericShip host)
        {
            GenericAction newAction = new WiredActionEffect()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                HostShip = host
            };
            host.AddAvailableDiceModification(newAction);
        }
    }
}

namespace ActionsList
{
    public class WiredActionEffect : GenericAction
    {

        public WiredActionEffect()
        {
            Name = DiceModificationName = "Wired";
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.Attacker.Tokens.HasToken(typeof(Tokens.StressToken)))
            {
                if (Combat.DiceRollAttack.Focuses > 0 && Combat.Attacker.GetDiceModificationsGenerated().Count(n => n.IsTurnsAllFocusIntoSuccess) == 0)
                {
                    result = 95;
                }
                else
                {
                    result = 30;
                }
            }

            return result;
        }

        public override bool IsDiceModificationAvailable()
        {
            return HostShip.Tokens.HasToken(typeof(Tokens.StressToken));
        }

        public override void ActionEffect(System.Action callBack)
        {
            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                SidesCanBeRerolled = new System.Collections.Generic.List<DieSide> { DieSide.Focus },
                CallBack = callBack
            };
            diceRerollManager.Start();
        }

    }

}
