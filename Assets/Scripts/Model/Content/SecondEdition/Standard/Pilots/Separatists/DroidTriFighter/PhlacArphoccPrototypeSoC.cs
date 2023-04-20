using Content;
using Ship;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.DroidTriFighter
{
    public class PhlacArphoccPrototypeSoC : DroidTriFighter
    {
        public PhlacArphoccPrototypeSoC()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Phlac-Arphocc Prototype",
                "Siege of Coruscant",
                Faction.Separatists,
                5,
                4,
                0,
                limited: 2,
                abilityType: typeof(Abilities.SecondEdition.PhlacArphoccPrototypeSoCAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Configuration,
                    UpgradeType.Modification,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.Droid
                },
                isStandardLayout: true
            );

            MustHaveUpgrades.Add(typeof(AfterBurners));
            MustHaveUpgrades.Add(typeof(ContingencyProtocol));
            MustHaveUpgrades.Add(typeof(EvasionSequence7));

            ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/5/5a/SWZ95_PhlacArphoccPrototype.png";

            PilotNameCanonical = "phlacarphoccprototype-siegeofcoruscant";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PhlacArphoccPrototypeSoCAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= CheckAbility;
        }

        private void CheckAbility()
        {
            foreach (GenericShip enemyShip in HostShip.Owner.EnemyShips.Values)
            {
                if (HostShip.SectorsInfo.IsShipInSector(enemyShip, Arcs.ArcType.Bullseye))
                {
                    RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AssignCalculateToken);
                    return;
                }
            }
        }

        private void AssignCalculateToken(object sender, EventArgs e)
        {
            Messages.ShowInfo($"{HostShip.PilotInfo.PilotName} (ID:{HostShip.ShipId}) gains calculate token");
            HostShip.Tokens.AssignToken(typeof(CalculateToken), Triggers.FinishTrigger);
        }
    }
}