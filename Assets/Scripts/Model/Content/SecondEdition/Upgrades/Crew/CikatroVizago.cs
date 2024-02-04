using Ship;
using Upgrade;
using UnityEngine;
using Movement;
using System;
using SubPhases;
using System.Linq;

namespace UpgradesList.SecondEdition
{
    public class CikatroVizago : GenericUpgrade
    {
        public CikatroVizago() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Cikatro Vizago",
                UpgradeType.Crew,
                cost: 1,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Scum),
                abilityType: typeof(Abilities.SecondEdition.CikatroVizagoAbility),
                seImageNumber: 131
            );

            Avatar = new AvatarInfo(
                Faction.Scum,
                new Vector2(417, 23)
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class CikatroVizagoAbility : GenericAbility
    {
        public GenericShip[] SelectedShips { get; private set; }
        public GenericUpgrade[] SelectedUpgrades { get; private set; }
        public UpgradeSlot[] SelectedUpgradeSlots { get; private set; }

        public override void ActivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers -= CheckAbility;
        }

        private void CheckAbility()
        {
            if (HasEnoughTargets())
            {
                RegisterAbilityTrigger(TriggerTypes.OnEndPhaseStart, AskToUseOwnAbility);
            }
        }

        private bool HasEnoughTargets()
        {
            return BoardTools.Board.GetShipsAtRange(HostShip, new Vector2(0, 1), Team.Type.Friendly)
                .Count(n => n.UpgradeBar.HasUpgradeTypeInstalled(UpgradeType.Illicit)) >= 2;
        }

        private void AskToUseOwnAbility(object sender, EventArgs e)
        {
            SelectedShips = new GenericShip[2];
            SelectedUpgrades = new GenericUpgrade[2];
            SelectedUpgradeSlots = new UpgradeSlot[2];

            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                AskToSelectFirstShip,
                descriptionLong: "Do you want to choose 2 Illicit upgrades equipped to friendly ships at range 0-1 to exchange these upgrades?",
                imageHolder: HostUpgrade
            );
        }

        private void AskToSelectFirstShip(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            SelectShipWithIllicit(0);
        }

        private void SelectShipWithIllicit(int index)
        {
            SelectTargetForAbility(
                delegate { ShipIsSelected(index); },
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                name: HostUpgrade.UpgradeInfo.Name,
                description: "Select ship #" + (index + 1),
                imageSource: HostUpgrade,
                showSkipButton: false
            );
        }

        private void ShipIsSelected(int index)
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            SelectedShips[index] = TargetShip;
            StartSelectIllicitUpgradeSubphase(
                "Choose illicit upgrade to swap",
                SelectedShips[index],
                (GenericUpgrade illicitUpgrade) => { UpgradeCardIsSelected(index, illicitUpgrade); },
                delegate { WhenSubphaseIsFinished(index); }
            );
        }

        private void StartSelectIllicitUpgradeSubphase(string descriptionText, GenericShip shipWithUpgrades, Action<GenericUpgrade> doWithIllicitUpgrade, Action subphaseCallback)
        {
            SelectIllicitUpgradeDecisionSubphase subphase = Phases.StartTemporarySubPhaseNew<SelectIllicitUpgradeDecisionSubphase>(
                descriptionText,
                subphaseCallback
            );

            subphase.DescriptionShort = "Cikatro Vizago";
            subphase.DescriptionLong = descriptionText;
            subphase.ImageSource = HostUpgrade;

            subphase.RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;
            subphase.DecisionViewType = DecisionViewTypes.ImagesUpgrade;

            foreach (GenericUpgrade illicitUpgrade in shipWithUpgrades.UpgradeBar.GetInstalledUpgrades(UpgradeType.Illicit))
            {
                subphase.AddDecision(
                    illicitUpgrade.UpgradeInfo.Name,
                    delegate { doWithIllicitUpgrade(illicitUpgrade); },
                    illicitUpgrade.ImageUrl
                );
            }

            subphase.DefaultDecisionName = subphase.GetDecisions().First().Name;

            subphase.Start();
        }

        private void UpgradeCardIsSelected(int index, GenericUpgrade illicitUpgrade)
        {
            SelectedUpgrades[index] = illicitUpgrade;
            SelectedUpgradeSlots[index] = illicitUpgrade.Slot;
            DecisionSubPhase.ConfirmDecision();
        }

        private void WhenSubphaseIsFinished(int index)
        {
            if (index == 0)
            {
                SelectShipWithIllicit(1);
            }
            else if (index == 1)
            {
                SwapUpgrades();
            }
        }

        private void SwapUpgrades()
        {
            for (int i = 0; i < 2; i++)
            {
                foreach (var upgradeAbility in SelectedUpgrades[i].UpgradeAbilities)
                {
                    upgradeAbility.DeactivateAbility();
                }
            }

            for (int i = 0; i < 2; i++)
            {
                int anotherIndex = (i == 0) ? 1 : 0;
                SelectedUpgradeSlots[i].PreInstallUpgrade(SelectedUpgrades[anotherIndex], SelectedShips[i]);
                Roster.ReplaceUpgrade(SelectedShips[i], SelectedUpgrades[i].State.Name, SelectedUpgrades[anotherIndex].State.Name, SelectedUpgrades[anotherIndex].ImageUrl);
                SelectedUpgrades[anotherIndex].ActivateAbility();
            }

            Messages.ShowInfo(string.Format("{0}: {1} and {2} were swapped", HostUpgrade.UpgradeInfo.Name, SelectedUpgrades[0].UpgradeInfo.Name, SelectedUpgrades[1].UpgradeInfo.Name));

            Triggers.FinishTrigger();
        }

        private bool FilterTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, TargetTypes.This, TargetTypes.OtherFriendly)
                && FilterTargetsByRange(ship, 0, 1)
                && ship.UpgradeBar.HasUpgradeTypeInstalled(UpgradeType.Illicit)
                && !SelectedShips.Contains(ship);
        }

        private int GetAiPriority(GenericShip ship)
        {
            return 0;
        }

        public class SelectIllicitUpgradeDecisionSubphase : DecisionSubPhase { }
    }
}