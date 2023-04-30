using Content;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.DroidTriFighter
{
    public class DisT81SoC : DroidTriFighter
    {
        public DisT81SoC()
        {
            PilotInfo = new PilotCardInfo25
            (
                "DIS-T81",
                "Siege of Coruscant",
                Faction.Separatists,
                4,
                5,
                0,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.DisT81SoCAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Configuration,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.Droid
                },
                isStandardLayout: true
            );

            MustHaveUpgrades.Add(typeof(Outmaneuver));
            MustHaveUpgrades.Add(typeof(AfterBurners));
            MustHaveUpgrades.Add(typeof(ContingencyProtocol));

            ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/d/db/Dist81-siegeofcoruscant.png";

            PilotNameCanonical = "dist81-siegeofcoruscant";
        }
    }
}

// TODO: Don't remove deplete token just after you get it

namespace Abilities.SecondEdition
{
    public class DisT81SoCAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification
            (
                HostName,
                IsDiceModificationAvailable,
                GetDiceModificationAiPriority,
                DiceModificationType.Reroll,
                int.MaxValue,
                payAbilityCost: PrepareAbilityCost
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        public bool IsDiceModificationAvailable()
        {
            return (Combat.AttackStep == CombatStep.Attack && Combat.Attacker == HostShip)
                || (Combat.AttackStep == CombatStep.Defence && Combat.Defender == HostShip);
        }

        private int GetDiceModificationAiPriority()
        {
            return 0;
        }

        private void PrepareAbilityCost(Action<bool> callback)
        {
            HostShip.OnImmediatelyAfterReRolling += AssignTokensForReroll;
            callback(true);
        }

        private void AssignTokensForReroll(DiceRoll diceroll)
        {
            HostShip.OnImmediatelyAfterReRolling -= AssignTokensForReroll;

            if (diceroll.DiceRerolled.Count > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnImmediatelyAfterReRolling, delegate { AssignTokens(diceroll.DiceRerolled.Count); });
            }
        }

        private void AssignTokens(int count)
        {
            if (Combat.AttackStep == CombatStep.Attack)
            {
                HostShip.Tokens.AssignTokens(CreateDepleteToken, count, Triggers.FinishTrigger);
            }
            else if (Combat.AttackStep == CombatStep.Defence)
            {
                HostShip.Tokens.AssignTokens(CreateStrainToken, count, Triggers.FinishTrigger);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private GenericToken CreateStrainToken()
        {
            return new StrainToken(HostShip);
        }

        private GenericToken CreateDepleteToken()
        {
            return new DepleteToken(HostShip);
        }
    }
}