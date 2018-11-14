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
                    84,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.RexlerBrathAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);

                SEImageNumber = 122;
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