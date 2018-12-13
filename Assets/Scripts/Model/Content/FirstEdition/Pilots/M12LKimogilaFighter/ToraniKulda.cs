﻿using Arcs;
using BoardTools;
using Ship;
using SubPhases;
using System;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.M12LKimogilaFighter
    {
        public class ToraniKulda : M12LKimogilaFighter
        {
            public ToraniKulda() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Torani Kulda",
                    8,
                    27,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.ToraniKuldaAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ModelInfo.SkinName = "Cartel Executioner";
            }
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
            foreach (var ship in opponent.Ships)
            {
                ShotInfo shotInfo = new ShotInfo(HostShip, ship.Value, HostShip.PrimaryWeapons);
                if (shotInfo.InArcByType(ArcType.Bullseye))
                {
                    Triggers.RegisterTrigger(new Trigger()
                    {
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
            Selection.ThisShip = (GenericShip)sender;

            Phases.StartTemporarySubPhaseOld(
                "Select effect of Torani Kulda's ability",
                typeof(ToraniKuldaAbilityDecisionSubPhase),
                Triggers.FinishTrigger
            );
        }
    }
}

namespace SubPhases
{
    public class ToraniKuldaAbilityDecisionSubPhase : DecisionSubPhase
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
}

