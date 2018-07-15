using Upgrade;
using Ship;
using ActionsList;
using System;
using Abilities;

namespace UpgradesList
{
    public class WeaponsGuildance : GenericUpgrade
    {
        public WeaponsGuildance() : base()
        {
            Types.Add(UpgradeType.Tech);
            Name = "Weapons Guidance";
            Cost = 2;

            UpgradeAbilities.Add(new WeaponsGuildanceAbility());
        }
    }
}

namespace Abilities
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
                Host = HostShip
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

            TokensSpend.Add(typeof(Tokens.FocusToken));
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

            Host.Tokens.SpendToken(
                typeof(Tokens.FocusToken),
                callBack
            );
        }

        public override bool IsDiceModificationAvailable()
        {
            if (Combat.AttackStep == CombatStep.Attack 
                && Host.Tokens.HasToken(typeof(Tokens.FocusToken)))
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