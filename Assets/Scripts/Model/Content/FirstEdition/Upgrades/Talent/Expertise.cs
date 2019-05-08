using Ship;
using Upgrade;
using System.Collections.Generic;
using ActionsList;

namespace UpgradesList.FirstEdition
{
    public class Expertise : GenericUpgrade
    {
        public Expertise() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Expertise",
                UpgradeType.Talent,
                cost: 4,
                abilityType: typeof(Abilities.FirstEdition.ExpertiseAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class ExpertiseAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddExpertiseDiceModification;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddExpertiseDiceModification;
        }

        private void AddExpertiseDiceModification(GenericShip host)
        {
            GenericAction newAction = new ExpertiseDiceModification
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

    public class ExpertiseDiceModification : GenericAction
    {

        public ExpertiseDiceModification()
        {
            Name = DiceModificationName = "Expertise";

            IsTurnsAllFocusIntoSuccess = true;
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
                if (!HostShip.Tokens.HasToken(typeof(Tokens.StressToken)))
                {
                    int attackFocuses = Combat.DiceRollAttack.Focuses;
                    if (attackFocuses > 0) result = 55;
                }
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            if (!HostShip.Tokens.HasToken(typeof(Tokens.StressToken)))
            {
                Combat.CurrentDiceRoll.ChangeAll(DieSide.Focus, DieSide.Success);
            }
            else
            {
                Messages.ShowErrorToHuman(HostShip.PilotInfo.PilotName + " cannot use Expertise while stressed");
            }
            callBack();
        }

    }

}