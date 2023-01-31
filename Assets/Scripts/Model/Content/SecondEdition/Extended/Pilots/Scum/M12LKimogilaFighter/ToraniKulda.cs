using Arcs;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.M12LKimogilaFighter
    {
        public class ToraniKulda : M12LKimogilaFighter
        {
            public ToraniKulda() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Torani Kulda",
                    "Rodian Freelancer",
                    Faction.Scum,
                    4,
                    5,
                    10,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ToraniKuldaAbility),
                    tags: new List<Tags>
                    {
                        Tags.BountyHunter
                    },
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Torpedo,
                        UpgradeType.Missile,
                        UpgradeType.Astromech,
                        UpgradeType.Illicit,
                        UpgradeType.Modification
                    },
                    seImageNumber: 207,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );

                ModelInfo.SkinName = "Cartel Executioner";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ToraniKuldaAbility : Abilities.FirstEdition.ToraniKuldaAbility
    {
        protected override void ShowChooseEffect(object sender, System.EventArgs e)
        {
            Selection.ThisShip = (GenericShip)sender;

            ToraniKuldaAbilityDecisionSubPhaseSE subphase = Phases.StartTemporarySubPhaseNew<ToraniKuldaAbilityDecisionSubPhaseSE>(
                "Select effect of " + HostShip.PilotInfo.PilotName + "'s ability",
                delegate {
                    Selection.ThisShip = HostShip;
                    Triggers.FinishTrigger();
                }
            );

            subphase.DescriptionShort = HostShip.PilotInfo.PilotName;
            subphase.DescriptionLong = Selection.ThisShip.ShipId + ": " + "Select effect of " + HostShip.PilotInfo.PilotName + "'s ability";
            subphase.ImageSource = HostShip;

            subphase.Start();
        }
    }
}

namespace SubPhases
{
    public class ToraniKuldaAbilityDecisionSubPhaseSE : RemoveGreenTokenDecisionSubPhase
    {
        public override void PrepareCustomDecisions()
        {
            DecisionOwner = Selection.ThisShip.Owner;

            AddDecision("Suffer 1 damage.", SufferDamage);
        }

        private void SufferDamage(object sender, System.EventArgs e)
        {
            Messages.ShowInfo("Torani Kulda's ability: " + Selection.ThisShip.PilotInfo.PilotName + " decided to suffer 1 damage");

            DamageSourceEventArgs toranikuldaDamage = new DamageSourceEventArgs()
            {
                Source = "Torani Kulda",
                DamageType = DamageTypes.CardAbility
            };

            Selection.ThisShip.Damage.TryResolveDamage(1, toranikuldaDamage, DecisionSubPhase.ConfirmDecision);
        }
    }
}

namespace Abilities.FirstEdition
{
    public class ToraniKuldaAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackFinish += RegisterPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinish -= RegisterPilotAbility;
        }

        private void RegisterPilotAbility(GenericShip ship)
        {
            if (Combat.Attacker.ShipId == HostShip.ShipId)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, TryRegisterAbility);
            }
        }

        private void TryRegisterAbility(object sender, System.EventArgs e)
        {
            Players.GenericPlayer opponent = Roster.GetPlayer(Roster.AnotherPlayer(HostShip.Owner.PlayerNo));
            foreach (GenericShip ship in opponent.Ships.Values)
            {
                if (HostShip.SectorsInfo.IsShipInSector(ship, ArcType.Bullseye))
                {
                    Triggers.RegisterTrigger(new Trigger()
                    {
                        Name = ship.ShipId + ": " + Name,
                        TriggerType = TriggerTypes.OnAbilityDirect,
                        TriggerOwner = opponent.PlayerNo,
                        EventHandler = ShowChooseEffect,
                        Sender = ship
                    });
                }
            }

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, Triggers.FinishTrigger);
        }

        protected virtual void ShowChooseEffect(object sender, System.EventArgs e)
        {
            Selection.ThisShip = (GenericShip)sender;

            ToraniKuldaAbilityDecisionSubPhase subphase = Phases.StartTemporarySubPhaseNew<ToraniKuldaAbilityDecisionSubPhase>(
                "Select effect of " + HostShip.PilotInfo.PilotName + "'s ability",
                delegate {
                    Selection.ThisShip = HostShip;
                    Triggers.FinishTrigger();
                }
            );

            subphase.DescriptionShort = HostShip.PilotInfo.PilotName;
            subphase.DescriptionLong = Selection.ThisShip.ShipId + ": " + "Select effect of " + HostShip.PilotInfo.PilotName + "'s ability";
            subphase.ImageSource = HostShip;

            subphase.Start();
        }
    }
}

namespace SubPhases
{
    public class ToraniKuldaAbilityDecisionSubPhase : DecisionSubPhase
    {
        public override void PrepareDecision(Action callBack)
        {
            DecisionOwner = Selection.ThisShip.Owner;

            AddDecision("Suffer 1 damage", SufferDamage);
            AddDecision("Remove all Focus and Evade tokens", DiscardFocusAndEvadeTokens);

            DefaultDecisionName = "Remove all Focus and Evade tokens";

            callBack();
        }

        private void DiscardFocusAndEvadeTokens(object sender, System.EventArgs e)
        {
            DiscardAllFocusTokens();
        }

        private void DiscardAllFocusTokens()
        {
            Selection.ThisShip.Tokens.RemoveAllTokensByType(
                typeof(FocusToken),
                DiscardAllEvadeTokens
            );
        }

        private void DiscardAllEvadeTokens()
        {
            Selection.ThisShip.Tokens.RemoveAllTokensByType(
                typeof(EvadeToken),
                ConfirmDecision
            );
        }

        private void SufferDamage(object sender, System.EventArgs e)
        {
            DamageSourceEventArgs toranikuldaDamage = new DamageSourceEventArgs()
            {
                Source = "Torani Kulda",
                DamageType = DamageTypes.CardAbility
            };

            Selection.ThisShip.Damage.TryResolveDamage(1, toranikuldaDamage, ConfirmDecision);
        }
    }
}