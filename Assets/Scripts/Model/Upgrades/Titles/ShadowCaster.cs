using Arcs;
using RuleSets;
using Ship;
using Ship.LancerClassPursuitCraft;
using Upgrade;

namespace UpgradesList
{
    public class ShadowCaster : GenericUpgradeSlotUpgrade, ISecondEditionUpgrade
    {
        public ShadowCaster() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "Shadow Caster";
            Cost = 3;

            UpgradeAbilities.Add(new Abilities.ShadowCasterAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 6;

            SEImageNumber = 153;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return (ship is LancerClassPursuitCraft);
        }
    }
}


namespace Abilities
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

            if (RuleSet.Instance is FirstEdition)
            {
                if (!Combat.ShotInfo.InArcByType(ArcTypes.Mobile) || Combat.ShotInfo.Range > 2) return;
            }
            else if (RuleSet.Instance is RuleSets.SecondEdition)
            {
                if (!(Combat.ShotInfo.InArcByType(ArcTypes.Mobile) && Combat.ShotInfo.InArcByType(ArcTypes.Primary))) return;
            }

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