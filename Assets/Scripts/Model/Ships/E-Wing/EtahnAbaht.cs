using RuleSets;
using BoardTools;
using System.Collections.Generic;

namespace Ship
{
    namespace EWing
    {
        public class EtahnAbaht : EWing
        {
            public EtahnAbaht() : base()
            {
                PilotName = "Etahn A'baht";
                PilotSkill = 5;
                Cost = 32;

                IsUnique = true;
                
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.EtahnAbahtAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
            }
        }
    }
}

namespace Abilities
{
    //When an enemy ship inside your firing arc at Range 1-3 is defending, the attacker may change 1 of its hit results to a critical result.
    public class EtahnAbahtAbility : GenericAbility
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
            var shotInfo = new ShotInfo(HostShip, Combat.Defender, HostShip.PrimaryWeapon);

            return Combat.AttackStep == CombatStep.Attack
                && Combat.Defender.Owner != HostShip.Owner
                && shotInfo.InPrimaryArc
                && shotInfo.Range >= 1 
                && shotInfo.Range <= 3;
        }
    }
}
