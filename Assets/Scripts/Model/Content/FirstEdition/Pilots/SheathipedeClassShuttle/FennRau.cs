using System.Collections;
using System.Collections.Generic;
using Ship;
using Conditions;
using Tokens;
using ActionsList;
using SubPhases;
using BoardTools;

namespace Ship
{
    namespace FirstEdition.SheathipedeClassShuttle
    {
        public class FennRau : SheathipedeClassShuttle
        {
            public FennRau() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Fenn Rau",
                    9,
                    20,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.FennRauRebelAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class FennRauRebelAbility : GenericAbility
    {
        private GenericShip affectedShip;

        public override void ActivateAbility()
        {
            GenericShip.OnCombatActivationGlobal += CheckAbility;
            Phases.Events.OnRoundEnd += RemoveFennRauPilotAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnCombatActivationGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericShip activatedShip)
        {
            if (activatedShip.Owner.PlayerNo == HostShip.Owner.PlayerNo) return;

            if (HostShip.Tokens.HasToken(typeof(StressToken))) return;

            ShotInfo shotInfo = new ShotInfo(HostShip, activatedShip, HostShip.PrimaryWeapon);
            if (!shotInfo.InArc || shotInfo.Range > 3) return;

            RegisterAbilityTrigger(TriggerTypes.OnCombatActivation, AskAbility);
        }

        private void AskAbility(object sender, System.EventArgs e)
        {
            Messages.ShowInfo("Fenn Rau can use his ability");
            AskToUseAbility(AlwaysUseByDefault, UseAbility, DontUseAbility);
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            if (!HostShip.Tokens.HasToken(typeof(StressToken)))
            {
                Messages.ShowInfoToHuman("Fenn Rau: Ability was used");

                HostShip.Tokens.AssignToken(typeof(StressToken), AssignConditionToActivatedShip);
            }
            else
            {
                Messages.ShowErrorToHuman("Fenn Rau: Cannot use ability - already has stress");
                Triggers.FinishTrigger();
            }
        }

        private void DontUseAbility(object sender, System.EventArgs e)
        {
            Messages.ShowInfoToHuman("Fenn Rau: Ability was not used");
            DecisionSubPhase.ConfirmDecision();
        }

        private void AssignConditionToActivatedShip()
        {
            affectedShip = Selection.ThisShip;
            affectedShip.OnTryAddAvailableDiceModification += UseFennRauRestriction;

            FennRauRebelCondition conditionToken = new FennRauRebelCondition(HostShip);
            conditionToken.TooltipType = HostShip.GetType();
            affectedShip.Tokens.AssignCondition(conditionToken);

            DecisionSubPhase.ConfirmDecision();
        }

        private void UseFennRauRestriction(GenericShip ship, GenericAction action, ref bool canBeUsed)
        {
            if (Combat.Attacker == affectedShip && action.DiceModificationTiming != DiceModificationTimingType.Opposite && action.TokensSpend.Count > 0)
            {
                Messages.ShowErrorToHuman("Fenn Rau: Cannot use dice modification\n" + action.Name);
                canBeUsed = false;
            }
        }

        private void RemoveFennRauPilotAbility()
        {
            if (affectedShip != null)
            {
                affectedShip.OnTryAddAvailableDiceModification -= UseFennRauRestriction;
                affectedShip.Tokens.RemoveCondition(typeof(FennRauRebelCondition));
                affectedShip = null;
            }
        }
    }
}

namespace Conditions
{
    public class FennRauRebelCondition : GenericToken
    {
        public FennRauRebelCondition(GenericShip host) : base(host)
        {
            Name = "Debuff Token";
            Temporary = false;
        }
    }
}
