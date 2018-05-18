﻿using Ship;
using Ship.LancerClassPursuitCraft;
using Upgrade;
using BoardTools;
using Arcs;

namespace UpgradesList
{
    public class ShadowCaster : GenericUpgradeSlotUpgrade
    {
        public ShadowCaster() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "Shadow Caster";
            Cost = 3;

            UpgradeAbilities.Add(new Abilities.ShadowCasterAbility());
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
            Phases.OnRoundEnd += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackHitAsAttacker -= CheckShadowCasterAbility;
            Phases.OnRoundEnd -= ClearIsAbilityUsedFlag;
        }

        private void CheckShadowCasterAbility()
        {
            if (IsAbilityUsed) return;

            if (!Combat.ShotInfo.InArcByType(ArcTypes.Mobile) || Combat.ShotInfo.Range > 2) return;

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