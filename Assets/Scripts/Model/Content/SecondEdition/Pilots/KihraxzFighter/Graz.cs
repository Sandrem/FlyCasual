using BoardTools;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.KihraxzFighter
    {
        public class Graz : KihraxzFighter
        {
            public Graz() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Graz",
                    4,
                    47,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.GrazAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 192
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class GrazAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotStartAsAttacker += CheckConditionsOffense;
            HostShip.OnShotStartAsDefender += CheckConditionsDefense;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotStartAsAttacker -= CheckConditionsOffense;
            HostShip.OnShotStartAsDefender -= CheckConditionsDefense;
        }

        private void CheckConditionsDefense()
        {
            if (Board.IsShipInFacingOnly(Combat.Attacker, HostShip, Arcs.ArcFacing.Rear180))
            {
                HostShip.AfterGotNumberOfDefenceDice += RollExtraDefenseDice;
            }
        }

        private void CheckConditionsOffense()
        {
            if (Board.IsShipInFacingOnly(Combat.Defender, HostShip, Arcs.ArcFacing.Rear180))
            {
                HostShip.AfterGotNumberOfAttackDice += RollExtraAttackDice;
            }
        }

        private void RollExtraDefenseDice(ref int count)
        {
            count++;
            Messages.ShowInfo("Graz is behind the combatant. Roll an additional die.");
            HostShip.AfterGotNumberOfDefenceDice -= RollExtraDefenseDice;
        }

        private void RollExtraAttackDice(ref int count)
        {
            count++;
            Messages.ShowInfo("Graz is behind the combatant. Roll an additional die.");
            HostShip.AfterGotNumberOfAttackDice -= RollExtraAttackDice;
        }
    }
}

