using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class SabineWrenGunner : GenericUpgrade
    {
        public SabineWrenGunner() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Sabine Wren",
                UpgradeType.Gunner,
                cost: 2,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.SabineWrenGunnerAbility),
                restriction: new FactionRestriction(Faction.Rebel)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/c5/8a/c58aba0b-0768-450b-afd9-c1e67ffab677/swz83_upgrade_sabinewrengunner.png";

            NameCanonical = "sabinewren-gunner";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SabineWrenGunnerAbility : GenericAbility
    {
        private GenericShip PreviousCurrentShip { get; set; }
        private int NumberOfShipsToUseAbility { get; set; }

        public override void ActivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker += CheckSabineWrenGunnerAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker -= CheckSabineWrenGunnerAbility;
        }

        private void CheckSabineWrenGunnerAbility(GenericShip ship)
        {
            if (Combat.ChosenWeapon.WeaponType != WeaponTypes.PrimaryWeapon && Combat.DamageCardsWereDealtToDefender > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, AskEachToRemoveToken);
            }
        }

        private void AskEachToRemoveToken(object sender, EventArgs e)
        {
            PreviousCurrentShip = Selection.ThisShip;
            NumberOfShipsToUseAbility = Combat.DamageCardsWereDealtToDefender;

            EachShipCanDoAction action = new EachShipCanDoAction
            (
                EachShipAction,
                onFinish: FinishAbility,
                conditions: new ConditionsBlock
                (
                    new TeamCondition(ShipTypes.Friendly),
                    new RangeToDefenderCondition(0, 3),
                    new HasTokenCondition(tokensList: new List<Type>() { typeof(Tokens.StrainToken), typeof(Tokens.StressToken) })
                ),
                description: new Abilities.Parameters.AbilityDescription
                (
                    HostUpgrade.UpgradeInfo.Name,
                    $"{NumberOfShipsToUseAbility} friendly ship at range 0-3 of the defender may remove 1 strain or stress token",
                    HostUpgrade
                )
            );

            action.DoAction(this);
        }

        private void EachShipAction(GenericShip ship, Action callback)
        {
            if (NumberOfShipsToUseAbility == 0)
            {
                Messages.ShowError("No more uses of this ability is left");
                callback();
            }
            else
            {
                Selection.ChangeActiveShip(ship);
                AskToSelectToken(callback);
            }
        }

        private void AskToSelectToken(Action callback)
        {
            SabineWrenGunnerDecisionSubphase subPhase = Phases.StartTemporarySubPhaseNew<SabineWrenGunnerDecisionSubphase>("Sabine Wren Gunner Decision Subphase", callback);

            subPhase.DescriptionShort = HostUpgrade.UpgradeInfo.Name;
            subPhase.DescriptionLong = "You may remove 1 strain or stress token from yourself";
            subPhase.ImageSource = HostUpgrade;

            if (Selection.ThisShip.Tokens.HasToken<StrainToken>()) subPhase.AddDecision("Strain Token", delegate { RemoveTokenByType(typeof(StrainToken), callback); });
            if (Selection.ThisShip.Tokens.HasToken<StressToken>()) subPhase.AddDecision("Stress Token", delegate { RemoveTokenByType(typeof(StressToken), callback); });

            subPhase.DecisionOwner = HostShip.Owner;
            subPhase.DefaultDecisionName = subPhase.GetDecisions().Last().Name;
            subPhase.ShowSkipButton = true;

            subPhase.Start();
        }

        private void RemoveTokenByType(Type type, Action callback)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            NumberOfShipsToUseAbility--;
            Selection.ThisShip.Tokens.RemoveToken(type, callback);
        }

        private class SabineWrenGunnerDecisionSubphase : DecisionSubPhase { }

        private void FinishAbility()
        {
            Selection.ChangeActiveShip(PreviousCurrentShip);
            Triggers.FinishTrigger();
        }
    }
}