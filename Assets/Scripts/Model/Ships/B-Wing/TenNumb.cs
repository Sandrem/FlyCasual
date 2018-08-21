using RuleSets;
using System;
using System.Collections.Generic;

namespace Ship
{
    namespace BWing
    {
        public class TenNumb : BWing, ISecondEditionPilot
        {
            public TenNumb() : base()
            {
                PilotName = "Ten Numb";
                PilotSkill = 8;
                Cost = 31;
                IsUnique = true;
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
 
                PilotAbilities.Add(new Abilities.TenNumbAbility());
                SkinName = "Blue";
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 4;
                Cost = 50;

                PilotAbilities.RemoveAll(ability => ability is Abilities.TenNumbAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.TenNumbAbility());
            }
        }
    }
}
 
namespace Abilities
{
    public class TenNumbAbility : GenericAbility
    {
        // When attacking or defending, if you have at least 1 stress token,
        // you may reroll 1 of your dice.
        public override void ActivateAbility()
        {
            HostShip.OnDefenceStartAsAttacker += RegisterTenNumbEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnDefenceStartAsAttacker -= RegisterTenNumbEffect;
        }

        private void RegisterTenNumbEffect()
        {
            foreach (Die die in Combat.DiceRollAttack.DiceList)
            {
                if (die.Side == DieSide.Crit)
                {
                    die.IsUncancelable = true;
                    return;
                }
            }
        }
    }

    namespace SecondEdition
    {
        //While you defend or perform an attack, you may spend 1 stress token to change all of your focus results to evade or hit results.
        public class TenNumbAbility : GenericAbility
        {
            public override void ActivateAbility()
            {
                AddDiceModification(
                    HostName,
                    IsDiceModificationAvailable,
                    GetDiceModificationAiPriority,
                    DiceModificationType.Change,
                    0,
                    new List<DieSide>() { DieSide.Focus },
                    DieSide.Success,
                    payAbilityCost: PayAbilityCost
                );
            }

            public override void DeactivateAbility()
            {
                RemoveDiceModification();
            }

            private bool IsDiceModificationAvailable()
            {
                return HostShip.IsStressed && (HostShip.IsAttacking || HostShip.IsDefending);
            }

            private int GetDiceModificationAiPriority()
            {
                var focusCount = HostShip.Tokens.CountTokensByType<Tokens.FocusToken>();

                switch (focusCount)
                {
                    case 0:
                        return 0;
                    case 1:
                        return 40;
                    default:
                        return 50;
                }
            }

            private void PayAbilityCost(Action<bool> callback)
            {
                if (HostShip.IsStressed)
                {
                    HostShip.Tokens.RemoveToken(typeof(Tokens.StressToken), () => callback(true));
                }
                else callback(false);
            }
        }
    }
}
