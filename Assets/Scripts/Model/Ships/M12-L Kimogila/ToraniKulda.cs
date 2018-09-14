﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SubPhases;
using Ship;
using Tokens;
using Abilities;
using Arcs;
using BoardTools;
using RuleSets;

namespace Ship
{
    namespace M12LKimogila
    {
        public class ToraniKulda : M12LKimogila, ISecondEditionPilot
        {
            public ToraniKulda() : base()
            {
                PilotName = "Torani Kulda";
                PilotSkill = 8;
                Cost = 27;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.ToraniKuldaAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 4;
                Cost = 50;

                PilotAbilities.RemoveAll(ability => ability is Abilities.ToraniKuldaAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.ToraniKuldaAbilitySE());

                SEImageNumber = 207;
            }
        }
    }
}

namespace Abilities
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
            foreach (var ship in opponent.Ships)
            {
                ShotInfo shotInfo = new ShotInfo(HostShip, ship.Value, HostShip.PrimaryWeapon);
                if (shotInfo.InArcByType(ArcTypes.Bullseye))
                {
                    Triggers.RegisterTrigger(new Trigger() {
                        Name = ship.Value.ShipId + ": " + Name,
                        TriggerType = TriggerTypes.OnAbilityDirect,
                        TriggerOwner = opponent.PlayerNo,
                        EventHandler = ShowChooseEffect,
                        Sender = ship.Value
                    });
                }
            }

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, Triggers.FinishTrigger);
        }

        protected virtual void ShowChooseEffect(object sender, System.EventArgs e)
        {
            Selection.ThisShip = (GenericShip) sender;

            Phases.StartTemporarySubPhaseOld(
                "Select effect of Torani Kulda's ability",
                typeof(ToraniKuldaAbilityDecisionSubPhase),
                Triggers.FinishTrigger
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ToraniKuldaAbilitySE : ToraniKuldaAbility
    {
        protected override void ShowChooseEffect(object sender, System.EventArgs e)
        {
            Selection.ThisShip = (GenericShip)sender;

            Phases.StartTemporarySubPhaseOld(
                "Select effect of Torani Kulda's ability",
                typeof(ToraniKuldaAbilityDecisionSubPhaseSE),
                Triggers.FinishTrigger
            );
        }
    }
}

namespace SubPhases
{
    public class ToraniKuldaAbilityDecisionSubPhase: DecisionSubPhase
    {
        public override void PrepareDecision(Action callBack)
        {
            InfoText = Selection.ThisShip.ShipId + ": " + "Select effect of Torani Kulda's ability";
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

    public class ToraniKuldaAbilityDecisionSubPhaseSE : RemoveGreenTokenDecisionSubPhase
    {
        public override void PrepareCustomDecisions()
        {
            InfoText = Selection.ThisShip.ShipId + ": " + "Select effect of Torani Kulda's ability";
            DecisionOwner = Selection.ThisShip.Owner;

            AddDecision("Suffer 1 damage.", SufferDamage);
        }

        private void SufferDamage(object sender, System.EventArgs e)
        {
            DamageSourceEventArgs toranikuldaDamage = new DamageSourceEventArgs()
            {
                Source = "Torani Kulda",
                DamageType = DamageTypes.CardAbility
            };

            Selection.ThisShip.Damage.TryResolveDamage(1, toranikuldaDamage, DecisionSubPhase.ConfirmDecision);
        }
    }
}
