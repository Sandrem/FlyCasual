using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;
using Bombs;
using RuleSets;
using SubPhases;

namespace UpgradesList
{

    public class SeismicCharges : GenericTimedBomb, ISecondEditionUpgrade
    {

        public SeismicCharges() : base()
        {
            Types.Add(UpgradeType.Bomb);
            Name = "Seismic Charges";
            Cost = 2;

            bombPrefabPath = "Prefabs/Bombs/SeismicCharge";

            IsDiscardedAfterDropped = true;
        }

        public void AdaptUpgradeToSecondEdition()
        {
            ImageUrl = "https://i.imgur.com/HGcGmlt.png";

            IsDiscardedAfterDropped = false;
            UsesCharges = true;
            MaxCharges = 2;
        }

        protected override void Detonate()
        {
            if (RuleSet.Instance is FirstEdition)
            {
                base.Detonate();
            }
            else if(RuleSet.Instance is SecondEdition)
            {
                SecondEditionDetonation();
            }
            else
            {
                Messages.ShowError("This ruleset doesn't support this ability");
            }
        }

        private void SecondEditionDetonation()
        {
            RegisterTriggerSE();
        }

        private void RegisterTriggerSE()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Detonation of Seismic Charges",
                TriggerType = TriggerTypes.OnAbilityDirect,
                TriggerOwner = Host.Owner.PlayerNo,
                EventHandler = StartSelectObstacle
            });

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, FinallyDetonateBomb);
        }

        private void FinallyDetonateBomb()
        {
            // base.base.Detonate

            BombsManager.UnregisterBomb(BombsManager.CurrentBombObject);
            CurrentBombObjects.Remove(BombsManager.CurrentBombObject);

            PlayDetonationAnimSound(BombsManager.CurrentBombObject, BombsManager.ResolveDetonationTriggers);
        }

        private void StartSelectObstacle(object sender, System.EventArgs e)
        {
            BombsManager.ToggleReadyToDetonateHighLight(true);

            SelectObstacleSubPhase subphase = Phases.StartTemporarySubPhaseNew<SelectObstacleSubPhase>(
                Name,
                delegate {
                    BombsManager.ToggleReadyToDetonateHighLight(false);
                    Triggers.FinishTrigger();
                }
            );

            subphase.PrepareByParameters(
                delegate { Messages.ShowInfo("Obstacle is selected"); },
                Host.Owner.PlayerNo,
                true,
                Name,
                "Select obstacle to destroy",
                ImageUrl
            );

            subphase.Start();
        }

        // FIRST EDITION

        public override void ExplosionEffect(GenericShip ship, Action callBack)
        {
            ship.AssignedDamageDiceroll.AddDice(DieSide.Success);

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Suffer damage from bomb",
                TriggerType = TriggerTypes.OnDamageIsDealt,
                TriggerOwner = ship.Owner.PlayerNo,
                EventHandler = ship.SufferDamage,
                EventArgs = new DamageSourceEventArgs()
                {
                    Source = this,
                    DamageType = DamageTypes.BombDetonation
                }
            });

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, callBack);
        }

        public override void PlayDetonationAnimSound(GameObject bombObject, Action callBack)
        {
            BombsManager.CurrentBomb = this;
            bombObject.transform.Find("Explosion/Explosion").GetComponent<ParticleSystem>().Play();

            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Wait(1, delegate { PlayDefferedSound(bombObject, callBack); });
        }

        private void PlayDefferedSound(GameObject bombObject, Action callBack)
        {
            Sounds.PlayBombSound(bombObject, "SeismicBomb");
            bombObject.transform.Find("Explosion/Ring").GetComponent<ParticleSystem>().Play();

            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Wait(1.4f, delegate { callBack(); });
        }

    }

}
