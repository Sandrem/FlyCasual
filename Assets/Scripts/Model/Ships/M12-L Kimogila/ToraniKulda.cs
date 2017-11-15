using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SubPhases;
using Ship;
using Tokens;

namespace Ship
{
    namespace M12LKimogila
    {
        public class ToraniKulda : M12LKimogila
        {
            public ToraniKulda() : base()
            {
                PilotName = "Torani Kulda";
                PilotSkill = 8;
                Cost = 27;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new PilotAbilitiesNamespace.ToraniKuldaAbility());
            }
        }
    }
}

namespace PilotAbilitiesNamespace
{
    public class ToraniKuldaAbility : GenericPilotAbility
    {
        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            Host.OnAttackFinish += RegisterPilotAbility;
        }

        private void RegisterPilotAbility(GenericShip ship)
        {
            if (Combat.Attacker.ShipId == Host.ShipId)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, TryRegisterAbility);
            }
        }

        private void TryRegisterAbility(object sender, System.EventArgs e)
        {
            Players.GenericPlayer opponent = Roster.GetPlayer(Roster.AnotherPlayer(Host.Owner.PlayerNo));
            foreach (var ship in opponent.Ships)
            {
                Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(Host, ship.Value, Host.PrimaryWeapon);
                if (shotInfo.InBullseyeArc)
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

        private void ShowChooseEffect(object sender, System.EventArgs e)
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

            DefaultDecision = "Remove all Focus and Evade tokens";

            callBack();
        }

        private void DiscardFocusAndEvadeTokens(object sender, System.EventArgs e)
        {
            Selection.ThisShip.RemoveToken(typeof(FocusToken), '*', true);
            Selection.ThisShip.RemoveToken(typeof(EvadeToken), '*', true);

            ConfirmDecision();
        }

        private void SufferDamage(object sender, System.EventArgs e)
        {
            DiceRoll damage = new DiceRoll(DiceKind.Attack, 0, DiceRollCheckType.Virtual);
            damage.AddDice(DieSide.Success);

            Selection.ThisShip.AssignedDamageDiceroll.DiceList.AddRange(damage.DiceList);

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Suffer Torani Kulda's ability damage",
                TriggerType = TriggerTypes.OnDamageIsDealt,
                TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                EventHandler = Selection.ThisShip.SufferDamage,
                Skippable = true,
                EventArgs = new DamageSourceEventArgs()
                {
                    Source = "Torani Kulda",
                    DamageType = DamageTypes.CardAbility
                }
            });

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, ConfirmDecision);
        }
    }
}
