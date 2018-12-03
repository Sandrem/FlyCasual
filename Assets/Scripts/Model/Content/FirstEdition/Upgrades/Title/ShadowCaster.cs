using Arcs;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class ShadowCaster : GenericUpgrade
    {
        public ShadowCaster() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Shadow Caster",
                UpgradeType.Title,
                cost: 3,
                isLimited: true,
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.LancerClassPursuitCraft.LancerClassPursuitCraft)),
                abilityType: typeof(Abilities.FirstEdition.ShadowCasterAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class ShadowCasterAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackHitAsAttacker += CheckShadowCasterAbility;
            Phases.Events.OnRoundEnd += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackHitAsAttacker -= CheckShadowCasterAbility;
            Phases.Events.OnRoundEnd -= ClearIsAbilityUsedFlag;
        }

        protected virtual void CheckShadowCasterAbility()
        {
            if (IsAbilityUsed) return;

            if (!Combat.ShotInfo.InArcByType(ArcType.SingleTurret) || Combat.ShotInfo.Range > 2) return;

            RegisterAbilityTrigger(TriggerTypes.OnAttackHit, AssignTractorBeamToken);
        }

        protected void AssignTractorBeamToken(object sender, System.EventArgs e)
        {
            Tokens.TractorBeamToken token = new Tokens.TractorBeamToken(Combat.Defender, Combat.Attacker.Owner);

            Combat.Defender.Tokens.AssignToken(
                token,
                delegate
                {
                    IsAbilityUsed = true;
                    Triggers.FinishTrigger();
                }
            );
        }
    }
}