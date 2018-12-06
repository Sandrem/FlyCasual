using Upgrade;
using System.Collections.Generic;
using ActionsList;
using Ship;
using System.Linq;

namespace UpgradesList.FirstEdition
{
    public class Predator : GenericUpgrade
    {
        public Predator() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Predator",
                UpgradeType.Talent,
                cost: 3,
                abilityType: typeof(Abilities.FirstEdition.PredatorAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class PredatorAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += PredatorActionEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= PredatorActionEffect;
        }

        private void PredatorActionEffect(GenericShip host)
        {
            GenericAction newAction = new PredatorActionEffect
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

    public class PredatorActionEffect : GenericAction
    {

        public PredatorActionEffect()
        {
            Name = DiceModificationName = "Predator";

            // Used for abilities like Dark Curse's that can prevent rerolls
            IsReroll = true;
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;
            if (Combat.AttackStep == CombatStep.Attack) result = true;
            return result;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                int attackFocuses = Combat.DiceRollAttack.FocusesNotRerolled;
                int attackBlanks = Combat.DiceRollAttack.BlanksNotRerolled;

                //if (Combat.Attacker.HasToken(typeof(Tokens.FocusToken)))
                if (Combat.Attacker.GetDiceModificationsGenerated().Count(n => n.IsTurnsAllFocusIntoSuccess) > 0)
                {
                    if (attackBlanks > 0) result = 90;
                }
                else
                {
                    if (attackBlanks + attackFocuses > 0) result = 90;
                }
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                NumberOfDiceCanBeRerolled = (Combat.Defender.State.Initiative > 2) ? 1 : 2,
                CallBack = callBack
            };
            diceRerollManager.Start();
        }

    }

}