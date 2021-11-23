using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;
using System.Linq;
using Bombs;
using SubPhases;

namespace UpgradesList.SecondEdition
{
    public class ConcussionBombs : GenericTimedBombSE
    {
        GenericShip _ship = null;

        public ConcussionBombs() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Concussion Bombs",
                type: UpgradeType.Device,
                cost: 4,
                charges: 3,
                abilityType: typeof(Abilities.SecondEdition.ConcussionBombsAbility)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/00/c6/00c65f3f-e738-4a5d-80a8-6b3aef6e8074/swz71_upgrade_concussion-bomb.png";

            bombPrefabPath = "Prefabs/Bombs/ConcussionBomb";
        }

        public override void ExplosionEffect(GenericShip ship, Action callBack)
        {
            _ship = ship;

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Suffer damage from bomb",
                TriggerType = TriggerTypes.OnDamageIsDealt,
                TriggerOwner = ship.Owner.PlayerNo,
                EventHandler = Detonation,
                EventArgs = new DamageSourceEventArgs()
                {
                    Source = this,
                    DamageType = DamageTypes.BombDetonation
                }
            });

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, callBack);
        }

        public override void PlayDetonationAnimSound(GenericDeviceGameObject bombObject, Action callBack)
        {
            BombsManager.CurrentDevice = this;

            Sounds.PlayBombSound(bombObject, "Explosion-7");
            bombObject.transform.Find("Explosion/Explosion").GetComponent<ParticleSystem>().Play();
            bombObject.transform.Find("Explosion/Ring").GetComponent<ParticleSystem>().Play();

            GameManagerScript.Wait(1.4f, delegate { callBack(); });
        }

        private void Detonation(object sender, EventArgs e)
        {
            Messages.ShowInfoToHuman($"{UpgradeInfo.Name}: Dealt facedown card to {_ship.PilotInfo.PilotName}");
            _ship.SufferHullDamage(false, e, AskToExposeOrBeStrained);
        }

        private void AskToExposeOrBeStrained()
        {
            Selection.ChangeActiveShip(_ship);

            ConcussionBombDecisionSubphase subphase = Phases.StartTemporarySubPhaseNew<ConcussionBombDecisionSubphase>(
                "Concussion Bomb Decision",
                delegate {
                    Selection.DeselectThisShip();
                    Triggers.FinishTrigger();
                }
            );

            subphase.DescriptionShort = UpgradeInfo.Name;
            subphase.DescriptionLong = "Expose 1 damage card unless you choos to gain 1 strain token";
            subphase.ImageSource = this;

            subphase.AddDecision("Gain 1 Strain token", GainStrainToken);
            subphase.AddDecision("Expose 1 damage card", ExposeDamageCard);

            subphase.DefaultDecisionName = subphase.GetDecisions().First().Name;
            subphase.DecisionOwner = _ship.Owner;
            subphase.ShowSkipButton = false;

            subphase.Start();
        }

        private void GainStrainToken(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            _ship.Tokens.AssignToken(
                typeof(Tokens.StrainToken),
                delegate {
                    Selection.DeselectThisShip();
                    Triggers.FinishTrigger();
                }
            );
        }

        private void ExposeDamageCard(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            _ship.Damage.ExposeRandomFacedownCard(delegate {
                Selection.DeselectThisShip();
                Triggers.FinishTrigger();
            });
        }

        private class ConcussionBombDecisionSubphase : DecisionSubPhase { }
    }
}

namespace Abilities.SecondEdition
{
    public class ConcussionBombsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCheckSystemSubphaseCanBeSkipped += CheckSystemPhaseSkip;
            BombsManager.OnCheckBombDropCanBeSkipped += CheckBombDropFix;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCheckSystemSubphaseCanBeSkipped -= CheckSystemPhaseSkip;
            BombsManager.OnCheckBombDropCanBeSkipped -= CheckBombDropFix;
        }

        private void CheckSystemPhaseSkip(ref bool canBeSkipped)
        {
            if (HostUpgrade.State.Charges > 0
                && HostUpgrade.State.Charges < HostUpgrade.State.MaxCharges
                && !HostShip.IsBombAlreadyDropped)
            {
                canBeSkipped = false;
            }
        }

        private void CheckBombDropFix(GenericShip ship, ref bool canBeSkipped)
        {
            //Only for ship that owns this upgrade
            if (HostUpgrade.HostShip != ship) return;

            if (HostUpgrade.State.Charges > 0
                && HostUpgrade.State.Charges < HostUpgrade.State.MaxCharges
                && !HostShip.IsBombAlreadyDropped)
            {
                canBeSkipped = false;
            }
        }
    }
}