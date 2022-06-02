using BoardTools;
using Content;
using Ship;
using SubPhases;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ModifiedTIELnFighter
    {
        public class CaptainSeevor : ModifiedTIELnFighter
        {
            public CaptainSeevor() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Captain Seevor",
                    "Noisy Nuisance",
                    Faction.Scum,
                    3,
                    4,
                    10,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.CaptainSeevorAbility),
                    charges: 1,
                    regensCharges: 1,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Cannon,
                        UpgradeType.Missile,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/c9/f5/c9f5de03-e3e2-45b9-a86e-10ba74d1baed/swz23_a1_captain-seevor.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CaptainSeevorAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += CheckPilotAbility;
            HostShip.OnAttackStartAsDefender += CheckPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= CheckPilotAbility;
            HostShip.OnAttackStartAsDefender -= CheckPilotAbility;
        }

        protected virtual void CheckPilotAbility()
        {
            if (HostShip.State.Charges > 0 && Combat.AttackStep == CombatStep.Attack && Combat.Defender == HostShip && !Board.IsShipInArcByType(Combat.Attacker, Combat.Defender, Arcs.ArcType.Bullseye))
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskGiveJam);
            }
            else if (HostShip.State.Charges > 0 && Combat.AttackStep == CombatStep.Attack && Combat.Attacker == HostShip && !Board.IsShipInArcByType(Combat.Defender, Combat.Attacker, Arcs.ArcType.Bullseye))
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskGiveJam);
            }
        }

        protected void AskGiveJam(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                AlwaysUseByDefault,
                GiveJam,
                descriptionLong: "Do you want to spend 1 Charge? (If you do, the enemy ship gains 1 Jam Token)",
                imageHolder: HostShip
            );
        }

        protected void GiveJam(object sender, System.EventArgs e)
        {
            HostShip.State.Charges--;
            var targetShip = Combat.Attacker;
            if (Combat.Attacker == HostShip)
            {
                targetShip = Combat.Defender;
            }

            var jammingShip = HostShip;

            targetShip.Tokens.AssignToken(
                new Tokens.JamToken(targetShip, HostShip.Owner),
                DecisionSubPhase.ConfirmDecision
            );
        }
    }
}