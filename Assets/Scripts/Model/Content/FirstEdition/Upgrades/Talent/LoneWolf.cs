using Upgrade;
using System.Collections.Generic;
using ActionsList;
using Ship;
using System.Linq;

namespace UpgradesList.FirstEdition
{
    public class LoneWolf : GenericUpgrade
    {
        public LoneWolf() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Lone Wolf",
                UpgradeType.Talent,
                cost: 2,
                isLimited: true,
                abilityType: typeof(Abilities.FirstEdition.LoneWolfAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
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
            GenericAction newAction = new LoneWolfActionEffect()
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

            foreach (var friendlyShip in HostShip.Owner.Ships)
            {
                if (friendlyShip.Value.ShipId != HostShip.ShipId)
                {
                    BoardTools.DistanceInfo distanceInfo = new BoardTools.DistanceInfo(HostShip, friendlyShip.Value);
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