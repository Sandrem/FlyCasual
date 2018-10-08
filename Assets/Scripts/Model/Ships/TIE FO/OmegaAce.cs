﻿using Ship;
using Abilities;
using ActionsList;
using Tokens;
using System.Collections.Generic;
using System.Linq;

namespace Ship
{
    namespace TIEFO
    {
        public class OmegaAce : TIEFO
        {
            public OmegaAce() : base()
            {
                PilotName = "\"Omega Ace\"";
                PilotSkill = 7;
                Cost = 20;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new OmegaAceAbility());
            }
        }
    }
}

namespace Abilities
{
    public class OmegaAceAbility : GenericAbility
{
    public override void ActivateAbility()
    {
        HostShip.OnGenerateDiceModifications += AddOmegaAceAbility;
    }

    public override void DeactivateAbility()
    {
        HostShip.OnGenerateDiceModifications -= AddOmegaAceAbility;
    }

    private void AddOmegaAceAbility(GenericShip ship)
    {
        ship.AddAvailableDiceModification(new OmegaAceAbilityAction());
    }
}
}

namespace ActionsList
{
    public class OmegaAceAbilityAction : GenericAction
    {
        private System.Action actionCallback;
        public OmegaAceAbilityAction()
        {
            Name = DiceModificationName = "\"Omega Ace\" ability";

            TokensSpend.Add(typeof(BlueTargetLockToken));
            TokensSpend.Add(typeof(FocusToken));
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;
            result = 100;
            return result;
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;
            if (Combat.AttackStep == CombatStep.Attack &&
                Actions.HasTargetLockOn(Combat.Attacker, Combat.Defender) &&
                Combat.Attacker.Tokens.HasToken(typeof(FocusToken)))
            {
                result = true;
            }
            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            if (Actions.HasTargetLockOn(Combat.Attacker, Combat.Defender))
            {
                this.actionCallback = callBack;
                payTargetLock();                
            }
        }
        
        private void payTargetLock()
        {
            List<char> targetLockLetters = Actions.GetTargetLocksLetterPairs(Combat.Attacker, Combat.Defender);
            Combat.Attacker.Tokens.SpendToken(typeof(BlueTargetLockToken), payFocus, targetLockLetters.First());            
        }

        private void payFocus()
        {
            Combat.Attacker.Tokens.SpendToken(typeof(FocusToken), execute);
        }

        private void execute()
        {
            Combat.CurrentDiceRoll.ChangeAll(DieSide.Blank, DieSide.Crit);
            Combat.CurrentDiceRoll.ChangeAll(DieSide.Focus, DieSide.Crit);
            Combat.CurrentDiceRoll.ChangeAll(DieSide.Success, DieSide.Crit);
            Combat.CurrentDiceRoll.OrganizeDicePositions();
            this.actionCallback();
        }

    }


}


