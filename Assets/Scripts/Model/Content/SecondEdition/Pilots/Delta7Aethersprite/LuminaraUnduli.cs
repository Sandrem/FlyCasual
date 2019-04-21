using Ship;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.Delta7Aethersprite
{
    public class LuminaraUnduli : Delta7Aethersprite
    {
        public LuminaraUnduli()
        {
            PilotInfo = new PilotCardInfo(
                "Luminara Unduli",
                4,
                44,
                true,
                force: 2,
                abilityType: typeof(Abilities.SecondEdition.LuminaraUnduliAbility),
                extraUpgradeIcon: UpgradeType.Force,
                pilotTitle: "Wise Protector"
            );

            ModelInfo.SkinName = "Blue";

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/7f87b6c12631687bedf75a18582af0b0.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    //While a friendly ship at range 0–2 defends, if it is not in the attacker’s bullseye arc, you may spend 1 force. 
    //If you do, change 1 crit result to a hit result or 1 hit result to a focus result.
    public class LuminaraUnduliAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            //Implemented as two separate dice modifications. Ideally AddDiceModification would support doing this as one mod

            AddDiceModification(
                HostName + ": crit to hit",
                IsAvailable,
                GetAICritModPriority,
                DiceModificationType.Change,
                1, 
                new List<DieSide> { DieSide.Crit},
                DieSide.Success,
                DiceModificationTimingType.Opposite,
                isGlobal: true,
                payAbilityCost: PayAbilityCost
            );

            AddDiceModification(
                HostName + ": hit to focus",
                IsAvailable,
                GetAINormalModPriority,
                DiceModificationType.Change,
                1,
                new List<DieSide> { DieSide.Success },
                DieSide.Focus,
                DiceModificationTimingType.Opposite,
                isGlobal: true,
                payAbilityCost: PayAbilityCost
            );

            GenericShip.OnAttackFinishGlobal += ResetUsedFlag;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
            GenericShip.OnAttackFinishGlobal -= ResetUsedFlag;
        }

        private void ResetUsedFlag(GenericShip ship)
        {
            ClearIsAbilityUsedFlag();
        }

        protected virtual bool IsAvailable()
        {
            return
                !IsAbilityUsed 
                && HostShip.State.Force > 0
                && Combat.AttackStep == CombatStep.Attack
                && Combat.Defender.Owner == HostShip.Owner
                && new BoardTools.DistanceInfo(HostShip, Combat.Defender).Range < 3
                && !Combat.Attacker.SectorsInfo.IsShipInSector(Combat.Defender, Arcs.ArcType.Bullseye);
        }

        private void PayAbilityCost(Action<bool> callback)
        {
            if (HostShip.State.Force > 0)
            {
                HostShip.State.Force--;
                IsAbilityUsed = true;
                callback(true);
            }
            else
            {
                callback(false);
            }
        }

        private int GetAICritModPriority()
        {
            int result = 0;


            int critSuccesses = Combat.CurrentDiceRoll.CriticalSuccesses;
            int numSuccesses = Combat.CurrentDiceRoll.Successes;
            int currentShields = Combat.Defender.State.ShieldsCurrent;
            if (critSuccesses > 0 && currentShields < numSuccesses && Combat.Defender.State.HullCurrent + currentShields >= numSuccesses && Combat.Defender.State.Agility > 0)
            {
                // We have don't have enough shields to mitigate this critical hit, and there's a chance we'll survive the attack.  Use this ability.
                result = 35;
            }
            return result;
        }

        private int GetAINormalModPriority()
        {
            int result = 0;
            int hitSuccesses = Combat.CurrentDiceRoll.Successes - Combat.CurrentDiceRoll.CriticalSuccesses;
            int focusResults = Combat.CurrentDiceRoll.Focuses;
            if (hitSuccesses > 0)
            {
                if (Combat.Attacker.Tokens.CountTokensByType(typeof(FocusToken)) == 0 && Combat.Attacker.State.Force <= focusResults && Combat.Attacker.Tokens.CountTokensByType(typeof(CalculateToken)) == 0)
                {
                    // The attack doesn't have a focus token or a calculate, and doesn't have enough Force to convert the focus back into a hit.
                    // Use this ability.
                    result = 20;
                }
            }
            return result;
        }
    }
}
