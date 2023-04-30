using System.Collections.Generic;
using Upgrade;
using System;
using Content;

namespace Ship.SecondEdition.HyenaClassDroidBomber
{
    public class DBS404 : HyenaClassDroidBomber
    {
        public DBS404()
        {
            PilotInfo = new PilotCardInfo25
            (
                "DBS-404",
                "Preservation Protocol Not Found",
                Faction.Separatists,
                4,
                4,
                10,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.DBS404Ability),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Torpedo,
                    UpgradeType.Missile,
                    UpgradeType.Device
                },
                tags: new List<Tags>
                {
                    Tags.Droid
                }
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DBS404Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.PrimaryWeapons.ForEach(n => n.WeaponInfo.MinRange = 0);

            HostShip.AfterGotNumberOfAttackDice += CheckBonusDice;
            HostShip.OnAttackHitAsAttacker += CheckSelfDamage;
        }

        public override void DeactivateAbility()
        {
            HostShip.PrimaryWeapons.ForEach(n => n.WeaponInfo.MinRange = 1);

            HostShip.AfterGotNumberOfAttackDice -= CheckBonusDice;
            HostShip.OnAttackHitAsAttacker -= CheckSelfDamage;
        }

        private void CheckBonusDice(ref int count)
        {
            if (Combat.ShotInfo.Range < 2)
            {
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": Attacker rolls +1 attack die");
                count++;
            }
        }

        private void CheckSelfDamage()
        {
            if (Combat.ShotInfo.Range < 2)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackHit, SufferSelfDamage);
            }
        }

        private void SufferSelfDamage(object sender, EventArgs e)
        {
            Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": Suffers critical damage");

            HostShip.Damage.TryResolveDamage(
                0,
                1,
                new DamageSourceEventArgs()
                {
                    Source = HostShip,
                    DamageType = DamageTypes.CardAbility
                },
                Triggers.FinishTrigger
            );
        }
    }
}