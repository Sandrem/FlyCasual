using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.FangFighter
    {
        public class FennRau : FangFighter
        {
            public FennRau() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Fenn Rau",
                    "Skull Leader",
                    Faction.Scum,
                    6,
                    7,
                    18,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.FennRauScumAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Torpedo
                    },
                    tags: new List<Tags>
                    {
                        Tags.Mandalorian
                    },
                    seImageNumber: 155
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class FennRauScumAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckFennRauAbility;
            HostShip.AfterGotNumberOfDefenceDice += CheckFennRauAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= CheckFennRauAbility;
            HostShip.AfterGotNumberOfDefenceDice -= CheckFennRauAbility;
        }

        private void CheckFennRauAbility(ref int value)
        {
            if (Combat.ShotInfo.Range == 1)
            {
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": The attack is at range 1, attacker gains +1 attack die");
                value++;
            }
        }
    }
}