using System;
using System.Collections.Generic;
using System.Linq;
using Content;
using Ship;
using SubPhases;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEBaInterceptor
    {
        public class MajorVonreg : TIEBaInterceptor
        {
            public MajorVonreg() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Major Vonreg",
                    "Red Baron",
                    Faction.FirstOrder,
                    6,
                    6,
                    18,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.MajorVonregAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Missile,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/b6/de/b6de4a15-0b5d-4c39-8a2e-c96af5dff9fe/swz62_card_major-vonreg.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MajorVonregAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCheckSystemsAbilityActivation += CheckForAbility;
            HostShip.OnSystemsAbilityActivation += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckSystemsAbilityActivation -= CheckForAbility;
            HostShip.OnSystemsAbilityActivation -= RegisterTrigger;
        }

        private void CheckForAbility(GenericShip ship, ref bool isAbilityActive)
        {
            isAbilityActive = true;
        }

        private void RegisterTrigger(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnSystemsAbilityActivation, StartSelectShip);
        }

        private void StartSelectShip(object sender, EventArgs e)
        {
            if (Roster.AllShips.Values.Any(n => FilterTargets(n)))
            {
                SelectTargetForAbility(
                    SelectShip,
                    FilterTargets,
                    GetAiPriority,
                    HostShip.Owner.PlayerNo,
                    HostShip.PilotInfo.PilotName,
                    "You may choose a ship in your bullseye arc to assign Strain or Deplete token to it",
                    HostShip
                );
            }
            else
            {
                Messages.ShowErrorToHuman("There is no ships in bullseye arc");
                Triggers.FinishTrigger();
            }
        }

        private void SelectShip()
        {
            SelectDebuffDecisionSubphase subphase = Phases.StartTemporarySubPhaseNew<SelectDebuffDecisionSubphase>(
                "Select debuff subphase",
                Triggers.FinishTrigger
            );

            subphase.DescriptionShort = HostShip.PilotInfo.PilotName;
            subphase.DescriptionLong = "You may assign Strain or Deplete token to target";
            subphase.ImageSource = HostShip;

            subphase.DecisionOwner = HostShip.Owner;
            subphase.ShowSkipButton = false;

            subphase.AddDecision(
                "Assign Strain token",
                SelectStrainToken
            );

            subphase.AddDecision(
                "Assign Deplete token",
                SelectDepleteToken
            );

            subphase.DefaultDecisionName = "Assign Strain token";

            subphase.Start();
        }

        private void SelectDepleteToken(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            TargetShip.Tokens.AssignToken(
                typeof(DepleteToken),
                Triggers.FinishTrigger
            );
        }

        private void SelectStrainToken(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            TargetShip.Tokens.AssignToken(
                typeof(StrainToken),
                Triggers.FinishTrigger
            );
        }

        private bool FilterTargets(GenericShip ship)
        {
            return ship.Owner.PlayerNo != HostShip.Owner.PlayerNo
                && HostShip.SectorsInfo.IsShipInSector(ship, Arcs.ArcType.Bullseye);
        }

        private int GetAiPriority(GenericShip ship)
        {
            return ship.PilotInfo.Cost;
        }

        private class SelectDebuffDecisionSubphase : DecisionSubPhase { };
    }
}