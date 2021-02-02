using Ship;
using Upgrade;
using System;
using BoardTools;
using SubPhases;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class Dengar : GenericUpgrade
    {
        public Dengar() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Dengar",
                UpgradeType.Gunner,
                cost: 6,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Scum),
                abilityType: typeof(Abilities.SecondEdition.DengarGunnerAbility),
                charges: 1,
                regensCharges: true,
                seImageNumber: 141
            );

            Avatar = new AvatarInfo(
                Faction.Scum,
                new Vector2(341, 1),
                new Vector2(150, 150)
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class DengarGunnerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackFinishAsDefender += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsDefender -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            ShotInfo shotInfo = new ShotInfo(HostShip, Combat.Attacker, HostShip.PrimaryWeapons);
            if (shotInfo.InArc && HostUpgrade.State.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, AskToUseOwnAbility);
            }
        }

        private void AskToUseOwnAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                AlwaysUseByDefault,
                UseOwnAbility,
                descriptionLong: "Do you want to spend 1 Charge? (If you do, roll 1 attack die unless the attacker chooses to remove 1 green token. On a \"hit\" or \"crit\" result, the attacker suffers 1 Damage.)",
                imageHolder: HostUpgrade
            );
        }

        private void UseOwnAbility(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostUpgrade.State.SpendCharge();

            DengarAbilityDecisionSubPhase subphase = Phases.StartTemporarySubPhaseNew<DengarAbilityDecisionSubPhase>(
                "Select effect of Dengar's ability",
                Triggers.FinishTrigger
            );
            subphase.HostUpgrade = HostUpgrade;
            subphase.Start();
        }
    }
}

namespace SubPhases
{
    public class DengarCheckSubPhase : DiceRollCheckSubPhase
    {
        public GenericUpgrade HostUpgrade;

        public override void Prepare()
        {
            DiceKind = DiceKind.Attack;
            DiceCount = 1;

            AfterRoll = FinishAction;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();

            if (CurrentDiceRoll.DiceList[0].Side == DieSide.Success || CurrentDiceRoll.DiceList[0].Side == DieSide.Crit)
            {
                Messages.ShowInfo("Dengar: Attacker suffered 1 damage");

                Combat.Attacker.Damage.TryResolveDamage(
                    1,
                    new DamageSourceEventArgs() {
                        DamageType = DamageTypes.CardAbility,
                        Source = HostUpgrade
                    },
                    CallBack
                );
            }
            else
            {
                CallBack();
            }
        }
    }
}

namespace SubPhases
{
    public class DengarAbilityDecisionSubPhase : RemoveGreenTokenDecisionSubPhase
    {
        public GenericUpgrade HostUpgrade;

        public override void PrepareCustomDecisions()
        {
            DescriptionShort = "Dengar";
            DescriptionLong = "Dengar rolls 1 attack die to deal damage unless you choose to remove 1 green token.";
            DecisionOwner = Combat.Attacker.Owner;

            AddDecision("Allow to roll a die", StartDiceCheck);
        }

        private void StartDiceCheck(object sender, EventArgs e)
        {
            ConfirmDecisionNoCallback();

            DengarCheckSubPhase subphase = Phases.StartTemporarySubPhaseNew<DengarCheckSubPhase>(
                "Dengar: Check damage",
                delegate {
                    Phases.FinishSubPhase(typeof(DengarCheckSubPhase));
                    Triggers.FinishTrigger();
                }
            );
            subphase.HostUpgrade = HostUpgrade;
            subphase.Start();
        }
    }
}