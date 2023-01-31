using BoardTools;
using Content;
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
                PilotInfo = new PilotCardInfo25(
                    "Graz",
                    "The Hunter",
                    Faction.Scum,
                    4,
                    4,
                    8,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.GrazAbility),
                    tags: new List<Tags>
                    {
                        Tags.BountyHunter,
                    },
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Missile,
                        UpgradeType.Illicit,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Modification
                    },
                    seImageNumber: 192,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
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
            if (Board.IsShipInFacingOnly(Combat.Attacker, HostShip, Arcs.ArcFacing.FullRear))
            {
                HostShip.AfterGotNumberOfDefenceDice += RollExtraDefenseDice;
            }
        }

        private void CheckConditionsOffense()
        {
            if (Board.IsShipInFacingOnly(Combat.Defender, HostShip, Arcs.ArcFacing.FullRear))
            {
                HostShip.AfterGotNumberOfAttackDice += RollExtraAttackDice;
            }
        }

        private void RollExtraDefenseDice(ref int count)
        {
            count++;
            Messages.ShowInfo(HostShip.PilotInfo.PilotName + " is behind the combatant and gains +1 defense die");
            HostShip.AfterGotNumberOfDefenceDice -= RollExtraDefenseDice;
        }

        private void RollExtraAttackDice(ref int count)
        {
            count++;
            Messages.ShowInfo(HostShip.PilotInfo.PilotName + " is behind the combatant and gains +1 attack die");
            HostShip.AfterGotNumberOfAttackDice -= RollExtraAttackDice;
        }
    }
}

