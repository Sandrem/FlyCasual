using Ship;
using Upgrade;
using UnityEngine;
using SubPhases;

namespace UpgradesList.FirstEdition
{
    public class DarthVader : GenericUpgrade
    {
        public DarthVader() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Darth Vader",
                UpgradeType.Crew,
                cost: 3,
                isLimited: true,
                restrictionFaction: Faction.Imperial,
                abilityType: typeof(Abilities.FirstEdition.DarthVaderCrewAbility)
            );

            Avatar = new AvatarInfo(Faction.Imperial, new Vector2(53, 1));
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class DarthVaderCrewAbility : GenericAbility
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
            RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, AskToUseVaderAbility);
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

            Triggers.RegisterTrigger(new Trigger()
            {
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

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, DecisionSubPhase.ConfirmDecision);
        }
    }
}