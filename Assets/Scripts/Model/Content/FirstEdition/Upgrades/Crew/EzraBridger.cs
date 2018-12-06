using Ship;
using Upgrade;
using UnityEngine;
using ActionsList;
using System;
using Tokens;

namespace UpgradesList.FirstEdition
{
    public class EzraBridger : GenericUpgrade
    {
        public EzraBridger() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Ezra Bridger",
                UpgradeType.Crew,
                cost: 3,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.FirstEdition.EzraBridgerCrewAbility)
            );

            Avatar = new AvatarInfo(Faction.Rebel, new Vector2(7, 2));
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class EzraBridgerCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += EzraBridgerActionEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= EzraBridgerActionEffect;
        }

        private void EzraBridgerActionEffect(GenericShip host)
        {
            GenericAction newAction = new EzraBridgerAction()
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
    public class EzraBridgerAction : GenericAction
    {

        public EzraBridgerAction()
        {
            Name = DiceModificationName = "Ezra Bridger";
        }

        public override void ActionEffect(Action callBack)
        {
            if (HostShip.Tokens.HasToken(typeof(StressToken)))
            {
                Combat.CurrentDiceRoll.ChangeOne(DieSide.Focus, DieSide.Crit);
            }
            else
            {
                Messages.ShowErrorToHuman("Cannot use: not stressed");
            }
            callBack();
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                result = true;
            }

            return result;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack && HostShip.Tokens.HasToken(typeof(StressToken)))
            {
                if (Combat.DiceRollAttack.RegularSuccesses > 0) result = 100;
            }

            return result;
        }

    }
}