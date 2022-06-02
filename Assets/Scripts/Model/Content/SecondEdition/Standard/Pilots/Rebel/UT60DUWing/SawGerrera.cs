using Abilities.SecondEdition;
using ActionsList;
using Content;
using Ship;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.UT60DUWing
    {
        public class SawGerrera : UT60DUWing
        {
            public SawGerrera() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Saw Gerrera",
                    "Obsessive Outlaw",
                    Faction.Rebel,
                    4,
                    5,
                    18,
                    isLimited: true,
                    abilityType: typeof(SawGerreraPilotAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Illicit,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Partisan
                    },
                    seImageNumber: 55,
                    skinName: "Partisan"
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SawGerreraPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal += AddSawGarreraAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal -= AddSawGarreraAbility;
        }

        private void AddSawGarreraAbility(GenericShip ship)
        {
            Combat.Attacker.AddAvailableDiceModification(new SawGarreraAction(), HostShip);
        }

        private class SawGarreraAction : FriendlyRerollAction
        {
            public SawGarreraAction() : base(1, 2, true, RerollTypeEnum.AttackDice)
            {
                Name = DiceModificationName = "Saw Gerrera";
                ImageUrl = new Ship.SecondEdition.UT60DUWing.SawGerrera().ImageUrl;
            }

            public override bool IsDiceModificationAvailable()
            {
                bool result = false;
                if (Combat.Attacker.Damage.IsDamaged) result = base.IsDiceModificationAvailable();
                return result;
            }
        }
    }
}
