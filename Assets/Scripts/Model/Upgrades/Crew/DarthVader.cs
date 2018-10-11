using Upgrade;
using Ship;
using SubPhases;
using Abilities;
using System;
using UnityEngine;
using RuleSets;
using BoardTools;
using System.Linq;

namespace UpgradesList
{
	public class DarthVader : GenericUpgrade, ISecondEditionUpgrade
	{
		public DarthVader() : base()
		{
			Types.Add(UpgradeType.Crew);
			Name = "Darth Vader";
			Cost = 3;
      
            isUnique = true;

            Avatar = new AvatarInfo(Faction.Imperial, new Vector2(53, 1));

			UpgradeAbilities.Add(new DarthVaderCrewAbility());
		}

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 14;            

            UpgradeAbilities.RemoveAll(a => a is DarthVaderCrewAbility);
            UpgradeAbilities.Add(new Abilities.SecondEdition.DarthVaderCrewAbility());

            SEImageNumber = 112;
        }

        public override bool IsAllowedForShip(GenericShip ship)
		{
			return ship.faction == Faction.Imperial;
		}
	}
}

namespace Abilities
{
	public class DarthVaderCrewAbility: GenericAbility
	{

		public override void ActivateAbility()
		{
			HostShip.OnAttackFinishAsAttacker += RegisterVaderAbility;
		}


		public override void DeactivateAbility()
		{
			HostShip.OnAttackFinishAsAttacker -= RegisterVaderAbility;
		}


		private void RegisterVaderAbility(GenericShip ship)
		{
			RegisterAbilityTrigger (TriggerTypes.OnAttackFinish, AskToUseVaderAbility);
		}

		//Confirmation for Darth Vader Ability
		private void AskToUseVaderAbility(object sender, System.EventArgs e)
		{
			AskToUseAbility(NeverUseByDefault, resolveDarthVaderAbility);
		}
			
		//This method assigns one crit to defender, and two damages to HostShip
		private void resolveDarthVaderAbility(object sender, System.EventArgs e)
		{
			
			Combat.Defender.AssignedDamageDiceroll.AddDice(DieSide.Crit);

			Triggers.RegisterTrigger ( new Trigger() {
					Name = "Damage from Darth Vader Ability",
					TriggerType = TriggerTypes.OnDamageIsDealt,
					TriggerOwner = Combat.Defender.Owner.PlayerNo,
					EventHandler = Combat.Defender.SufferDamage,
					EventArgs = new DamageSourceEventArgs()
					{
						Source = "Dath Vader",
						DamageType = DamageTypes.CardAbility
					}

				});
			//Delegates flow control to Hostship damage resolution
			Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, AssignDamageToHost);
		}

		//Assigns n damages to HostShip
		private void AssignDamageToHost()
        {
            for (int i = 0; i < 2; i++)
            {
                HostShip.AssignedDamageDiceroll.AddDice(DieSide.Success);

                Triggers.RegisterTrigger(
                    new Trigger()
                    {
                        Name = "Damage from Darth Vader Ability",
                        TriggerType = TriggerTypes.OnDamageIsDealt,
                        TriggerOwner = Combat.Attacker.Owner.PlayerNo,
                        EventHandler = Combat.Attacker.SufferDamage,
                        EventArgs = new DamageSourceEventArgs()
                        {
                            Source = "Dath Vader",
                            DamageType = DamageTypes.CardAbility
                        },
                        Skippable = true
                    }
                );
            }

			Triggers.ResolveTriggers (TriggerTypes.OnDamageIsDealt, DecisionSubPhase.ConfirmDecision);
		}
	}

    namespace SecondEdition
    {
        //At the start of the Engagement Phase, you may choose 1 ship in your firing arc at range 0-2 and spend 1 force. 
        //If you do, that ship suffers 1 hit damage unless it chooses to remove 1 green token.
        public class DarthVaderCrewAbility : GenericAbility
        {
            public override void ActivateAbility()
            {
                Phases.Events.OnCombatPhaseStart_Triggers += RegisterVaderAbility;
                HostShip.MaxForce += 1;
            }

            public override void DeactivateAbility()
            {
                Phases.Events.OnCombatPhaseStart_Triggers -= RegisterVaderAbility;
                HostShip.MaxForce -= 1;
            }

            private void RegisterVaderAbility()
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, Ability);
            }

            private void Ability(object sender, EventArgs e)
            { 
                if (HostShip.Force > 0)
                {
                    SelectTargetForAbility(
                        SelectAbilityTarget,
                        FilterAbilityTarget,
                        GetAiAbilityPriority,
                        HostShip.Owner.PlayerNo,
                        HostUpgrade.Name,
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
                    new ShotInfo(HostShip, ship, HostShip.PrimaryWeapon).InArc &&
                    FilterTargetsByRange(ship, 0, 2);
            }

            private int GetAiAbilityPriority(GenericShip ship)
            {
                return 100;
            }

            private void SelectAbilityTarget()
            {
                HostShip.Force--;

                var tokenTypes = TargetShip.Tokens
                    .GetAllTokens().Where(t => t.TokenColor == Tokens.TokenColors.Green)
                    .Select(t => new { Name = t.Name, Type = t.GetType() })
                    .Distinct().ToList();

                if (tokenTypes.Count > 0)
                {
                    var decisionSubPhase = Phases.StartTemporarySubPhaseNew<DecisionSubPhase>(
                        Name,
                        SelectShipSubPhase.FinishSelection
                    );

                    decisionSubPhase.InfoText = "Remove token to prevent 1 damage from Darth Vader?";

                    decisionSubPhase.AddDecision("No", delegate { AssignDamage(); });

                    tokenTypes.ForEach(token =>
                    {
                        decisionSubPhase.AddDecision(token.Name, delegate { RemoveGreenToken(token.Type); });
                        decisionSubPhase.AddTooltip(token.Name, HostUpgrade.ImageUrl);
                    });

                    decisionSubPhase.DefaultDecisionName = decisionSubPhase.GetDecisions().First(d => d.Name != "No").Name;
                    decisionSubPhase.RequiredPlayer = TargetShip.Owner.PlayerNo;

                    decisionSubPhase.Start();
                }
                else
                {
                    SelectShipSubPhase.FinishSelection();
                    AssignDamage();
                }
            }

            private void RemoveGreenToken(Type tokenType)
            {
                TargetShip.Tokens.RemoveToken(tokenType, DecisionSubPhase.ConfirmDecision);
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
                        Source = "Dath Vader",
                        DamageType = DamageTypes.CardAbility
                    }
                });

                Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, DecisionSubPhase.ConfirmDecision);
            }
        }
    }
}
