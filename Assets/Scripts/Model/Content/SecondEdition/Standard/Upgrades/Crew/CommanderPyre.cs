using Ship;
using Upgrade;
using System.Linq;
using Tokens;
using SubPhases;
using System;
using UnityEngine;
using Content;
using System.Collections.Generic;

namespace UpgradesList.SecondEdition
{
    public class CommanderPyre : GenericUpgrade
    {
        public CommanderPyre() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Commander Pyre",
                UpgradeType.Crew,
                cost: 5,
                isLimited: true,
                restriction: new FactionRestriction(Faction.FirstOrder),
                abilityType: typeof(Abilities.SecondEdition.CommanderPyreAbility),
                legalityInfo: new List<Legality> { Legality.StandartBanned }
            );

            Avatar = new AvatarInfo(
                Faction.FirstOrder,
                new Vector2(259, 1),
                new Vector2(75, 75)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/10/5d/105d386c-ff1b-44be-9b9c-a2c1dc2877ec/swz69_pyre_card.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    //Setup: After placing forces, choose an enemy ship. It gains 2 stress tokens.
    //While you defend, if the attacker is stressed, you may reroll 1 defense die.
    public class CommanderPyreAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnSetupEnd += RegisterAbility;
            AddDiceModification(
                HostName,
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Reroll,
                1
                );
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnSetupEnd -= RegisterAbility;
            RemoveDiceModification();
        }

        private int GetAiPriority()
        {
            return 90;
        }

        private bool IsAvailable()
        {
            return Combat.Defender == HostShip
                && Combat.AttackStep == CombatStep.Defence
                && Combat.Attacker.IsStressed;
        }

        protected void RegisterAbility()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = HostShip.PilotInfo.PilotName,
                TriggerType = TriggerTypes.OnSetupEnd,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = SelectTarget
            });
        }


        private void SelectTarget(object Sender, System.EventArgs e)
        {
            SelectTargetForAbility(
                  AssignStress,
                  CheckRequirements,
                  GetAiPriority,
                  HostShip.Owner.PlayerNo,
                  HostName,
                  "Assign 2 stress tokens to 1 enemy ship",
                  HostUpgrade,
                  showSkipButton: false
            );
        }

        protected virtual void AssignStress()
        {
            TargetShip.Tokens.AssignTokens(() => new StressToken(TargetShip), 2, SelectShipSubPhase.FinishSelection);
        }

        protected virtual bool CheckRequirements(GenericShip ship)
        {
            return ship.Owner != HostShip.Owner;
        }

        private int GetAiPriority(GenericShip ship)
        {
            return ship.PilotInfo.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.UpgradeInfo.Cost);
        }
    }
}