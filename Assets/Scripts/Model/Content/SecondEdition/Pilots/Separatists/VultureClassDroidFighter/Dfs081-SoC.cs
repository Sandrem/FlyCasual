using Content;
using System;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.VultureClassDroidFighter
{
    public class Dfs081SoC : VultureClassDroidFighter
    {
        public Dfs081SoC()
        {
            PilotInfo = new PilotCardInfo25
            (
                "DFS-081",
                "Siege of Coruscant",
                Faction.Separatists,
                3,
                2,
                0,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.Dfs081SoCAbility),
                charges: 2,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Missile,
                    UpgradeType.Modification,
                    UpgradeType.Configuration
                },
                tags: new List<Tags>
                {
                    Tags.Droid
                },
                isStandardLayout: true
            );

            MustHaveUpgrades.Add(typeof(DiscordMissiles));
            MustHaveUpgrades.Add(typeof(ContingencyProtocol));
            MustHaveUpgrades.Add(typeof(StrutLockOverride));

            ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/1/1e/Dfs081-siegeofcoruscant.png";

            PilotNameCanonical = "dfs081-siegeofcoruscant";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class Dfs081SoCAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification
            (
                HostName,
                IsDiceModificationAvailable,
                GetDiceModificationAiPriority,
                DiceModificationType.Cancel,
                1,
                new List<DieSide> { DieSide.Crit },
                timing: DiceModificationTimingType.Opposite,
                payAbilityCost: PayAbilityCost
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        public bool IsDiceModificationAvailable()
        {
            return (Combat.AttackStep == CombatStep.Attack
                && HostShip.Tokens.HasToken<Tokens.CalculateToken>()
                && HostShip.State.Charges > 0);
        }

        private void PayAbilityCost(Action<bool> callback)
        {
            if (HostShip.Tokens.HasToken<Tokens.CalculateToken>()
                && HostShip.State.Charges > 0)
            {
                HostShip.LoseCharge();
                HostShip.Tokens.SpendToken(typeof(Tokens.CalculateToken), () => callback(true));
            }
            else
            {
                callback(false);
            }
        }

        public int GetDiceModificationAiPriority()
        {
            return 95;
        }
    }
}
