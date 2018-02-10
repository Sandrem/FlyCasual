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
            Type = UpgradeType.Tech;
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
            HostShip.AfterGenerateAvailableActionEffectsList += WeaponsGuidanceActionEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList -= WeaponsGuidanceActionEffect;
        }

        private void WeaponsGuidanceActionEffect(GenericShip host)
        {
            GenericAction newAction = new WeaponsGuildanceDiceModification()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = HostShip
            };
            host.AddAvailableActionEffect(newAction);
        }

    }
}

namespace ActionsList
{
    public class WeaponsGuildanceDiceModification : GenericAction
    {

        public WeaponsGuildanceDiceModification()
        {
            Name = EffectName = "Weapons Guidance";

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

        public override bool IsActionEffectAvailable()
        {
            if (Combat.AttackStep == CombatStep.Attack 
                && Host.Tokens.HasToken(typeof(Tokens.FocusToken)))
            {
                return true;
            }

            return false;
        }

        public override int GetActionEffectPriority()
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