using Ship;
using Upgrade;
using System.Collections.Generic;
using UnityEngine;
using SubPhases;
using System;
using BoardTools;
using Content;

namespace UpgradesList.SecondEdition
{
    public class SupremeLeaderSnoke : GenericUpgrade
    {
        public SupremeLeaderSnoke() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Supreme Leader Snoke",
                types: new List<UpgradeType>()
                {
                    UpgradeType.Crew,
                    UpgradeType.Crew
                },
                cost: 13,
                isLimited: true,
                addForce: 1,                
                restriction: new FactionRestriction(Faction.FirstOrder),
                abilityType: typeof(Abilities.SecondEdition.SupremeLeaderSnokeAbility),
                legalityInfo: new List<Legality>
                {
                    Legality.StandardBanned,
                    Legality.ExtendedLegal
                }
            );

            Avatar = new AvatarInfo(
                Faction.FirstOrder,
                new Vector2(346, 1),
                new Vector2(150, 150)
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SupremeLeaderSnokeAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnSystemsAbilityActivation += RegisterAbility;
            HostShip.OnCheckSystemsAbilityActivation += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnSystemsAbilityActivation -= RegisterAbility;
            HostShip.OnCheckSystemsAbilityActivation -= CheckAbility;
        }

        private void RegisterAbility(GenericShip ship)
        {
            if (HostShip.State.Force > 0 && Board.GetShipsAtRange(HostShip, new Vector2(2, float.MaxValue), Team.Type.Enemy).Count > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnSystemsAbilityActivation, StartMultiSelectionSubphase);
            }
        }

        private void CheckAbility(GenericShip ship, ref bool flag)
        {
            if (HostShip.State.Force > 0 && Board.GetShipsAtRange(HostShip, new Vector2(2, float.MaxValue), Team.Type.Enemy).Count > 0) flag = true;
        }

        private void StartMultiSelectionSubphase(object sender, EventArgs e)
        {
            MultiSelectionSubphase subphase = Phases.StartTemporarySubPhaseNew<MultiSelectionSubphase>("Supreme Leader Snoke", Triggers.FinishTrigger);

            subphase.RequiredPlayer = HostShip.Owner.PlayerNo;

            subphase.Filter = FilterSelection;
            subphase.GetAiPriority = GetAiPriority;
            subphase.MaxToSelect = HostShip.State.Force;
            subphase.WhenDone = FlipDialsFaceup;

            subphase.DescriptionShort = HostUpgrade.UpgradeInfo.Name;
            subphase.DescriptionLong = "Choose any number of enemy ships beyond Range 1 - spend that many Force tokens to flip their dials faceup";
            subphase.ImageSource = HostUpgrade;

            subphase.Start();
        }

        private int GetAiPriority(GenericShip ship)
        {
            // Never use ability
            return 0;
        }

        private void FlipDialsFaceup(Action callback)
        {
            int forceToSpend = 0;
            foreach (GenericShip ship in Selection.MultiSelectedShips)
            {
                Roster.ToggleManeuverVisibility(ship, true);
                forceToSpend++;
                Messages.ShowInfo(string.Format("{0}: Dial of {1} is flipped faceup", HostUpgrade.UpgradeInfo.Name, ship.PilotInfo.PilotName));
            }
            HostShip.State.SpendForce(forceToSpend, callback);
        }

        private bool FilterSelection(GenericShip ship)
        {
            if (ship.Owner.PlayerNo == HostShip.Owner.PlayerNo) return false;

            DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
            if (distInfo.Range <= 1) return false;

            return true;
        }
    }
}