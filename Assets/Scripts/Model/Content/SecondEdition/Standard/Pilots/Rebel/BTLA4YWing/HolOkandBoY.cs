using Abilities.SecondEdition;
using Ship;
using SubPhases;
using System;
using Content;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using BoardTools;
using System.Linq;

namespace Ship
{
    namespace SecondEdition.BTLA4YWing
    {
        public class HolOkandBoY : BTLA4YWing
        {
            public HolOkandBoY() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Hol Okand",
                    "Battle of Yavin",
                    Faction.Rebel,
                    2,
                    4,
                    0,
                    isLimited: true,
                    abilityType: typeof(HolOkandBoYAbility),
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
                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.PreciseAstromech));

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/a/a4/Holokand-battleofyavin.png";

                PilotNameCanonical = "holokand-battleofyavin";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class HolOkandBoYAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCheckSystemsAbilityActivation += CheckAbility;
            HostShip.OnSystemsAbilityActivation += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckSystemsAbilityActivation -= CheckAbility;
            HostShip.OnSystemsAbilityActivation -= RegisterAbility;
        }

        private void CheckAbility(GenericShip ship, ref bool flag)
        {
            flag = HasEnemyShipsAtR12AndHasRechargableUpgrades();
        }

        private bool HasEnemyShipsAtR12AndHasRechargableUpgrades()
        {
            return (
                Board.GetShipsAtRange(HostShip, new Vector2(1, 2), Team.Type.Enemy).Count == 0
                && HasRechargableUpgrades()
            );
        }

        private bool HasRechargableUpgrades()
        {
            return HostShip.UpgradeBar.GetRechargableUpgrades().Count > 0;
        }

        private void RegisterAbility(GenericShip ship)
        {
            if (HasEnemyShipsAtR12AndHasRechargableUpgrades())
            {
                RegisterAbilityTrigger(TriggerTypes.OnSystemsAbilityActivation, AskToRechargeUpgrade);
            }
        }

        private void AskToRechargeUpgrade(object sender, EventArgs e)
        {
            if (HasEnemyShipsAtR12AndHasRechargableUpgrades())
            {
                HolOkandBoYReloadDecisionSubphase subphase = Phases.StartTemporarySubPhaseNew<HolOkandBoYReloadDecisionSubphase>(
                    "Recover 1 Charge on any upgrade",
                    Triggers.FinishTrigger
                );

                subphase.DescriptionShort = "Recover 1 Charge on any upgrade";
                subphase.RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;
                subphase.DecisionViewType = DecisionViewTypes.ImagesUpgrade;

                foreach (GenericUpgrade upgrade in HostShip.UpgradeBar.GetRechargableUpgrades())
                {
                    subphase.AddDecision(
                        upgrade.UpgradeInfo.Name,
                        delegate { RechargeUpgradeAndFinish(upgrade); },
                        upgrade.ImageUrl,
                        upgrade.State.Charges
                    );
                }

                subphase.DefaultDecisionName = subphase.GetDecisions().First().Name;

                subphase.Start();
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private static void RechargeUpgradeAndFinish(GenericUpgrade upgrade)
        {
            RechargeUpgrade(upgrade);

            DecisionSubPhase.ConfirmDecision();
        }

        private static void RechargeUpgrade(GenericUpgrade upgrade)
        {
            int count = upgrade.HostShip.GetReloadChargesCount(upgrade);
            upgrade.State.RestoreCharges(count);

            string chargesText = (count == 1) ? "1 Charge" : $"{count} Charges";
            Messages.ShowInfo($"Reload: {chargesText} of {upgrade.UpgradeInfo.Name} is restored");
        }

        public class HolOkandBoYReloadDecisionSubphase : DecisionSubPhase { }
    }
}