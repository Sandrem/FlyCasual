using Ship;
using Abilities;
using ActionsList;
using Tokens;


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
        HostShip.AfterGenerateAvailableActionEffectsList += AddOmegaAceAbility;
    }

    public override void DeactivateAbility()
    {
        HostShip.AfterGenerateAvailableActionEffectsList -= AddOmegaAceAbility;
    }

    private void AddOmegaAceAbility(GenericShip ship)
    {
        ship.AddAvailableActionEffect(new OmegaAceAbilityAction());
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
            Name = EffectName = "\"Omega Ace\" ability";

            TokensSpend.Add(typeof(BlueTargetLockToken));
            TokensSpend.Add(typeof(FocusToken));
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;
            result = 100;
            return result;
        }

        public override bool IsActionEffectAvailable()
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
            char targetLockLetter = Actions.GetTargetLocksLetterPair(Combat.Attacker, Combat.Defender);
            Combat.Attacker.Tokens.SpendToken(typeof(BlueTargetLockToken), payFocus, targetLockLetter);            
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


