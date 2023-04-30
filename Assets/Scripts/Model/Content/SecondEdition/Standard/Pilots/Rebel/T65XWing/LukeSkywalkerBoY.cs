using Abilities.SecondEdition;
using ActionsList;
using BoardTools;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship
{
    namespace SecondEdition.T65XWing
    {
        public class LukeSkywalkerBoY : T65XWing
        {
            public LukeSkywalkerBoY() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Luke Skywalker",
                    "Battle of Yavin",
                    Faction.Rebel,
                    5,
                    6,
                    0,
                    isLimited: true,
                    abilityType: typeof(LukeSkywalkerAbility),
                    force: 2,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.ForcePower,
                        UpgradeType.Talent,
                        UpgradeType.Torpedo,
                        UpgradeType.Astromech
                    },
                    tags: new List<Tags>
                    {
                        Tags.LightSide,
                        Tags.XWing
                    },
                    seImageNumber: 2,
                    skinName: "Luke Skywalker",
                    isStandardLayout: true
                );

                ShipAbilities.Add(new HopeAbility());

                MustHaveUpgrades.Add(typeof(AttackSpeed));
                MustHaveUpgrades.Add(typeof(InstinctiveAim));
                MustHaveUpgrades.Add(typeof(ProtonTorpedoes));
                MustHaveUpgrades.Add(typeof(R2D2BoY));

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/6/61/Lukeskywalker-battleofyavin.png";

                PilotNameCanonical = "lukeskywalker-battleofyavin";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class HopeAbility : GenericAbility
    {
        private GenericShip PreviousCurrentShip { get; set; }

        public override void ActivateAbility()
        {
            GenericShip.OnShipIsDestroyedGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnShipIsDestroyedGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship, bool flag)
        {
            if (!Tools.IsSameTeam(HostShip, ship) || Tools.IsSameShip(HostShip, ship)) return;

            DistanceInfo distanceInfo = new DistanceInfo(HostShip, ship);
            if (distanceInfo.Range > 3) return;

            RegisterAbilityTrigger(
                TriggerTypes.OnShipIsDestroyed,
                AskWhatToDo,
                customTriggerName: $"Hope (ID: {HostShip.ShipId})"
            );
        }

        private void AskWhatToDo(object sender, EventArgs e)
        {
            PreviousCurrentShip = Selection.ThisShip;

            Selection.ChangeActiveShip(HostShip);
            Selection.ThisShip.AskPerformFreeAction(
                new List<GenericAction>()
                {
                    new FocusAction(){ HostShip = HostShip },
                    new BoostAction(){ HostShip = HostShip },
                },
                FinishAbility,
                descriptionShort: "Hope",
                descriptionLong: "You may perform a Focus or Boost action"
            );
        }

        private void FinishAbility()
        {
            Selection.ChangeActiveShip(PreviousCurrentShip);
            Triggers.FinishTrigger();
        }
    }
}