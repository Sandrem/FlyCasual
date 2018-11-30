using BoardTools;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.EWing
    {
        public class GavinDarklighter : EWing
        {
            public GavinDarklighter() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Gavin Darklighter",
                    4,
                    68,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.GavinDarklighterAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 51
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //While a friendly ship performs an attack, if the defender is in your arc, the attacker may change 1 hit result to a critical hit result.
    public class GavinDarklighterAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostName,
                IsAvailable,
                () => 20,
                DiceModificationType.Change,
                1,
                new List<DieSide> { DieSide.Success },
                DieSide.Crit,
                isGlobal: true
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack
                && Combat.Attacker.Owner == HostShip.Owner
                && new ShotInfo(HostShip, Combat.Defender, HostShip.PrimaryWeapon).InPrimaryArc;
        }
    }
}
