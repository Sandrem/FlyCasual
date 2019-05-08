using Ship;
using Upgrade;
using UnityEngine;
using System.Linq;
using Tokens;

namespace UpgradesList.FirstEdition
{
    public class LattsRazzi : GenericUpgrade
    {
        public LattsRazzi() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Latts Razzi",
                UpgradeType.Crew,
                cost: 2,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Scum),
                abilityType: typeof(Abilities.FirstEdition.LattsRazziCrewAbility)
            );

            Avatar = new AvatarInfo(Faction.Scum, new Vector2(37, 0));
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class LattsRazziCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddLattsRazziDiceModification;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddLattsRazziDiceModification;
        }

        private void AddLattsRazziDiceModification(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.LattsRazziDiceModification()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                HostShip = HostShip
            };
            host.AddAvailableDiceModification(newAction);
        }
    }
}

namespace ActionsList
{
    public class LattsRazziDiceModification : GenericAction
    {

        public LattsRazziDiceModification()
        {
            Name = DiceModificationName = "Latts Razzi";
        }

        public override void ActionEffect(System.Action callBack)
        {
            Messages.ShowInfo("Latts Razzi removes a stress token from the attacker to gain 1 Evade result");

            Combat.CurrentDiceRoll.ApplyEvade();

            Combat.Attacker.Tokens.RemoveToken(
                typeof(StressToken),
                callBack
            );
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Defence && Combat.Attacker.Tokens.HasToken(typeof(StressToken)))
            {
                result = true;
            }

            return result;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Defence && Combat.Attacker.Tokens.HasToken(typeof(StressToken)))
            {
                int attackSuccessesCancelable = Combat.DiceRollAttack.SuccessesCancelable;
                int defenceSuccesses = Combat.DiceRollDefence.Successes;
                if (attackSuccessesCancelable > defenceSuccesses)
                {
                    result = (attackSuccessesCancelable - defenceSuccesses == 1) ? 65 : 15;
                }
            }

            return result;
        }

    }
}