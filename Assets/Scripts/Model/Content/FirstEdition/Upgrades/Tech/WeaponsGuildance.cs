using ActionsList;
using Ship;
using Tokens;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class WeaponsGuildance : GenericUpgrade
    {
        public WeaponsGuildance() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Weapons Guildance",
                UpgradeType.Tech,
                cost: 2,
                abilityType: typeof(Abilities.FirstEdition.WeaponsGuildanceAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class WeaponsGuildanceAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += WeaponsGuidanceActionEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= WeaponsGuidanceActionEffect;
        }

        private void WeaponsGuidanceActionEffect(GenericShip host)
        {
            GenericAction newAction = new WeaponsGuildanceDiceModification()
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
    public class WeaponsGuildanceDiceModification : GenericAction
    {

        public WeaponsGuildanceDiceModification()
        {
            Name = DiceModificationName = "Weapons Guidance";

            TokensSpend.Add(typeof(FocusToken));
        }

        public override void ActionEffect(System.Action callBack)
        {
            //This is done in cases where other abilities can be activated upon using a token.
            //e.g. See Pilot Garven Dreis for synergy.
            if (Combat.CurrentDiceRoll.Blanks <= 0)
            {
                Messages.ShowInfoToHuman("Focus token is spent, but there are no blanks.");
            }

            Combat.CurrentDiceRoll.ChangeOne(DieSide.Blank, DieSide.Success);
            Combat.CurrentDiceRoll.OrganizeDicePositions();

            HostShip.Tokens.SpendToken(
                typeof(FocusToken),
                callBack
            );
        }

        public override bool IsDiceModificationAvailable()
        {
            if (Combat.AttackStep == CombatStep.Attack
                && HostShip.Tokens.HasToken(typeof(FocusToken)))
            {
                return true;
            }

            return false;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;
            DiceRoll diceValues = Combat.CurrentDiceRoll;
            if (diceValues.Blanks > 0)
            {
                if (diceValues.Focuses == 0 && diceValues.Blanks == 1)
                {
                    result = 81;
                }
                else
                {
                    result = 40;
                }
            }

            return result;
        }

    }
}