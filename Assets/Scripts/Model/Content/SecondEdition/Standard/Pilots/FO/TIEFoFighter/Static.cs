using ActionsList;
using Content;
using Ship;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEFoFighter
    {
        public class Static : TIEFoFighter
        {
            public Static() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Static\"",
                    "Omega Ace",
                    Faction.FirstOrder,
                    4,
                    3,
                    8,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.StaticAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Tech,
                        UpgradeType.Cannon,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/75/01/7501b0b3-6350-4f5a-af84-0c988a5493ba/swz26_a1_static.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class StaticAbility : GenericAbility
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
            ship.AddAvailableDiceModificationOwn(new StaticDiceModification());
        }
    }
}

namespace ActionsList
{
    public class StaticDiceModification : GenericAction
    {
        public override string Name => HostShip.PilotInfo.PilotName;
        public override string DiceModificationName => HostShip.PilotInfo.PilotName;
        public override string ImageUrl => HostShip.ImageUrl;

        private System.Action ActionCallback;

        public StaticDiceModification()
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
            this.ActionCallback();
        }

    }
}
