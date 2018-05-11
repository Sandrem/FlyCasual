using Abilities;
using ActionsList;
using RuleSets;
using Ship;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using UnityEngine;

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
            HostShip.AfterGenerateAvailableActionEffectsList += CheckAbility;
        }
                
        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList -= CheckAbility;
        }

        private void CheckAbility(Ship.GenericShip ship)
        {
            if (Combat.AttackStep == CombatStep.Defence && Combat.Attacker.Tokens.HasToken<JamToken>())
            {
                var action = new CaptainFerophAction(HostShip);                
                ship.AddAvailableActionEffect(action);
            }
        }

        private class CaptainFerophAction : GenericReinforceAction
        {
            public CaptainFerophAction(GenericShip hostShip)
            {
                Name = EffectName = "Captain Feroph's ability";
                Host = hostShip;
                ImageUrl = Host.ImageUrl;
                Host.OnTryConfirmDiceResults += CheckMustUseReinforce;
            }
                    
            public override bool IsActionAvailable()
            {
                bool result = true;
                if (Host.IsAlreadyExecutedAction(typeof(CaptainFerophAction)))
                {
                    result = false;
                };
                return result;
            }

            public override int GetActionEffectPriority()
            {
                int result = 0;

                if (Combat.AttackStep == CombatStep.Defence)
                {
                    if (Combat.DiceRollAttack.Successes > Combat.DiceRollDefence.Successes)
                    {
                        if (Combat.DiceRollDefence.Focuses > 0) result = 80;
                    }
                }

                return result;
            }
        }
    }

}


