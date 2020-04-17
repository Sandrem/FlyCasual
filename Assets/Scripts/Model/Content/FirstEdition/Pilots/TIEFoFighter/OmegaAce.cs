using ActionsList;
using Ship;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIEFoFighter
    {
        public class OmegaAce : TIEFoFighter
        {
            public OmegaAce() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Omega Ace\"",
                    7,
                    20,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.OmegaAceAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
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
            ship.AddAvailableDiceModificationOwn(new OmegaAceDiceModification());
        }
    }
}

namespace ActionsList
{
    public class OmegaAceDiceModification : GenericAction
    {
        public override string Name => HostShip.PilotInfo.PilotName;
        public override string DiceModificationName => HostShip.PilotInfo.PilotName;
        public override string ImageUrl => HostShip.ImageUrl;

        private System.Action ActionCallback;

        public OmegaAceDiceModification()
        {
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
                ActionsHolder.HasTargetLockOn(Combat.Attacker, Combat.Defender) &&
                Combat.Attacker.Tokens.HasToken(typeof(FocusToken)))
            {
                result = true;
            }
            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            if (ActionsHolder.HasTargetLockOn(Combat.Attacker, Combat.Defender))
            {
                this.ActionCallback = callBack;
                PayTargetLock();
            }
        }

        private void PayTargetLock()
        {
            List<char> targetLockLetters = ActionsHolder.GetTargetLocksLetterPairs(Combat.Attacker, Combat.Defender);
            Combat.Attacker.Tokens.SpendToken(typeof(BlueTargetLockToken), PayFocus, targetLockLetters.First());
        }

        private void PayFocus()
        {
            Combat.Attacker.Tokens.SpendToken(typeof(FocusToken), Execute);
        }

        private void Execute()
        {
            Combat.CurrentDiceRoll.ChangeAll(DieSide.Blank, DieSide.Crit);
            Combat.CurrentDiceRoll.ChangeAll(DieSide.Focus, DieSide.Crit);
            Combat.CurrentDiceRoll.ChangeAll(DieSide.Success, DieSide.Crit);
            Combat.CurrentDiceRoll.OrganizeDicePositions();
            this.ActionCallback();
        }

    }
}