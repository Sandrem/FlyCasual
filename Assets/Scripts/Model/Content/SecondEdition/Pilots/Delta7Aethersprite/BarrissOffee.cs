using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.Delta7Aethersprite
{
    public class BarrissOffee : Delta7Aethersprite
    {
        public BarrissOffee()
        {
            PilotInfo = new PilotCardInfo(
                "Barriss Offee",
                4,
                43,
                true,
                force: 1,
                abilityType: typeof(Abilities.SecondEdition.BarrissOffeeAbility),
                extraUpgradeIcon: UpgradeType.Force,
                pilotTitle: "Conflicted Padawan"
            );

            ModelInfo.SkinName = "Blue";

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/f8f58005da18d7041b1ec2374701465c.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    //While a friendly ship at range 0–2 performs an attack, if the defender is in its bullseye arc,
    //you may spend 1 force to change 1 focus result to a hit result or 1 hit result to a crit result.
    public class BarrissOffeeAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            //Implemented as two separate dice modifications. Ideally AddDiceModification would support doing this as one mod

            AddDiceModification(
                HostName + ": focus to hit",
                IsAvailable,
                () => 35,
                DiceModificationType.Change,
                1, 
                new List<DieSide> { DieSide.Focus},
                DieSide.Success,
                isGlobal: true,
                payAbilityCost: PayAbilityCost
            );

            AddDiceModification(
                HostName + ": hit to crit",
                IsAvailable,
                () => 20,
                DiceModificationType.Change,
                1,
                new List<DieSide> { DieSide.Success },
                DieSide.Crit,
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
                && Combat.Attacker.Owner == HostShip.Owner
                && new BoardTools.DistanceInfo(HostShip, Combat.Attacker).Range < 3
                && Combat.Attacker.SectorsInfo.IsShipInSector(Combat.Defender, Arcs.ArcType.Bullseye);
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
    }
}
