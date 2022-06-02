using Tokens;
using Upgrade;
using System.Collections.Generic;
using Content;

namespace Ship
{
    namespace SecondEdition.TIEDDefender
    {
        public class RexlerBrath : TIEDDefender
        {
            public RexlerBrath() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Rexler Brath",
                    "Onyx Leader",
                    Faction.Imperial,
                    5,
                    7,
                    13,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.RexlerBrathAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Missile,
                        UpgradeType.Missile
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
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
                    Name = HostShip.PilotInfo.PilotName + " exposes facedown card",
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