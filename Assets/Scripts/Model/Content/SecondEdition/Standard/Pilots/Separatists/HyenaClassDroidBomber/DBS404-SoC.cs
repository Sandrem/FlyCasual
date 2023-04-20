using System.Collections.Generic;
using Upgrade;
using System;
using Content;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.HyenaClassDroidBomber
{
    public class DBS404SoC : HyenaClassDroidBomber
    {
        public DBS404SoC()
        {
            PilotInfo = new PilotCardInfo25
            (
                "DBS-404",
                "Siege of Coruscant",
                Faction.Separatists,
                4,
                3,
                0,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.DBS404SoCAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Torpedo,
                    UpgradeType.Modification,
                    UpgradeType.Configuration
                },
                tags: new List<Tags>
                {
                    Tags.Droid
                },
                isStandardLayout: true
            );

            MustHaveUpgrades.Add(typeof(AdvProtonTorpedoes));
            MustHaveUpgrades.Add(typeof(ContingencyProtocol));
            MustHaveUpgrades.Add(typeof(StrutLockOverride));

            ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/6/68/Dbs404-siegeofcoruscant.png";

            PilotNameCanonical = "dbs404-siegeofcoruscant";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DBS404SoCAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckBonusDice;
            HostShip.OnAttackHitAsAttacker += CheckSelfDamage;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= CheckBonusDice;
            HostShip.OnAttackHitAsAttacker -= CheckSelfDamage;
        }

        private void CheckBonusDice(ref int count)
        {
            if (Combat.ShotInfo.Range == 1)
            {
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": Attacker rolls +1 attack die");
                count++;
            }
        }

        private void CheckSelfDamage()
        {
            if (Combat.ShotInfo.Range == 1)
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