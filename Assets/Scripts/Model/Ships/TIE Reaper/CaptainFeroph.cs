using Abilities;
using RuleSets;
using System.Collections;
using System.Collections.Generic;
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

                ImageUrl = "https://i.imgur.com/r6DoYoV.png";
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

}


