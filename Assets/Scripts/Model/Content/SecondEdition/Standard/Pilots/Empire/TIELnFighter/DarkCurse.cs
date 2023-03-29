using System.Collections.Generic;
using Ship;
using Abilities.SecondEdition;
using Upgrade;
using Content;
using UpgradesList.SecondEdition;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class DarkCurse : TIELnFighter
        {
            public DarkCurse() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Dark Curse\"",
                    "Battle of Yavin",
                    Faction.Imperial,
                    6,
                    4,
                    0,
                    isLimited: true,
                    abilityType: typeof(DarkCurseAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    isStandardLayout: true
                );

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/9/90/Darkcurse-battleofyavin.png";

                MustHaveUpgrades.Add(typeof(Ruthless));
                MustHaveUpgrades.Add(typeof(PrecisionIonEngines));

                ShipInfo.Hull++;

                PilotNameCanonical = "darkcurse-battleofyavin";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DarkCurseAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsDefender += AddDarkCursePilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsDefender -= AddDarkCursePilotAbility;
            if (Combat.Attacker != null) RemoveDarkCursePilotAbility(Combat.Attacker);
        }

        private void AddDarkCursePilotAbility()
        {
            Combat.Attacker.OnTryAddAvailableDiceModification += UseDarkCurseRestriction;
            Combat.Attacker.OnAttackFinish += RemoveDarkCursePilotAbility;
        }

        private void UseDarkCurseRestriction(GenericShip ship, ActionsList.GenericAction diceModification, ref bool canBeUsed)
        {
            if (!diceModification.IsNotRealDiceModification)
            {
                Messages.ShowErrorToHuman(HostShip.PilotInfo.PilotName + ": Attacker's dice cannot be modified");
                canBeUsed = false;
            }
        }

        private void RemoveDarkCursePilotAbility(GenericShip ship)
        {
            ship.OnTryAddAvailableDiceModification -= UseDarkCurseRestriction;
            ship.OnAttackFinish -= RemoveDarkCursePilotAbility;
        }
    }
}