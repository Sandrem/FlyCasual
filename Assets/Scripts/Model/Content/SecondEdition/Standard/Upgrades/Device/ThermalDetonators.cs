using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;
using System.Linq;
using SubPhases;
using SubPhases.SecondEdition;
using Bombs;
using BoardTools;
using Movement;

namespace UpgradesList.SecondEdition
{
    public class ThermalDetonators : GenericTimedBombSE
    {
        public ThermalDetonators() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Thermal Detonators",
                type: UpgradeType.Device,
                cost: 5,
                charges: 4,
                abilityType: typeof(Abilities.SecondEdition.ThermalDetonatorsAbility),
                subType: UpgradeSubType.Bomb
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/52/bf/52bff580-897d-4af0-9084-5088765babf0/swz80_upgrade_thermal-detonators.png";

            bombPrefabPath = "Prefabs/Bombs/ThermalDetonator";
        }

        public override void ExplosionEffect(GenericShip ship, Action callBack)
        {
            Selection.ActiveShip = ship;
            ThermalDetonatorsCheckSubPhase sufferBombletDamageSubphase = Phases.StartTemporarySubPhaseNew<ThermalDetonatorsCheckSubPhase>(
                "Damage from " + UpgradeInfo.Name,
                () => {
                    Phases.FinishSubPhase(typeof(ThermalDetonatorsCheckSubPhase));
                    callBack();
                }
            );
            sufferBombletDamageSubphase.HostUpgrade = this;
            sufferBombletDamageSubphase.Start();
        }

        public override void PlayDetonationAnimSound(GenericDeviceGameObject bombObject, Action callBack)
        {
            int random = UnityEngine.Random.Range(1, 8);
            Sounds.PlayBombSound(bombObject, "Explosion-" + random);
            bombObject.transform.Find("Explosion/Explosion").GetComponent<ParticleSystem>().Play();
            bombObject.transform.Find("Explosion/Ring").GetComponent<ParticleSystem>().Play();

            GameManagerScript.Wait(1, delegate { callBack(); });
        }

    }

}

namespace Abilities.SecondEdition
{
    public class ThermalDetonatorsAbility : GenericAbility
    {
        private bool IsSecondBombDropped = false;
        private ManeuverTemplate ForbiddenTemplate;

        public override void ActivateAbility()
        {
            HostShip.OnGetAvailableBombDropTemplatesNoConditions += TryToAddSecondTemplate;
            HostShip.OnGetReloadChargesCount += CheckAdditionalReload;
            HostShip.OnCheckDropOfSecondDevice += CheckSecondDrop;
        }

        private void CheckAdditionalReload(GenericUpgrade upgrade, ref int count)
        {
            if (upgrade == HostUpgrade && upgrade.State.Charges <= (upgrade.State.MaxCharges - 2))
            {
                Messages.ShowInfo("Thermal Detonators: Additional charge is restored during Reload action");
                count++;
            }
        }

        private void TryToAddSecondTemplate(List<ManeuverTemplate> availableTemplates, GenericUpgrade upgrade)
        {
            if (upgrade == HostUpgrade)
            {
                ManeuverTemplate secondTemplate = new ManeuverTemplate(
                    ManeuverBearing.Straight,
                    ManeuverDirection.Forward,
                    ManeuverSpeed.Speed2,
                    isBombTemplate: true
                );

                if (!availableTemplates.Any(t => t.Name == secondTemplate.Name))
                {
                    availableTemplates.Add(secondTemplate);
                }
            }
        }

        private void CheckSecondDrop()
        {
            if (BombsManager.CurrentDevice.GetType() == HostUpgrade.GetType() && !IsSecondBombDropped && HostUpgrade.State.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnCheckDropOfSecondDevice, AskSecondDrop);
            }
            else
            {
                IsSecondBombDropped = false;
                ForbiddenTemplate = null;
            }
        }

        private void AskSecondDrop(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                DoSecondDrop,
                descriptionLong: "Do you want to spend 1 charge to drop second bomb using another template?",
                imageHolder: HostUpgrade,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private void DoSecondDrop(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            IsSecondBombDropped = true;
            ForbiddenTemplate = BombsManager.LastManeuverTemplateUsed;

            HostShip.OnGetAvailableBombDropTemplatesForbid += RestrictLastTemplate;

            // TODO: Check interactions
            BombsManager.DropSelectedDevice(false);
        }

        private void RestrictLastTemplate(List<ManeuverTemplate> availableTemplates, GenericUpgrade upgrade)
        {
            HostShip.OnGetAvailableBombDropTemplatesForbid -= RestrictLastTemplate;

            ManeuverTemplate existingTemplate = availableTemplates.FirstOrDefault(n => n.Name == ForbiddenTemplate?.Name);
            if (existingTemplate != null) availableTemplates.Remove(existingTemplate);
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetAvailableBombDropTemplatesNoConditions -= TryToAddSecondTemplate;
            HostShip.OnGetReloadChargesCount -= CheckAdditionalReload;
            HostShip.OnCheckDropOfSecondDevice -= CheckSecondDrop;
        }
    }
}

namespace SubPhases.SecondEdition
{
    public class ThermalDetonatorsCheckSubPhase : DiceRollCheckSubPhase
    {
        public GenericUpgrade HostUpgrade;

        public override void Prepare()
        {
            DiceKind = DiceKind.Attack;
            DiceCount = 1;

            AfterRoll = FinishAction;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();

            DieSide dieResult = CurrentDiceRoll.ResultsArray.First();
            switch (dieResult)
            {
                case DieSide.Focus:
                    AssignStrain();
                    break;
                case DieSide.Success:
                case DieSide.Crit:
                    SufferDamage(dieResult);
                    break;
                default:
                    NoDamage();
                    break;
            }
        }

        private void AssignStrain()
        {
            Messages.ShowInfo($"Thermal Detonators: " +
                $"{ Selection.ActiveShip.PilotInfo.PilotName} (ID:{ Selection.ActiveShip.ShipId}) " +
                $"assigned Strain token");

            Selection.ActiveShip.Tokens.AssignToken(
                typeof(Tokens.StrainToken),
                CallBack
            );
        }

        private void SufferDamage(DieSide dieResult)
        {
            string damageType = (dieResult == DieSide.Crit) ? "critical" : "regular";
            Messages.ShowInfo($"Thermal Detonators: " +
                $"{Selection.ActiveShip.PilotInfo.PilotName} (ID:{Selection.ActiveShip.ShipId}) " +
                $"suffered {damageType} damage");

            DamageSourceEventArgs thermalDetonatorsDamage = new DamageSourceEventArgs()
            {
                Source = HostUpgrade,
                DamageType = DamageTypes.BombDetonation
            };

            int regularDamageCount = (dieResult == DieSide.Crit) ? 0 : 1;
            int critDamageCount = (dieResult == DieSide.Crit) ? 1 : 0;
            Selection.ActiveShip.Damage.TryResolveDamage(regularDamageCount, critDamageCount, thermalDetonatorsDamage, CallBack);
        }

        private void NoDamage()
        {
            Messages.ShowInfo($"Thermal Detonators: " +
                $"{ Selection.ActiveShip.PilotInfo.PilotName} (ID:{ Selection.ActiveShip.ShipId}) " +
                $"didn't suffer any effects");
            CallBack();
        }
    }

}
