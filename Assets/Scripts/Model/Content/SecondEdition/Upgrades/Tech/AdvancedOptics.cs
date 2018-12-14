using ActionsList;
using Ship;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class AdvancedOptics : GenericUpgrade
    {
        public AdvancedOptics() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Advanced Optics",
                UpgradeType.Tech,
                cost: 4,
                abilityType: typeof(Abilities.SecondEdition.AdvancedOpticsAbility)//,
                                                                               //seImageNumber: 69
            );

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/e77e204e6b7164f6a1d945b20a0c4359.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AdvancedOpticsAbility : GenericAbility
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
            GenericAction newAction = new AdvancedOpticsDiceModification()
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
    public class AdvancedOpticsDiceModification : GenericAction
    {

        public AdvancedOpticsDiceModification()
        {
            Name = DiceModificationName = "Advanced Optics";

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