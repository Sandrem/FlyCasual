using Bombs;
using Content;
using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLA4YWing
    {
        public class Padric : BTLA4YWing
        {
            public Padric() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Padric",
                    "Napkin Bomber",
                    Faction.Scum,
                    3,
                    4,
                    10,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.PadricAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Tech,
                        UpgradeType.Turret,
                        UpgradeType.Torpedo,
                        UpgradeType.Missile,
                        UpgradeType.Astromech,
                        UpgradeType.Device,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.YWing
                    },
                    skinName: "Gray"
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/df/43/df43e318-057c-4c0e-9419-104687ed1ef2/swz85_ship_padric.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    // TODO
    // You need to lock the bomb first

    public class PadricAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericBomb.OnBombIsDetonated += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericBomb.OnBombIsDetonated -= CheckAbility;
        }

        private void CheckAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnBombIsDetonated, TryToDealStrainTokens);
        }

        private void TryToDealStrainTokens(object sender, EventArgs e)
        {
            List<GenericShip> shipsInRange = BombsManager.GetShipsInRange(BombsManager.CurrentBombObject);
            shipsInRange = shipsInRange.Where(n => !Tools.IsSameTeam(HostShip, n)).ToList();

            foreach (GenericShip enemyShip in shipsInRange)
            {
                RegisterAbilityTrigger(
                    TriggerTypes.OnAbilityDirect,
                    delegate { DealStrainTo(enemyShip); },
                    customTriggerName: $"Assign Strain token (ID:{enemyShip.ShipId})"
                );
            }

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, Triggers.FinishTrigger);
        }

        private void DealStrainTo(GenericShip enemyShip)
        {
            enemyShip.Tokens.AssignToken(
                typeof(Tokens.StrainToken),
                Triggers.FinishTrigger
            );
        }
    }
}