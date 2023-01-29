using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.CloneZ95Headhunter
    {
        public class Knack : CloneZ95Headhunter
        {
            public Knack() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Knack\"",
                    "Incautious Instructor",
                    Faction.Republic,
                    5,
                    3,
                    7,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.KnackAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Clone
                    }
                );

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/knack.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class KnackAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShipIsDestroyed += TryRegisterDestructionAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShipIsDestroyed -= TryRegisterDestructionAbility;
        }

        private void TryRegisterDestructionAbility(GenericShip ship, bool isFled)
        {
            if (HostShip.UpgradeBar.GetInstalledUpgrades(UpgradeType.Talent).Count > 0
                && BoardTools.Board.GetShipsAtRange(HostShip, new UnityEngine.Vector2(0, 1), Team.Type.Friendly).FindAll(s => s.PilotInfo.IsLimited).Count > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnShipIsDestroyed, AskSelectFriendlyShip);
            }
        }

        private void AskSelectFriendlyShip(object sender, System.EventArgs e)
        {
            SelectTargetForAbility(
                ShipIsSelected,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                name: HostShip.PilotInfo.PilotName,
                description: "Select friendly non-limited ship at range 0-2",
                imageSource: HostShip,
                showSkipButton: true
            );
        }

        private void ShipIsSelected()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();
            StartSelectIllicitUpgradeSubphase(
                "Choose Talent upgrade to assign to " + TargetShip.PilotInfo.PilotName,
                (GenericUpgrade talentUpgrade) => { UpgradeCardIsSelected(talentUpgrade); },
                delegate { Triggers.FinishTrigger(); }
            );
        }

        private void StartSelectIllicitUpgradeSubphase(string descriptionText, Action<GenericUpgrade> doWithIllicitUpgrade, Action subphaseCallback)
        {
            AssignTalentUpgradeDecisionSubphase subphase = Phases.StartTemporarySubPhaseNew<AssignTalentUpgradeDecisionSubphase>(
                descriptionText,
                subphaseCallback
            );

            subphase.DescriptionShort = "Knack";
            subphase.DescriptionLong = descriptionText;
            subphase.ImageSource = HostUpgrade;

            subphase.RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;
            subphase.DecisionViewType = DecisionViewTypes.ImagesUpgrade;

            foreach (GenericUpgrade talentUpgrade in HostShip.UpgradeBar.GetInstalledUpgrades(UpgradeType.Talent))
            {
                subphase.AddDecision(
                    talentUpgrade.UpgradeInfo.Name,
                    delegate { doWithIllicitUpgrade(talentUpgrade); },
                    talentUpgrade.ImageUrl
                );
            }

            subphase.DefaultDecisionName = subphase.GetDecisions().First().Name;

            subphase.Start();
        }

        private void UpgradeCardIsSelected(GenericUpgrade talentUpgrade)
        {
            TargetShip.PilotInfo.ExtraUpgrades.Add(UpgradeType.Talent);

            talentUpgrade.PreAttachToShip(TargetShip);
            talentUpgrade.AttachToShip(TargetShip);
            Roster.UpdateUpgradesPanel(TargetShip, TargetShip.InfoPanel);
            Roster.SubscribeUpgradesPanel(TargetShip, TargetShip.InfoPanel);
            DecisionSubPhase.ConfirmDecisionNoCallback();
            Triggers.FinishTrigger();
        }

        public class AssignTalentUpgradeDecisionSubphase : DecisionSubPhase { }

        private bool FilterTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, TargetTypes.OtherFriendly)
                && FilterTargetsByRange(ship, 0, 1)
                && !ship.PilotInfo.IsLimited;
        }

        private int GetAiPriority(GenericShip ship)
        {
            return 0;
        }
    }
}