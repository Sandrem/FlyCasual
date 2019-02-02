using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEDDefender
    {
        public class RexlerBrath : TIEDDefender
        {
            public RexlerBrath() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Rexler Brath",
                    5,
                    82,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.RexlerBrathAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 122
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class RexlerBrathAbility : GenericAbility
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
            if (HostShip.Tokens.HasToken(typeof(EvadeToken)) && Combat.Defender.Damage.HasFacedownCards)
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