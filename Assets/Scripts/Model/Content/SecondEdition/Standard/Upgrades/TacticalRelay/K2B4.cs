using Ship;
using Upgrade;
using System.Linq;
using System.Collections.Generic;
using ActionsList;
using Actions;
using System;
using BoardTools;
using SubPhases;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class K2B4 : GenericUpgrade
    {
        public K2B4() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "K2-B4",
                UpgradeType.TacticalRelay,
                cost: 6,
                isLimited: true,
                isSolitary: true,
                restriction: new FactionRestriction(Faction.Separatists),
                abilityType: typeof(Abilities.SecondEdition.K2B4Ability)
            );

            Avatar = new AvatarInfo(
                Faction.Separatists,
                new Vector2(256, 6),
                new Vector2(125, 125)
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class K2B4Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal += CheckRelayAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal -= CheckRelayAbility;
        }

        private void CheckRelayAbility(GenericShip ship)
        {
            if (ship.Owner.PlayerNo == HostShip.Owner.PlayerNo && ship.Tokens.HasToken(typeof(Tokens.CalculateToken)))
            {
                DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
                if (distInfo.Range < 4)
                {
                    ship.AddAvailableDiceModification(
                        new K2B4DiceModification() {
                            ImageUrl = HostUpgrade.ImageUrl,
                            UpgradeSource = HostUpgrade
                        },
                        HostShip
                    );
                }
            }
        }

        private class K2B4DiceModification : GenericAction
        {
            public GenericUpgrade UpgradeSource;

            public K2B4DiceModification()
            {
                Name = DiceModificationName = "K2-B4";
                TokensSpend = new List<Type>() { typeof(Tokens.CalculateToken) };
            }

            public override void ActionEffect(Action callBack)
            {
                Combat.Defender.Tokens.SpendToken(
                    typeof(Tokens.CalculateToken),
                    delegate { AskAttackerToGainStrainToken(callBack); }
                );
            }

            private void AskAttackerToGainStrainToken(Action callBack)
            {
                K2B4DecisionSubphase subphase = Phases.StartTemporarySubPhaseNew<K2B4DecisionSubphase>(
                    "K2-B4 decision",
                    callBack
                );

                subphase.DecisionOwner = Combat.Attacker.Owner;

                subphase.DescriptionShort = "K2-B4";
                subphase.DescriptionLong = "Do you want to gain 1 Strain Token to prevent adding of 1 Evade result by defender?";
                subphase.ImageSource = UpgradeSource;

                subphase.ShowSkipButton = false;

                subphase.AddDecision("Yes", GainStrainToken);
                subphase.AddDecision("No", AllowToAddEvade);

                subphase.DefaultDecisionName = "No";

                subphase.Start();
            }

            private void GainStrainToken(object sender, EventArgs e)
            {
                Messages.ShowInfo("K2-B4: Attacker decided to gain Strain Token");

                Combat.Attacker.Tokens.AssignToken(
                    typeof(Tokens.StrainToken),
                    DecisionSubPhase.ConfirmDecision
                );
            }

            private void AllowToAddEvade(object sender, EventArgs e)
            {
                Messages.ShowInfo("K2-B4: Attacker decided to allow the defender to add Evade result");
                Combat.DiceRollDefence.AddDiceAndShow(DieSide.Success);
                DecisionSubPhase.ConfirmDecision();
            }

            public override int GetActionPriority()
            {
                return (Combat.DiceRollDefence.Focuses == 0) ? 89 : 0;
            }

            public override bool IsDiceModificationAvailable()
            {
                return Combat.AttackStep == CombatStep.Defence
                    && HostShip.Tokens.HasToken(typeof(Tokens.CalculateToken));
            }

            private class K2B4DecisionSubphase : DecisionSubPhase { }
        }
    }
}