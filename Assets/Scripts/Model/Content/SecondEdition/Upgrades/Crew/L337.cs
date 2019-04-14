using Upgrade;
using Ship;
using ActionsList;
using System;
using SubPhases;
using Actions;
using BoardTools;
using System.Linq;
using Movement;
using Tokens;

namespace UpgradesList.SecondEdition
{
    public class L337Crew : GenericDualUpgrade
    {
        public L337Crew() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "L3-37",
                UpgradeType.Crew,
                cost: 4,
                restriction: new FactionRestriction(Faction.Scum),
                abilityType: typeof(Abilities.SecondEdition.L337CrewAbility),
                seImageNumber: 158
            );

            SelectSideOnSetup = false;
            AnotherSide = typeof(L337sProgramming);
        }
    }

    public class L337sProgramming : GenericDualUpgrade
    {
        public L337sProgramming() : base()
        {
            IsHidden = true; // Hidden in Squad Builder only

            UpgradeInfo = new UpgradeCardInfo(
                "L3-37's Programming",
                UpgradeType.Configuration,
                cost: 4,
                restriction: new FactionRestriction(Faction.Scum),
                abilityType: typeof(Abilities.SecondEdition.L337sProgrammingAbility),
                seImageNumber: 158
            );

            AnotherSide = typeof(L337Crew);
            IsSecondSide = true;
        }
    }
}

namespace Abilities.SecondEdition
{
    public class L337CrewAbility : GenericAbility
    {
        //While you defend, you may flip this card. If you do, the attacker must reroll all attack dice.
        public override void ActivateAbility()
        {

            AddDiceModification(
                HostName,
                IsDiceModificationAvailable,
                GetAiPriority,
                DiceModificationType.Reroll,
                int.MaxValue,
                payAbilityCost: PayAbilityCost,
                timing: DiceModificationTimingType.Opposite,
                isForcedFullReroll: true
            );
        }

        private void PayAbilityCost(Action<bool> callback)
        {
            (HostUpgrade as GenericDualUpgrade).Flip();
            callback(true);
        }

        private int GetAiPriority()
        {
            //Only use if expected evade results are lower than expected hits

            double expectedEvades = HostShip.State.Agility * 3 / 8.0;
            if (HostShip.Tokens.HasToken<FocusToken>())
            {
                expectedEvades += HostShip.State.Agility * 2 / 8.0;
            }
            else
            {
                var forceAndCalculates = Combat.Attacker.Tokens.CountTokensByType<CalculateToken>() + Combat.Attacker.Tokens.CountTokensByType<ForceToken>();
                expectedEvades += Math.Min(forceAndCalculates, HostShip.State.Agility) * 2 / 8.0;
            }            

            double expectedHits = Combat.CurrentDiceRoll.Successes;
            var misses = Combat.CurrentDiceRoll.Blanks;
            if (Combat.Attacker.Tokens.HasToken<FocusToken>())
            {
                expectedHits += Combat.CurrentDiceRoll.Focuses;
            }
            else
            {
                var forceAndCalculates = Combat.Attacker.Tokens.CountTokensByType<CalculateToken>() + Combat.Attacker.Tokens.CountTokensByType<ForceToken>();
                expectedHits += Math.Min(forceAndCalculates, Combat.CurrentDiceRoll.Focuses);
                misses += Math.Abs(forceAndCalculates - Combat.CurrentDiceRoll.Focuses);
            }
            if (Combat.Attacker.Tokens.GetTargetLockLetterPairsOn(HostShip).Any())
            {
                expectedHits += misses * 0.5;
            }

            if (expectedHits > expectedEvades)
                return 100;
            else
                return 0; 
        }

        private bool IsDiceModificationAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack && Combat.Defender == HostShip;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }

    public class L337sProgrammingAbility : GenericAbility
    {
        //If you are not shielded, decrease the difficulty of your bank maneuvers.
        public override void ActivateAbility()
        {
            HostShip.AfterGetManeuverColorDecreaseComplexity += DecreaseDifficulty;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGetManeuverColorDecreaseComplexity -= DecreaseDifficulty;
        }

        private void DecreaseDifficulty(GenericShip ship, ref ManeuverHolder movement)
        {
            if (HostShip.State.ShieldsCurrent == 0 && movement.ColorComplexity != MovementComplexity.None && movement.Bearing == ManeuverBearing.Bank)
            {
                movement.ColorComplexity = GenericMovement.ReduceComplexity(movement.ColorComplexity);
            }
        }
    }
}
