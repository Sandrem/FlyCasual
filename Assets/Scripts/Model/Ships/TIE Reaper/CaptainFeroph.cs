using Abilities;
using ActionsList;
using RuleSets;
using Ship;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tokens;

namespace Ship
{
    namespace TIEReaper
    {
        public class CaptainFeroph : TIEReaper, ISecondEditionPilot
        {
            public CaptainFeroph() : base()
            {
                PilotName = "Captain Feroph";
                PilotSkill = 4;
                Cost = 24;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new CaptainFerophAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 3;
                Cost = 58;

                PilotAbilities.RemoveAll(a => a is CaptainFerophAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.CaptainFerophAbilitySE());
            }
        }                
    }
}

namespace Abilities
{
    public class CaptainFerophAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnImmediatelyAfterRolling += CheckAbility;
        }
                
        public override void DeactivateAbility()
        {
            HostShip.OnImmediatelyAfterRolling -= CheckAbility;
        }

        private void CheckAbility(DiceRoll diceroll)
        {
            if (diceroll.Type == DiceKind.Defence && diceroll.CheckType == DiceRollCheckType.Combat && Combat.Attacker.Tokens.HasToken<JamToken>())
            {
                Messages.ShowInfo("Captain Feroph: Evade result is added");
                diceroll.AddDice(DieSide.Success).ShowWithoutRoll();
                diceroll.OrganizeDicePositions();
            }
        }
    }

    namespace SecondEdition
    {
        public class CaptainFerophAbilitySE : GenericAbility
        {
            public override void ActivateAbility()
            {
                HostShip.AfterGenerateAvailableActionEffectsList += TryAddCaptainFerophDiceModification;
            }

            public override void DeactivateAbility()
            {
                HostShip.AfterGenerateAvailableActionEffectsList -= TryAddCaptainFerophDiceModification;
            }

            private void TryAddCaptainFerophDiceModification(GenericShip host)
            {
                GenericAction newAction = new CaptainFerophDiceModification()
                {
                    ImageUrl = HostShip.ImageUrl,
                    Host = host
                };
                host.AddAvailableActionEffect(newAction);
            }
        }

    }

}

namespace ActionsList
{
    public class CaptainFerophDiceModification : GenericAction
    {
        public CaptainFerophDiceModification()
        {
            Name = EffectName = "Captain Feroph";
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;

            if (Combat.CurrentDiceRoll.Blanks > 0 || Combat.CurrentDiceRoll.Focuses > 0) result = 100;

            return result;
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Defence)
            {
                if (!Combat.Attacker.Tokens.GetAllTokens().Any(n => n.TokenColor == TokenColors.Green))
                {
                    result = true;
                }
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            if (Combat.CurrentDiceRoll.Blanks > 0)
            {
                Combat.CurrentDiceRoll.ChangeOne(DieSide.Blank, DieSide.Success);
            }
            else if (Combat.CurrentDiceRoll.Focuses > 0)
            {
                Combat.CurrentDiceRoll.ChangeOne(DieSide.Focus, DieSide.Success);
            }

            callBack();
        }
    }

}


