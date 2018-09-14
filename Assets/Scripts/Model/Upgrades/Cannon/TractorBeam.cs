﻿using Abilities;
using Upgrade;
using UpgradesList;
using Tokens;
using RuleSets;

namespace UpgradesList
{

    public class TractorBeam : GenericSecondaryWeapon, ISecondEditionUpgrade
    {
        public TractorBeam() : base()
        {
            Types.Add(UpgradeType.Cannon);

            Name = "Tractor Beam";
            Cost = 1;

            MinRange = 1;
            MaxRange = 3;
            AttackValue = 3;

            UpgradeAbilities.Add(new TractorBeamAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 3;
            UpgradeAbilities.RemoveAll(a => a is TractorBeamAbility);
            UpgradeAbilities.Add(new TractorBeamAbilitySE());

            SEImageNumber = 30;
        }
    }
}

namespace Abilities
{
    public class TractorBeamAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotHitAsAttacker += RegisterTractorBeamEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotHitAsAttacker -= RegisterTractorBeamEffect;
        }

        private void RegisterTractorBeamEffect()
        {
            if (Combat.ChosenWeapon is TractorBeam)
            {
                RegisterAbilityTrigger(TriggerTypes.OnShotHit, TractorBeamEffect);
            }
        }

        protected virtual void TractorBeamEffect(object sender, System.EventArgs e)
        {
            Combat.DiceRollAttack.CancelAllResults();
            Combat.DiceRollAttack.RemoveAllFailures();

            TractorBeamToken token = new TractorBeamToken(Combat.Defender, Combat.Attacker.Owner);
            Combat.Defender.Tokens.AssignToken(token, Triggers.FinishTrigger);
        }
    }

    public class TractorBeamAbilitySE : TractorBeamAbility
    {
        protected override void TractorBeamEffect(object sender, System.EventArgs e)
        {
            int tractorBeamTokens =  Combat.DiceRollAttack.Successes;
            Combat.DiceRollAttack.CancelAllResults();
            Combat.DiceRollAttack.RemoveAllFailures();

            Combat.Defender.Tokens.AssignTokens(() => new TractorBeamToken(Combat.Defender, Combat.Attacker.Owner), tractorBeamTokens, Triggers.FinishTrigger);
        }
    }
}