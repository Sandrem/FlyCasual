using Ship;
using Upgrade;
using UnityEngine;
using System.Linq;

namespace UpgradesList.FirstEdition
{
    public class Dengar : GenericUpgrade
    {
        public Dengar() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Dengar",
                UpgradeType.Crew,
                cost: 3,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Scum),
                abilityType: typeof(Abilities.FirstEdition.DengarCrewAbility)
            );

            Avatar = new AvatarInfo(Faction.Scum, new Vector2(16, 1));
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class DengarCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += DengarCrewActionEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= DengarCrewActionEffect;
        }

        private void DengarCrewActionEffect(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.DengarDiceModification
            {
                HostShip = host,
                ImageUrl = HostUpgrade.ImageUrl
            };
            host.AddAvailableDiceModification(newAction);
        }
    }
}

namespace ActionsList
{

    public class DengarDiceModification : GenericAction
    {

        public DengarDiceModification()
        {
            Name = DiceModificationName = "Dengar";

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
                NumberOfDiceCanBeRerolled = (Combat.Defender.PilotInfo.IsLimited) ? 2 : 1,
                CallBack = callBack
            };
            diceRerollManager.Start();
        }

    }

}