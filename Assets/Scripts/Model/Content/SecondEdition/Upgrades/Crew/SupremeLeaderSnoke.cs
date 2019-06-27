﻿using Ship;
using Upgrade;
using System.Collections.Generic;
using UnityEngine;
using SubPhases;
using System;
using BoardTools;

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
                abilityType: typeof(Abilities.SecondEdition.SupremeLeaderSnokeAbility)
            );

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/5b699f8b5268e5290c42adce0fd2ee3e.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SupremeLeaderSnokeAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnSystemsAbilityActivation += CheckAbility;   
        }

        public override void DeactivateAbility()
        {
            HostShip.OnSystemsAbilityActivation -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (HostShip.State.Force > 0 && Board.GetShipsAtRange(HostShip, new Vector2(2, float.MaxValue), Team.Type.Enemy).Count > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnSystemsAbilityActivation, StartMultiSelectionSubphase);
            }
        }

        private void StartMultiSelectionSubphase(object sender, EventArgs e)
        {
            MultiSelectionSubphase subphase = Phases.StartTemporarySubPhaseNew<MultiSelectionSubphase>("Supreme Leader Snoke", Triggers.FinishTrigger);

            subphase.RequiredPlayer = HostShip.Owner.PlayerNo;

            subphase.Filter = FilterSelection;
            subphase.MaxToSelect = HostShip.State.Force;
            subphase.WhenDone = FlipDialsFaceup;

            subphase.AbilityName = HostUpgrade.UpgradeInfo.Name;
            subphase.Description = "Choose any number of enemy ships beyond Range 1 - spend that many Force tokens to flip their dials faceup";
            subphase.ImageSource = HostUpgrade;

            subphase.Start();
        }

        private void FlipDialsFaceup(Action callback)
        {
            foreach (GenericShip ship in Selection.MultiSelectedShips)
            {
                Roster.ToggleManeuverVisibility(ship, true);
                HostShip.State.Force--;
                Messages.ShowInfo(string.Format("{0}: Dial of {1} is flipped faceup", HostUpgrade.UpgradeInfo.Name, ship.PilotInfo.PilotName));
            }
            callback();
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