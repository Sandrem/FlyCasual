using Ship;
using System;
using System.Collections.Generic;
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
                () => 35,
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
                () => 20,
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
    }
}
