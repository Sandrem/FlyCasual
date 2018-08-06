using Abilities.SecondEdition;
using RuleSets;
using Tokens;

namespace Ship
{
    namespace TIEDefender
    {
        public class RexlerBrath : TIEDefender, ISecondEditionPilot
        {
            public RexlerBrath() : base()
            {
                PilotName = "Rexler Brath";
                PilotSkill = 5;
                Cost = 84;

                IsUnique = true;

                PilotRuleType = typeof(SecondEdition);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new RexlerBrathAbilitySE());
            }

            public void AdaptPilotToSecondEdition()
            {
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class RexlerBrathAbilitySE : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackHitAsAttacker += CheckRexlerBrathTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackHitAsAttacker -= CheckRexlerBrathTrigger;
        }

        private void CheckRexlerBrathTrigger()
        {
            if(HostShip.Tokens.HasToken(typeof(EvadeToken)) && Combat.Defender.Damage.HasFacedownCards)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Rexler Brath exposes facedown card.",
                    TriggerType = TriggerTypes.OnAttackHit,
                    TriggerOwner = Combat.Defender.Owner.PlayerNo,
                    EventHandler = delegate {
                        Combat.Defender.Damage.ExposeRandomFacedownCard(Triggers.FinishTrigger);
                    }
                });
            }
        }
    }
}