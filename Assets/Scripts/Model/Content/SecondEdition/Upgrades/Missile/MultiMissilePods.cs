using Arcs;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class MultiMissilePods : GenericSpecialWeapon
    {
        public MultiMissilePods() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Multi-Missile Pods",
                types: new List<UpgradeType>(){
                    UpgradeType.Missile,
                    UpgradeType.Missile
                },
                cost: 4,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 2,
                    minRange: 1,
                    maxRange: 2,
                    arc: ArcType.FullFront,
                    charges: 5,
                    requiresTokens: new List<Type>(){
                        typeof(CalculateToken),
                        typeof(BlueTargetLockToken)
                    }
                ),
                abilityType: typeof(Abilities.SecondEdition.MultiMissilePodsAbility)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/2b/f1/2bf158e3-099c-4969-a882-b52af5a88273/swz71_upgrade_multi-missile-pod.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class MultiMissilePodsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += CheckConditions;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= CheckConditions;
        }

        private void CheckConditions()
        {
            if (HostShip.SectorsInfo.IsShipInSector(Combat.Defender, ArcType.Front)
                && (Combat.ChosenWeapon.GetType() == HostUpgrade.GetType())
                && HostUpgrade.State.Charges > 0
            )
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskToUseCharges);
            }
        }

        private void AskToUseCharges(object sender, EventArgs e)
        {
            MultiMissilePodDecisionSubphase subphase = Phases.StartTemporarySubPhaseNew<MultiMissilePodDecisionSubphase>("Multi-Missile Pod Decision", Triggers.FinishTrigger);

            subphase.DescriptionShort = "Multi-Missile Pods";
            subphase.DescriptionLong = "You can spend charges to roll additional dice";
            subphase.ImageSource = HostUpgrade;

            if (HostShip.SectorsInfo.IsShipInSector(Combat.Defender, ArcType.Front) && HostUpgrade.State.Charges > 0)
            {
                subphase.AddDecision("Add 1 die", AddOneDie);
            }

            if (HostShip.SectorsInfo.IsShipInSector(Combat.Defender, ArcType.Bullseye) && HostUpgrade.State.Charges > 1)
            {
                subphase.AddDecision("Add 2 dice", AddTwoDice);
            }

            subphase.DefaultDecisionName = subphase.GetDecisions().Last().Name;
            subphase.DecisionOwner = HostShip.Owner;
            subphase.ShowSkipButton = true;

            subphase.Start();
        }

        private void AddOneDie(object sender, EventArgs e)
        {
            HostUpgrade.State.SpendCharge();
            HostShip.AfterGotNumberOfAttackDice += IncreaseByOne;
            DecisionSubPhase.ConfirmDecision();
        }

        private void IncreaseByOne(ref int count)
        {
            count++;
            HostShip.AfterGotNumberOfAttackDice -= IncreaseByOne;
        }

        private void AddTwoDice(object sender, EventArgs e)
        {
            HostUpgrade.State.SpendCharges(2);
            HostShip.AfterGotNumberOfAttackDice += IncreaseByTwo;
            DecisionSubPhase.ConfirmDecision();
        }

        private void IncreaseByTwo(ref int count)
        {
            count += 2;
            HostShip.AfterGotNumberOfAttackDice -= IncreaseByTwo;
        }

        public class MultiMissilePodDecisionSubphase: DecisionSubPhase { }
    }
}