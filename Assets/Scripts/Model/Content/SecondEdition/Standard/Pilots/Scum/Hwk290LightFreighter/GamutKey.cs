using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using UnityEngine;
using Upgrade;

namespace Ship.SecondEdition.Hwk290LightFreighter
{
    public class GamutKey : Hwk290LightFreighter
    {
        public GamutKey() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Gamut Key",
                "Collaborationist Governor",
                Faction.Scum,
                3,
                3,
                8,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.GamutKeyPilotAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Crew,
                    UpgradeType.Device,
                    UpgradeType.Illicit,
                    UpgradeType.Modification,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.Freighter
                },
                charges: 2,
                regensCharges: 1
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/12/d5/12d5d473-b2ec-419b-a5ec-eb57060684bb/swz85_pilot_gamutkey.png";

            ModelInfo.SkinName = "Black";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class GamutKeyPilotAbility : GenericAbility
    {
        GenericShip ShipThatKeepTokens { get; set; }
        private bool IsDelayed = false;

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
            if (HasEnoughCharges())
            {
                RegisterAbilityTrigger(TriggerTypes.OnEndPhaseStart, AskToSelectShip);
            }
        }

        protected virtual bool HasEnoughCharges()
        {
            return HostShip.State.Charges >= 2;
        }

        private void AskToSelectShip(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                LeaveTokens,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                name: GetAbilityName(),
                description: "Choose a ship - during the End Phase circular tokens are not removed from it",
                imageSource: GetAbilityImage()
            );
        }

        protected virtual string GetAbilityName()
        {
            return HostShip.PilotInfo.PilotName;
        }

        protected virtual IImageHolder GetAbilityImage()
        {
            return HostShip;
        }

        private void LeaveTokens()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            SpendChargesForAbility();
            Messages.ShowInfo($"{GetAbilityName()}: {TargetShip.PilotInfo.PilotName} doesn't remove cirular tokens during this End Phase");
            ShipThatKeepTokens = TargetShip;

            IsDelayed = true;
            ShipThatKeepTokens.BeforeRemovingTokenInEndPhase += DontRemoveCircularTokens;
            ShipThatKeepTokens.OnRoundEnd += UnsubscribeGainedAbility;

            Triggers.FinishTrigger();
        }

        protected virtual void SpendChargesForAbility()
        {
            HostShip.SpendCharges(2);
        }

        private void DontRemoveCircularTokens(GenericShip ship, GenericToken token, ref bool willBeRemoved)
        {
            if (token.TokenShape == TokenShapes.Cirular) willBeRemoved = false;
        }

        private void UnsubscribeGainedAbility(GenericShip ship)
        {
            //Don't remove ability in the same turn
            if (!IsDelayed)
            {
                ShipThatKeepTokens.BeforeRemovingTokenInEndPhase -= DontRemoveCircularTokens;
                ShipThatKeepTokens.OnRoundEnd -= UnsubscribeGainedAbility;

                ShipThatKeepTokens = null;
            }
            else
            {
                IsDelayed = false;
            }
        }

        protected virtual bool FilterTargets(GenericShip ship)
        {
            ShotInfo shotInfo = new ShotInfo(HostShip, ship, HostShip.PrimaryWeapons);
            return ship.ShipId == HostShip.ShipId
                || shotInfo.InArcByType(Arcs.ArcType.SingleTurret)
                && ship.Tokens.CountTokensByShape(TokenShapes.Cirular) > 0;
        }

        private int GetAiPriority(GenericShip ship)
        {
            int result = 0;

            if (Tools.IsSameTeam(HostShip, ship))
            {
                result += ship.PilotInfo.Cost + (ship.Tokens.CountTokensByShape(TokenShapes.Cirular) * 100);
            }

            return result;
        }
    }
}
