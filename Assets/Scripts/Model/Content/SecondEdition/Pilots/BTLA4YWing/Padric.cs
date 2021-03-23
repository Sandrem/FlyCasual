using Bombs;
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
                RequiredMods = new List<Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };

                PilotInfo = new PilotCardInfo(
                    "Padric",
                    3,
                    35,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.PadricAbility),
                    extraUpgradeIcons: new List<UpgradeType>() { UpgradeType.Talent, UpgradeType.Illicit },
                    factionOverride: Faction.Scum
                );

                ModelInfo.SkinName = "Gray";

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