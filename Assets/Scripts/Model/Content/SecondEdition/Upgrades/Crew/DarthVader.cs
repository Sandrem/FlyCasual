using Ship;
using Upgrade;
using SubPhases;
using System;
using BoardTools;
using System.Linq;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class DarthVader : GenericUpgrade
    {
        public DarthVader() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Darth Vader",
                UpgradeType.Crew,
                cost: 14,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Imperial),
                addForce: 1,
                abilityType: typeof(Abilities.SecondEdition.DarthVaderCrewAbility),
                seImageNumber: 112
            );

            Avatar = new AvatarInfo(
                Faction.Imperial,
                new Vector2(429, 24)
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    //At the start of the Engagement Phase, you may choose 1 ship in your firing arc at range 0-2 and spend 1 force. 
    //If you do, that ship suffers 1 hit damage unless it chooses to remove 1 green token.
    public class DarthVaderCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterVaderAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterVaderAbility;
        }

        private void RegisterVaderAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, Ability);
        }

        private void Ability(object sender, EventArgs e)
        {
            if (HostShip.State.Force > 0)
            {
                SelectTargetForAbility(
                    SelectAbilityTarget,
                    FilterAbilityTarget,
                    GetAiAbilityPriority,
                    HostShip.Owner.PlayerNo,
                    HostUpgrade.UpgradeInfo.Name,
                    "Choose 1 ship to suffer 1 damage",
                    HostUpgrade);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        protected virtual bool FilterAbilityTarget(GenericShip ship)
        {
            return
                ship != HostShip &&
                new ShotInfo(HostShip, ship, HostShip.PrimaryWeapons).InArc &&
                FilterTargetsByRange(ship, 0, 2);
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            // Select only enemy ships by cost
            return (ship.Owner.PlayerNo != HostShip.Owner.PlayerNo) ? ship.PilotInfo.Cost : -1;
        }

        private void SelectAbilityTarget()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            var tokenTypes = TargetShip.Tokens
                .GetAllTokens().Where(t => t.TokenColor == Tokens.TokenColors.Green)
                .Select(t => new { Name = t.Name, Type = t.GetType() })
                .Distinct().ToList();

            if (tokenTypes.Count > 0)
            {
                var decisionSubPhase = Phases.StartTemporarySubPhaseNew<DecisionSubPhase>(
                    Name,
                    Triggers.FinishTrigger
                );

                decisionSubPhase.DescriptionShort = "Remove token to prevent 1 damage from Darth Vader?";

                decisionSubPhase.AddDecision(
                    "No",
                    delegate {
                        DecisionSubPhase.ConfirmDecisionNoCallback();
                        AssignDamage();
                    }
                );

                tokenTypes.ForEach(token =>
                {
                    decisionSubPhase.AddDecision(token.Name, delegate { RemoveGreenToken(token.Type); });
                    decisionSubPhase.AddTooltip(token.Name, HostUpgrade.ImageUrl);
                });

                decisionSubPhase.DefaultDecisionName = decisionSubPhase.GetDecisions().First(d => d.Name != "No").Name;
                decisionSubPhase.RequiredPlayer = TargetShip.Owner.PlayerNo;

                HostShip.State.SpendForce(1, decisionSubPhase.Start);
            }
            else
            {
                HostShip.State.SpendForce(1, AssignDamage);
            }
        }

        private void RemoveGreenToken(Type tokenType)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            TargetShip.Tokens.RemoveToken(tokenType, Triggers.FinishTrigger);
        }

        private void AssignDamage()
        {
            TargetShip.AssignedDamageDiceroll.AddDice(DieSide.Success);

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Damage from Darth Vader Ability",
                TriggerType = TriggerTypes.OnDamageIsDealt,
                TriggerOwner = TargetShip.Owner.PlayerNo,
                EventHandler = TargetShip.SufferDamage,
                EventArgs = new DamageSourceEventArgs()
                {
                    Source = HostUpgrade,
                    DamageType = DamageTypes.CardAbility
                }
            });

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, Triggers.FinishTrigger);
        }
    }
}