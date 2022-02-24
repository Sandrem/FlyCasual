using Ship;
using Upgrade;
using System.Linq;
using System.Collections.Generic;
using ActionsList;
using Actions;
using System;
using BoardTools;
using SubPhases;
using Tokens;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class Kraken : GenericUpgrade
    {
        public Kraken() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Kraken",
                UpgradeType.TacticalRelay,
                cost: 10,
                isLimited: true,
                isSolitary: true,
                addAction: new ActionInfo(typeof(CalculateAction), ActionColor.White, this),
                restriction: new FactionRestriction(Faction.Separatists),
                abilityType: typeof(Abilities.SecondEdition.KrakenAbility)
            );

            Avatar = new AvatarInfo(
                Faction.Separatists,
                new Vector2(209, 1)
            );

            ImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/7/77/Swz29_kraken.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class KrakenAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers -= RegisterAbility;
        }

        private void RegisterAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnEndPhaseStart, StartMultiselect);
        }

        private void StartMultiselect(object sender, System.EventArgs e)
        {
            MultiSelectionSubphase subphase = Phases.StartTemporarySubPhaseNew<MultiSelectionSubphase>("Kraken", Triggers.FinishTrigger);

            subphase.RequiredPlayer = HostShip.Owner.PlayerNo;

            subphase.Filter = FilterSelection;
            subphase.GetAiPriority = GetAiPriority;
            subphase.MaxToSelect = 3;
            subphase.WhenDone = LeaveCalculateTokens;

            subphase.DescriptionShort = HostUpgrade.UpgradeInfo.Name;
            subphase.DescriptionLong = "You may choose up to 3 ships. If you do, these ships do not remove 1 calculate token.";
            subphase.ImageSource = HostUpgrade;

            subphase.Start();
        }

        private int GetAiPriority(GenericShip ship)
        {
            return ship.PilotInfo.Cost;
        }

        private bool FilterSelection(GenericShip ship)
        {
            if (ship.Owner.PlayerNo != HostShip.Owner.PlayerNo) return false;

            if (!ship.Tokens.HasToken<CalculateToken>()) return false;

            DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
            if (distInfo.Range > 3) return false;

            return true;
        }

        private void LeaveCalculateTokens(Action callback)
        {
            foreach (GenericShip ship in Selection.MultiSelectedShips)
            {
                ship.BeforeRemovingTokenInEndPhase += KeepOneCalculateToken;
            }
            callback();
        }

        private void KeepOneCalculateToken(GenericShip ship, GenericToken token, ref bool remove)
        {
            if (token is CalculateToken)
            {
                remove = false;
                ship.BeforeRemovingTokenInEndPhase -= KeepOneCalculateToken;
            }
        }
    }
}