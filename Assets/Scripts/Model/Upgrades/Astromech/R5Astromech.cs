using System.Linq;
using System.Collections.Generic;
using Upgrade;
using Abilities;
using RuleSets;
using ActionsList;
using System;

namespace UpgradesList
{

    public class R5Astromech : GenericUpgrade, ISecondEditionUpgrade
    {
        public R5Astromech() : base()
        {
            Types.Add(UpgradeType.Astromech);
            Name = "R5 Astromech";
            Cost = 1;

            UpgradeAbilities.Add(new R5AstromechAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            MaxCharges = 2;

            ImageUrl = "https://i.imgur.com/B7zcHyk.png";

            UpgradeAbilities.RemoveAll(a => a is R5AstromechAbility);
            UpgradeAbilities.Add(new Abilities.SecondEdition.R5AstromechAbility());
        }
    }

}

namespace Abilities
{
    public class R5AstromechAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.OnEndPhaseStart_Triggers += RegisterR5AstromechAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.OnEndPhaseStart_Triggers -= RegisterR5AstromechAbility;
        }

        private void RegisterR5AstromechAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnEndPhaseStart, R5AstromechAbilityEffect);
        }

        private void R5AstromechAbilityEffect(object sender, System.EventArgs e)
        {
            Selection.ActiveShip = HostShip;

            List<GenericDamageCard> shipCritsList = HostShip.Damage.GetFaceupCrits();

            if (shipCritsList.Count == 1)
            {
                Selection.ActiveShip.Damage.FlipFaceupCritFacedown(shipCritsList.First());
                Sounds.PlayShipSound("R2D2-Proud");
                Triggers.FinishTrigger();
            }
            else if (shipCritsList.Count > 1)
            {
                Phases.StartTemporarySubPhaseOld(
                    "R5 Astromech: Select faceup ship Crit",
                    typeof(SubPhases.R5AstromechDecisionSubPhase),
                    Triggers.FinishTrigger
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}

namespace SubPhases
{

    public class R5AstromechDecisionSubPhase : DecisionSubPhase
    {

        public override void PrepareDecision(System.Action callBack)
        {
            InfoText = "R5 Astromech: Select faceup ship Crit";

            foreach (var shipCrit in Selection.ActiveShip.Damage.GetFaceupCrits().Where(n => n.Type == CriticalCardType.Ship).ToList())
            {
                //TODO: what if two same crits?
                AddDecision(shipCrit.Name, delegate { DiscardCrit(shipCrit); });
            }

            DefaultDecisionName = GetDecisions().First().Name;

            callBack();
        }

        private void DiscardCrit(GenericDamageCard critCard)
        {
            Selection.ActiveShip.Damage.FlipFaceupCritFacedown(critCard);
            Sounds.PlayShipSound("R2D2-Proud");
            ConfirmDecision();
        }

    }

}


namespace Abilities.SecondEdition
{
    //Action: Spend 1 charge to repair 1 facedown damage card.
    //Action: Repair 1 faceup Ship damage card.
    public class R5AstromechAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGenerateAvailableActionsList += AddAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionsList -= AddAction;
        }

        private void AddAction(Ship.GenericShip ship)
        {
            if (ship.Damage.GetFacedownCards().Any() && HostUpgrade.Charges > 0)
            {
                ship.AddAvailableAction(new RepairAction(RepairAction.CardFace.FaceDown)
                {
                    ImageUrl = HostUpgrade.ImageUrl,
                    Host = HostShip,
                    PayRepairCost = () =>
                    {
                        var result = false;
                        if (HostUpgrade.Charges > 0) HostUpgrade.SpendCharge(() => result = true);
                        return result;
                    }
                });
            }
            if (ship.Damage.GetFaceupCrits(CriticalCardType.Ship).Any())
            {
                ship.AddAvailableAction(new RepairAction(RepairAction.CardFace.FaceUp, CriticalCardType.Ship)
                {
                    ImageUrl = HostUpgrade.ImageUrl,
                    Host = HostShip
                });
            }
        }
    }
}

namespace ActionsList
{
    public class RepairAction : GenericAction
    {
        public enum CardFace
        {
            FaceDown,
            FaceUp
        }

        public Func<bool> PayRepairCost = () => true;

        private readonly CardFace damageCardFace;
        private readonly CriticalCardType? criticalCardType;

        public RepairAction(CardFace face, CriticalCardType? type = null)
        {
            damageCardFace = face;
            criticalCardType = type;

            EffectName = Name = "Repair 1 " + face.ToString().ToLower() + (type != null ? " " + type.ToString() : "")  + " damage";
        }

        public override void ActionTake()
        {
            if (PayRepairCost())
            {
                if (damageCardFace == CardFace.FaceDown)
                {
                    if (Host.Damage.DiscardRandomFacedownCard())
                    {
                        Sounds.PlayShipSound("R2D2-Proud");
                        Messages.ShowInfoToHuman("Facedown Damage card is discarded");
                    }
                }
                else if (damageCardFace == CardFace.FaceUp)
                {
                    List<GenericDamageCard> shipCritsList = Host.Damage.GetFaceupCrits(criticalCardType);


                    if (shipCritsList.Count == 1)
                    {
                        Host.Damage.FlipFaceupCritFacedown(shipCritsList.First());
                        Sounds.PlayShipSound("R2D2-Proud");
                    }
                    else if (shipCritsList.Count > 1)
                    {
                        Phases.StartTemporarySubPhaseOld(
                            Source.Name + ": Select faceup ship Crit",
                            typeof(SubPhases.R5AstromechDecisionSubPhase)
                        );
                    }
                }
            }
            Phases.CurrentSubPhase.CallBack();
        }
    }
}
