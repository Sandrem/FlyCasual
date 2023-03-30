using Abilities.SecondEdition;
using Ship;
using SubPhases;
using System;
using Content;
using System.Collections.Generic;
using Tokens;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLA4YWing
    {
        public class DexTireeBoY : BTLA4YWing
        {
            public DexTireeBoY() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Dex Tiree",
                    "Battle of Yavin",
                    Faction.Rebel,
                    2,
                    4,
                    0,
                    isLimited: true,
                    abilityType: typeof(DexTireeBoYAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Turret,
                        UpgradeType.Torpedo,
                        UpgradeType.Astromech
                    },
                    tags: new List<Tags>
                    {
                        Tags.YWing
                    },
                    isStandardLayout: true
                );

                ShipAbilities.Add(new HopeAbility());

                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.DorsalTurret));
                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.AdvProtonTorpedoes));
                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.R4Astromech));

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/f/ff/Dextiree-battleofyavin.png";

                PilotNameCanonical = "dextiree-battleofyavin";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DexTireeBoYAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfDefenceDice += TryAddDice;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfDefenceDice -= TryAddDice;
        }

        private void TryAddDice(ref int count)
        {
            int anotherFriendlyShipsAtRange = BoardTools.Board.GetShipsAtRange(Combat.Defender, new Vector2(0, 1), Team.Type.Friendly).Count - 1;

            if (Combat.AttackStep == CombatStep.Defence && Combat.Defender == HostShip && anotherFriendlyShipsAtRange > 0)
            {
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + " rolls 1 additional die");
                count++;
            }
        }
    }
}