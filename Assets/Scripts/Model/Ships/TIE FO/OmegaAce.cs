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
        public OmegaAceAbilityAction()
        {
            Name = EffectName = "\"Omega Ace\" ability";
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
                Combat.Attacker.HasToken(typeof(FocusToken)))
            {
                result = true;
            }
            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            if (Actions.HasTargetLockOn(Combat.Attacker, Combat.Defender))
            {
                payCost(callBack);
                Combat.CurrentDiceRoll.ChangeAll(DieSide.Blank, DieSide.Crit);
                Combat.CurrentDiceRoll.ChangeAll(DieSide.Focus, DieSide.Crit);
                Combat.CurrentDiceRoll.ChangeAll(DieSide.Success, DieSide.Crit);
                Combat.CurrentDiceRoll.OrganizeDicePositions();                
                callBack();
            }
        }

        private void payCost(System.Action callBack)
        {
            char targetLockLetter = Actions.GetTargetLocksLetterPair(Combat.Attacker, Combat.Defender);
            Combat.Attacker.SpendToken(typeof(BlueTargetLockToken),callBack,targetLockLetter);
            Combat.Attacker.SpendToken(typeof(FocusToken),callBack);
        }
    }

}


