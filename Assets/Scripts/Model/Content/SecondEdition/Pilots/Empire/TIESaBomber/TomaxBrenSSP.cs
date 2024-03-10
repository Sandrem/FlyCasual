using ActionsList;
using Content;
using SubPhases;
using System.Collections.Generic;
using System.Linq;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship
{
    namespace SecondEdition.TIESaBomber
    {
        public class TomaxBrenSSP : TIESaBomber
        {
            public TomaxBrenSSP() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Tomax Bren",
                    "Brash Maverick",
                    Faction.Imperial,
                    5,
                    5,
                    0,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.TomaxBrenSSPAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Missile,
                        UpgradeType.Missile,
                        UpgradeType.Device
                    },
                    seImageNumber: 107,
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    legality: new List<Legality>() { Legality.StandardLegal, Legality.ExtendedLegal },
                    isStandardLayout: true
                );

                ModelInfo.SkinName = "White Death";

                MustHaveUpgrades.Add(typeof(Elusive));
                MustHaveUpgrades.Add(typeof(BarrageRockets));
                MustHaveUpgrades.Add(typeof(ProximityMines));

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/tomaxbren-swz105.png";

                PilotNameCanonical = "tomaxbren-swz105";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TomaxBrenSSPAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckAbility;
        }

        private void CheckAbility(GenericAction action)
        {
            if (action is ReloadAction && HasEliteUpgradesToRecharge())
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, ShowEliteUpgradeToRecharge);
            }
        }

        private bool HasEliteUpgradesToRecharge()
        {
            return GetEliteUpgradesToRecharge().Count > 0;
        }

        private List<GenericUpgrade> GetEliteUpgradesToRecharge()
        {
            return HostShip.UpgradeBar.GetUpgradesOnlyFaceup().Where(n => n.UpgradeInfo.HasType(UpgradeType.Talent) && (n.State.Charges < n.State.MaxCharges)).ToList();
        }

        private void ShowEliteUpgradeToRecharge(object sender, System.EventArgs e)
        {
            var phase = Phases.StartTemporarySubPhaseNew<TomaxBrenSSPDecisionSubphase>(
                HostShip.PilotInfo.PilotName,
                Triggers.FinishTrigger
            );

            phase.DescriptionShort = HostShip.PilotInfo.PilotName;
            phase.DescriptionLong = "You may recover a charge:";
            phase.ImageSource = HostShip;

            phase.RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;

            phase.ShowSkipButton = true;

            List<GenericUpgrade> AvailableUpgrades = GetEliteUpgradesToRecharge();

            foreach (var upgrade in AvailableUpgrades)
            {
                phase.AddDecision(
                    upgrade.UpgradeInfo.Name,
                    delegate { RestoreCharge(upgrade); },
                    upgrade.ImageUrl,
                    upgrade.State.Charges
                );
            }

            phase.DefaultDecisionName = AvailableUpgrades[0].UpgradeInfo.Name;

            phase.DecisionViewType = DecisionViewTypes.ImagesUpgrade;

            phase.Start();
        }

        protected void RestoreCharge(GenericUpgrade upgrade)
        {
            upgrade.State.RestoreCharge();
            DecisionSubPhase.ConfirmDecision();
        }

        public class TomaxBrenSSPDecisionSubphase : DecisionSubPhase { }
    }
}