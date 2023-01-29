using Abilities.SecondEdition;
using ActionsList;
using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class MoffGideon : TIELnFighter
        {
            public MoffGideon() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Moff Gideon",
                    "Ruthless Remnant Leader",
                    Faction.Imperial,
                    4,
                    3,
                    8,
                    isLimited: true,
                    charges: 2,
                    regensCharges: 1,
                    abilityType: typeof(MoffGideonAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Missile,
                        UpgradeType.Illicit,
                        UpgradeType.Illicit,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/moffgideon.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MoffGideonAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            GenericShip.OnAttackStartAsAttackerGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnAttackStartAsAttackerGlobal -= CheckAbility;
        }

        private void CheckAbility()
        {
            bool IsDifferentPlayer = (HostShip.Owner.PlayerNo != Combat.Defender.Owner.PlayerNo);
            DistanceInfo distanceInfo = new DistanceInfo(HostShip, Combat.Defender);

            if (IsDifferentPlayer
                && distanceInfo.Range <= 3
                && distanceInfo.Range >= 1
                && HostShip.State.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskChooseFriendlyShip);
            }
        }

        private void AskChooseFriendlyShip(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                PayForAbility,
                FilterTargets,
                GetFriendlyTargetPriority,
                HostShip.Owner.PlayerNo,
                name: HostShip.PilotInfo.PilotName,
                description: "You may spend a charge token and choose a friendly ship at range 0-1 of the defender to gain a strain token. If you do, defense dice cannot be modified. ",
                imageSource: HostShip,
                onSkip: delegate { Selection.ChangeActiveShip(Combat.Attacker); }
            );
        }

        private void PayForAbility()
        {
            HostShip.SpendCharge();
            TargetShip.Tokens.AssignToken(typeof(StrainToken), SelectShipSubPhase.FinishSelection);
            Selection.ChangeActiveShip(Combat.Attacker);
            Combat.Defender.OnTryAddAvailableDiceModification += PreventDiceModification;

        }

        private void PreventDiceModification(GenericShip ship, GenericAction action, ref bool canBeUsed)
        {
            if (Combat.AttackStep == CombatStep.Defence && Combat.Defender.ShipId == ship.ShipId)
            {
                ship.OnTryAddAvailableDiceModification -= PreventDiceModification;
                Messages.ShowInfo("Moff Gideon: The defender cannot use " + action.DiceModificationName);
                canBeUsed = false;
            }

        }

        private bool FilterTargets(GenericShip ship)
        {
            DistanceInfo distanceInfo = new DistanceInfo(ship, Combat.Defender);
            return distanceInfo.Range < 2 && Tools.IsSameTeam(ship, HostShip);
        }

        private int GetFriendlyTargetPriority(GenericShip ship)
        {
            return 100 - ship.PilotInfo.Cost;
        }
    }
}