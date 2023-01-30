using Arcs;
using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class ShadowCaster : GenericUpgrade
    {
        public ShadowCaster() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Shadow Caster",
                UpgradeType.Title,
                cost: 0,
                isLimited: true,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.LancerClassPursuitCraft.LancerClassPursuitCraft)),
                abilityType: typeof(Abilities.SecondEdition.ShadowCasterAbility),
                seImageNumber: 153
            );
        }        
    }
}

namespace Abilities.SecondEdition
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

        private void CheckShadowCasterAbility()
        {
            if (IsAbilityUsed) return;

            if (!(Combat.ShotInfo.InArcByType(ArcType.SingleTurret) && Combat.ShotInfo.InArcByType(ArcType.Front))) return;

            RegisterAbilityTrigger(TriggerTypes.OnAttackHit, AssignTractorBeamToken);
        }

        private void AssignTractorBeamToken(object sender, System.EventArgs e)
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