using Ship;
using Upgrade;
using ActionsList;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class RoseTico : GenericUpgrade
    {
        public RoseTico() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Rose Tico",
                UpgradeType.Crew,
                cost: 9,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Resistance),
                abilityType: typeof(Abilities.SecondEdition.RoseTicoAbility)
            );

            Avatar = new AvatarInfo(
                Faction.Resistance,
                new Vector2(301, 3)
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class RoseTicoAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddRoseTicoDiceModification;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddRoseTicoDiceModification;
        }

        private void AddRoseTicoDiceModification(GenericShip host)
        {
            GenericAction diceModification = new ActionsList.SecondEdition.RoseTicoDiceModification()
            {
                HostShip = host,
                ImageUrl = HostUpgrade.ImageUrl
            };
            host.AddAvailableDiceModificationOwn(diceModification);
        }
    }
}

namespace ActionsList.SecondEdition
{
    public class RoseTicoDiceModification : GenericAction
    {
        public RoseTicoDiceModification()
        {
            Name = DiceModificationName = "Rose Tico's Ability";
        }

        public override bool IsDiceModificationAvailable()
        {
            return Combat.CurrentDiceRoll.Count > 0;
        }

        public override int GetDiceModificationPriority()
        {
            // TODO: Improve AI

            int result = 0;

            GenericShip opponentShip = (Combat.AttackStep == CombatStep.Attack) ? Combat.Defender : Combat.Attacker;

            if (Combat.CurrentDiceRoll.WorstResult == DieSide.Blank || Combat.CurrentDiceRoll.WorstResult == DieSide.Focus
                && !ActionsHolder.HasTargetLockOn(HostShip, opponentShip))
            {
                result = 1;
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            DieSide worstDieSide = Combat.CurrentDiceRoll.WorstResult;
            Combat.CurrentDiceRoll.RemoveType(worstDieSide);

            GenericShip opponentShip = (Combat.AttackStep == CombatStep.Attack) ? Combat.Defender : Combat.Attacker;
            ActionsHolder.AcquireTargetLock(HostShip, opponentShip, callBack, callBack);
        }

    }

}