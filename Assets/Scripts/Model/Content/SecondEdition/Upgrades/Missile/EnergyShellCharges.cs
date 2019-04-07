using ActionsList;
using Arcs;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class EnergyShellCharges : GenericSpecialWeapon
    {
        public EnergyShellCharges() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Energy-Shell Charges",
                UpgradeType.Missile,
                cost: 4,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 3,
                    minRange: 2,
                    maxRange: 3,
                    requiresToken: typeof(CalculateToken),
                    charges: 1,
                    arc: ArcType.Front
                ),
               restrictions: new UpgradeCardRestrictions(new ActionBarRestriction(typeof(CalculateAction)), new FactionRestriction(Faction.Separatists)),
               abilityType: typeof(Abilities.SecondEdition.EnergyShellChargesAbility)
            );
            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/4b6213e5ed13735bb381df08bdd1398d.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class EnergyShellChargesAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                "Spend Calculate for Eye to Crit",
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Change,
                1,
                payAbilityCost: PayAbilityCost,                
                sidesCanBeSelected: new List<DieSide> { DieSide.Focus},
                sideCanBeChangedTo: DieSide.Crit
            );
            HostShip.OnGenerateActions += AddEnergyShellChargesAction;
        }

        private bool IsAvailable()
        {
            return Combat.ChosenWeapon == HostUpgrade                
                && HostShip.Tokens.CountTokensByType<CalculateToken>() > 0
                && Combat.AttackStep == CombatStep.Attack
                && Combat.DiceRollAttack.Focuses > 0;
        }

        private int GetAiPriority()
        {
            return 42; // Just above Calculate's default priority
        }
                
        private void PayAbilityCost(Action<bool> callback)
        {
            if (HostShip.Tokens.HasToken<CalculateToken>())
            {
                HostShip.Tokens.SpendToken(
                    typeof(CalculateToken),
                    () => callback(true)
                );
            }
            else
            {
                callback(false);
            }
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
            HostShip.OnGenerateActions -= AddEnergyShellChargesAction;
        }

        protected void AddEnergyShellChargesAction(GenericShip ship)
        {
            if (HostUpgrade.State.Charges <= 0)
            {
                ship.AddAvailableAction(new ReloadEnergyShellChargesAction()
                {
                    ImageUrl = HostUpgrade.ImageUrl,
                    HostShip = HostShip,
                    Source = HostUpgrade,
                    Name = "Reload Energy-Shell Charges"
                });
            }
        }

        protected class ReloadEnergyShellChargesAction : ReloadAction
        {
            public override void ActionTake()
            {
                Source.State.RestoreCharge();
                Messages.ShowInfo($"Reload: One charge of \"{Source.UpgradeInfo.Name}\" is restored.  {HostShip.PilotInfo.PilotName} gains a Disarmed token.");
                AssignTokenAndFinish();
            }
        }

    }
}