using Upgrade;
using Ship;
using ActionsList;

namespace UpgradesList
{
    public class WeaponsGuildance : GenericUpgrade
    {
        public WeaponsGuildance() : base()
        {
            Type = UpgradeType.Tech;
            Name = "Weapons Guidance";
            Cost = 2;            
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGenerateAvailableActionEffectsList += WeaponsGuidanceActionEffect;
        }

        private void WeaponsGuidanceActionEffect(GenericShip host)
        {
            GenericAction newAction = new WeaponsGuildanceDiceModification()
            {
                ImageUrl = ImageUrl,
                Host = host
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
        }

        public override void ActionEffect(System.Action callBack)
        {
            //This is done in cases where other abilities can be activated upon using a token.
            //e.g. See Pilot Garven Dreis for synergy.
            if (Combat.CurrentDiceRoll.Blanks <= 0)
            {
                Messages.ShowInfoToHuman("Focus token is spent, but there are no blanks.");
            }

            Host.RemoveToken(typeof(Tokens.FocusToken));
            Combat.CurrentDiceRoll.ChangeOne(DieSide.Blank, DieSide.Success);
            Combat.CurrentDiceRoll.OrganizeDicePositions();

            callBack();
        }

        public override bool IsActionEffectAvailable()
        {
            if (Combat.AttackStep == CombatStep.Attack 
                && Host.HasToken(typeof(Tokens.FocusToken)))
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
                else if (diceValues.Focuses > 1)
                {
                    result = 45;
                }
                else if (diceValues.Blanks == 1 && diceValues.Focuses == 1)
                {
                    result = 40;
                }
            }

            return result;
        }

    }
}