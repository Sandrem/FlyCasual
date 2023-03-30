using Abilities.SecondEdition;
using Actions;
using ActionsList;
using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEInterceptor
    {
        public class Sigma7BoY : TIEInterceptor
        {
            public Sigma7BoY() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Sigma 7",
                    "Battle of Yavin",
                    Faction.Imperial,
                    4,
                    4,
                    0,
                    charges: 2,
                    isLimited: true,
                    abilityType: typeof(Sigma7BoYAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Sensor
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    isStandardLayout: true
                );

                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(TargetLockAction)));

                AutoThrustersAbility oldAbility = (AutoThrustersAbility) ShipAbilities.First(n => n.GetType() == typeof(AutoThrustersAbility));
                //oldAbility.DeactivateAbility();
                ShipAbilities.Remove(oldAbility);
                ShipAbilities.Add(new SensitiveControlsBoYRealAbility());

                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.Marksmanship));
                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.FireControlSystem));

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/b/bc/Sigma7-battleofyavin.png";

                PilotNameCanonical = "sigma7-battleofyavin";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class Sigma7BoYAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCheckSystemsAbilityActivation += CheckAbility;
            HostShip.OnSystemsAbilityActivation += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckSystemsAbilityActivation -= CheckAbility;
            HostShip.OnSystemsAbilityActivation -= RegisterAbility;
        }

        private void CheckAbility(GenericShip ship, ref bool flag)
        {
            flag = (HostShip.State.Charges > 0);
        }

        private void RegisterAbility(GenericShip ship)
        {
            if (HostShip.State.Charges > 0)
            {
                RegisterAbilityTrigger
                (
                    TriggerTypes.OnSystemsAbilityActivation,
                    AskToSelectShipForLock,
                    customTriggerName: "Sigma 7's ability"
                );
            }
        }

        private void AskToSelectShipForLock(object sender, EventArgs e)
        {
            if (Board.GetShipsAtRange(HostShip, new UnityEngine.Vector2(0,1), Team.Type.Enemy).Count > 0)
            {
                SelectTargetForAbility
                (
                    AcquireLock,
                    IsEnemyAtR01,
                    GetAiPriority,
                    HostShip.Owner.PlayerNo,
                    HostShip.PilotInfo.PilotName,
                    "You may spend 1 Charge to acquire a lock on an enemy ship at range 0-1"
                );
            }
            else
            {
                Messages.ShowInfo($"{HostShip.PilotInfo.PilotName}: There are no targets for ability");
                Triggers.FinishTrigger();
            }
        }

        private void AcquireLock()
        {
            HostShip.SpendCharge();
            ActionsHolder.AcquireTargetLock(HostShip, TargetShip, SelectShipSubPhase.FinishSelection, SelectShipSubPhase.FinishSelection);
        }

        private bool IsEnemyAtR01(GenericShip ship)
        {
            DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
            return distInfo.Range <= 1;
        }

        private int GetAiPriority(GenericShip ship)
        {
            return ship.PilotInfo.Cost;
        }
    }
}