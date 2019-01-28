using Ship;
using SubPhases;
using System;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class C4LOM: GenericUpgrade
    {
        public C4LOM() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "4-LOM",
                type: UpgradeType.Crew,
                cost: 2,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Scum),
                abilityType: typeof(Abilities.SecondEdition.C4LOMAbility),
                seImageNumber: 128
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    //While you perform an attack, after rolling attack dice, you may name a type of green token. 
    //If you do, gain 2 ion tokens and, during this attack, the defender cannot spend tokens of the named type.
    public class C4LOMAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnImmediatelyAfterRolling += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnImmediatelyAfterRolling -= RegisterAbility;
        }

        private void RegisterAbility(DiceRoll diceroll)
        {
            if (Combat.AttackStep == CombatStep.Attack && Combat.Attacker == HostShip)
            {
                RegisterAbilityTrigger(TriggerTypes.OnImmediatelyAfterRolling, Ability);
            }
        }

        private void Ability(object sender, EventArgs e)
        {
            var phase = Phases.StartTemporarySubPhaseNew<C4LOMDecisionSubphase>(
                "4-LOM: Name a green token the defender can not spend",
                Triggers.FinishTrigger);
            phase.TargetShip = Combat.Defender;
            phase.HostShip = HostShip;
            phase.Start();            
        }

        protected class C4LOMDecisionSubphase : DecisionSubPhase
        {
            public GenericShip HostShip;
            public GenericShip TargetShip;
            private string NamedToken;
            private Type[] NamedTokenTypes;

            public override void PrepareDecision(Action callBack)
            {
                InfoText = "4-LOM: Name a green token the defender can not spend";

                DecisionViewType = DecisionViewTypes.TextButtons;

                AddDecision("Calculate", delegate { RestrictTokens("Calculate", typeof(Tokens.CalculateToken)); });
                AddDecision("Evade", delegate { RestrictTokens("Evade", typeof(Tokens.EvadeToken)); });
                AddDecision("Focus", delegate { RestrictTokens("Focus", typeof(Tokens.FocusToken)); });
                AddDecision("Reinforce", delegate { RestrictTokens("Reinforce", typeof(Tokens.ReinforceAftToken), typeof(Tokens.ReinforceAftToken)); });

                DefaultDecisionName = GetDecisions().First().Name;
                ShowSkipButton = true;
                callBack();
            }

            private void RestrictTokens(string name, params Type[] types)
            {
                NamedToken = name;
                NamedTokenTypes = types;
                TargetShip.OnTryAddAvailableDiceModification += Use4LOMRestriction;
                HostShip.Tokens.AssignTokens(() => new Tokens.IonToken(HostShip), 2, ConfirmDecision);
            }

            private void Use4LOMRestriction(GenericShip ship, ActionsList.GenericAction action, ref bool canBeUsed)
            {
                if (action.TokensSpend.Any(t => NamedTokenTypes.Contains(t)))
                {
                    Messages.ShowErrorToHuman("4-LOM: Cannot spend " + NamedToken);
                    canBeUsed = false;
                }
            }
        }
    }
}
