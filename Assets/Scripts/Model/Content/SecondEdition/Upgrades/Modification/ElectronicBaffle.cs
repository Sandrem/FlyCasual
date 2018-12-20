using Upgrade;
using System.Collections.Generic;
using System;
using Tokens;
using SubPhases;
using Players;
using Editions;

namespace UpgradesList.SecondEdition
{
    public class ElectronicBaffle : GenericUpgrade
    {
        public ElectronicBaffle() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Electronic Baffle",
                UpgradeType.Modification,
                cost: 2,
                abilityType: typeof(Abilities.SecondEdition.ElectronicBaffleAbility),
                seImageNumber: 71
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you fully execute a speed 3-5 maneuver you may spend 1 charge to perform a boost action, even while stressed.
    public class ElectronicBaffleAbility : GenericAbility
    {
        List<GenericToken> RedTokens;
        GenericToken SelectedToken;
        public override void ActivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers -= CheckAbility;
        }

        private void CheckAbility()
        {
            List<GenericToken> AssignedTokens = HostShip.Tokens.GetAllTokens();
            RedTokens = new List<GenericToken>();
            foreach (GenericToken Token in AssignedTokens)
            {
                if (Token.TokenColor == TokenColors.Red)
                    RedTokens.Add(Token);
            }
            if (RedTokens.Count > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnEndPhaseStart, ChooseToken);
            }
        }

        private void ChooseToken(object sender, EventArgs e)
        {
            if (HostShip.Owner.Type == PlayerType.Ai)
            {
                DamageSourceEventArgs electronicBaffleDamage = new DamageSourceEventArgs()
                {
                    Source = "Electronic Battle",
                    DamageType = DamageTypes.CardAbility
                };
                double DamageLevel = (double)(HostShip.State.HullCurrent + HostShip.State.ShieldsCurrent)/(HostShip.State.HullMax + HostShip.State.ShieldsMax);
                if (DamageLevel > .34 && HostShip.Tokens.HasToken(typeof(IonToken)) && HostShip.Tokens.CountTokensByType(typeof(IonToken)) == Edition.Current.NegativeTokensToAffectShip[HostShip.ShipInfo.BaseSize])
                {
                    SelectedToken = HostShip.Tokens.GetToken(typeof(IonToken));
                    HostShip.Damage.TryResolveDamage(1, electronicBaffleDamage, RemoveTokenAI);
                }
                else if (DamageLevel > .75 && HostShip.Tokens.HasToken(typeof(StressToken)))
                {
                    SelectedToken = HostShip.Tokens.GetToken(typeof(StressToken));
                    HostShip.Damage.TryResolveDamage(1, electronicBaffleDamage, RemoveTokenAI);
                }
                else if (DamageLevel > .99)
                {
                    Random rnd = new Random();
                    SelectedToken = RedTokens[rnd.Next(RedTokens.Count)];
                    HostShip.Damage.TryResolveDamage(1, electronicBaffleDamage, RemoveTokenAI);
                }
                else
                {
                    Triggers.FinishTrigger();
                }
            }
            else
            {
                DecisionSubPhase pilotAbilityDecision = (DecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                    Name,
                    typeof(ElectronicBaffleDecisionSubphase),
                    Triggers.FinishTrigger
                );

                pilotAbilityDecision.InfoText = "Electronic Baffle: Choose which token to remove";
                pilotAbilityDecision.RequiredPlayer = HostShip.Owner.PlayerNo;

                foreach (var Token in RedTokens)
                {
                    string name = Token.Name;
                    if (Token.GetType() == typeof(RedTargetLockToken))
                    {
                        RedTargetLockToken targetLockToken = (RedTargetLockToken)Token;
                        name = Token.Name + " " + targetLockToken.Letter; 
                    }
                    pilotAbilityDecision.AddDecision(name, delegate {
                        DamageSourceEventArgs electronicBaffleDamage = new DamageSourceEventArgs()
                        {
                            Source = "Electronic Battle",
                            DamageType = DamageTypes.CardAbility
                        };
                        SelectedToken = Token;

                        HostShip.Damage.TryResolveDamage(1, electronicBaffleDamage, RemoveToken);
                    });
                }

                pilotAbilityDecision.ShowSkipButton = true;
                pilotAbilityDecision.Start();
            }
        }

        private void RemoveToken()
        {
            HostShip.Tokens.RemoveToken(SelectedToken, DecisionSubPhase.ConfirmDecision);
        }

        private void RemoveTokenAI()
        {
            HostShip.Tokens.RemoveToken(SelectedToken, Triggers.FinishTrigger);
        }

        private class ElectronicBaffleDecisionSubphase : DecisionSubPhase { }
    }
}