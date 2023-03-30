using Abilities.SecondEdition;
using ActionsList;
using BoardTools;
using Content;
using System;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship
{
    namespace SecondEdition.ModifiedYT1300LightFreighter
    {
        public class HanSoloBoY : ModifiedYT1300LightFreighter
        {
            public HanSoloBoY() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Han Solo",
                    "Battle of Yavin",
                    Faction.Rebel,
                    6,
                    7,
                    0,
                    isLimited: true,
                    abilityType: typeof(HanSoloBoYAbility),
                    charges: 4,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Crew,
                        UpgradeType.Illicit,
                        UpgradeType.Title,
                        UpgradeType.Configuration
                    },
                    tags: new List<Tags>
                    {
                        Tags.Freighter,
                        Tags.YT1300
                    },
                    isStandardLayout: true
                );

                ShipAbilities.Add(new SoloAbility());

                MustHaveUpgrades.Add(typeof(ChewbaccaBoY));
                MustHaveUpgrades.Add(typeof(RiggedCargoChute));
                MustHaveUpgrades.Add(typeof(MillenniumFalcon));
                MustHaveUpgrades.Add(typeof(L337sProgramming));

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/5/5e/Hansolo-battleofyavin.png";

                PilotNameCanonical = "hansolo-battleofyavin";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class HanSoloBoYAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackHitAsAttacker += RegisterHanSoloAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackHitAsAttacker -= RegisterHanSoloAbility;
        }

        public void RegisterHanSoloAbility()
        {
            if (HostShip.State.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackHit, AskPerformCoordinateAction);
            }
        }

        private void AskPerformCoordinateAction(object sender, System.EventArgs e)
        {
            if (HostShip.State.Charges > 0)
            {
                HostShip.BeforeActionIsPerformed += PayChargeCost;

                HostShip.AskPerformFreeAction
                (
                    new CoordinateAction() { HostShip = HostShip },
                    CleanUp,
                    HostShip.PilotInfo.PilotName,
                    "You may spend 1 charge to perform a Coordinate action"
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void PayChargeCost(GenericAction action, ref bool isFreeAction)
        {
            HostShip.SpendCharge();
            HostShip.BeforeActionIsPerformed -= PayChargeCost;
        }

        private void CleanUp()
        {
            HostShip.BeforeActionIsPerformed -= PayChargeCost;
            Triggers.FinishTrigger();
        }
    }

    public class SoloAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification
            (
                "Solo",
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Reroll,
                1,
                payAbilityCost: SpendCharge
            );
        }

        private bool IsAvailable()
        {
            return ((Combat.Attacker.ShipId == HostShip.ShipId || Combat.Defender.ShipId == HostShip.ShipId)
                && HostShip.State.Charges > 0
                && Board.GetShipsAtRange(HostShip, new Vector2(0, 1), Team.Type.Friendly).Count == 1);
        }

        private int GetAiPriority()
        {
            return 90;
        }

        private void SpendCharge(Action<bool> callback)
        {
            HostShip.SpendCharge();
            callback(true);
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}
