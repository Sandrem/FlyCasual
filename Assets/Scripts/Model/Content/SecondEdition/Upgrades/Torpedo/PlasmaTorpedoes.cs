using ActionsList;
using Ship;
using System;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class PlasmaTorpedoes : GenericSpecialWeapon
    {
        public PlasmaTorpedoes() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Plasma Torpedoes",
                UpgradeType.Torpedo,
                cost: 9,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 3,
                    minRange: 2,
                    maxRange: 3,
                    charges: 2,
                    requiresToken: typeof(BlueTargetLockToken)
                ),
                abilityType: typeof(Abilities.SecondEdition.PlasmaTorpedoesAbility)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/6f/83/6f83abcd-9460-4208-a439-f6a81597f5f0/swz40_card-plasma-torpedoes.png";
        }
    }
}

namespace Abilities
{
    namespace SecondEdition
    {
        public class PlasmaTorpedoesAbility : GenericAbility
        {
            public override void ActivateAbility()
            {
                HostShip.OnShotHitAsAttacker += PlasmaTorpedoesRemoveShield;
                HostShip.OnDefenceStartAsAttacker += CancelCritsFirstEffect;
            }

            public override void DeactivateAbility()
            {
                HostShip.OnShotHitAsAttacker -= PlasmaTorpedoesRemoveShield;
                HostShip.OnDefenceStartAsAttacker -= CancelCritsFirstEffect;
            }

            private void PlasmaTorpedoesRemoveShield()
            {
                if (Combat.ChosenWeapon.GetType() == HostUpgrade.GetType() && Combat.Defender.State.ShieldsCurrent != 0)
                {
                    RegisterAbilityTrigger(TriggerTypes.OnShotHit, ShieldRemove);
                }
            }

            private void ShieldRemove(object sender, EventArgs e)
            {
                Messages.ShowInfoToHuman($"{Combat.Defender.PilotInfo.PilotName} had a Shield removed by Plasma Torpedoes");
                Combat.Defender.LoseShield();
                Triggers.FinishTrigger();
            }
            private void CancelCritsFirstEffect()
            {
                if (Combat.ChosenWeapon.GetType() == HostUpgrade.GetType())
                {
                    Combat.DiceRollAttack.CancelCritsFirst = true;
                }
            }
        }
    }
}