using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;
using System.Linq;
using Bombs;
using Obstacles;
using BoardTools;
using SubPhases;

namespace UpgradesList.SecondEdition
{
    public class SeismicCharges : GenericTimedBomb
    {
        private GenericObstacle ChosenObstacle;

        public SeismicCharges() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Seismic Charges",
                type: UpgradeType.Bomb,
                charges: 2,
                cost: 3,
                seImageNumber: 67
            );

            bombPrefabPath = "Prefabs/Bombs/SeismicCharge";
        }

        protected override void Detonate()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Detonation of Seismic Charges",
                TriggerType = TriggerTypes.OnAbilityDirect,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = StartSelectObstacle
            });

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, FinallyDetonateBomb);
        }

        private void FinallyDetonateBomb()
        {
            BombsManager.UnregisterBomb(BombsManager.CurrentBombObject);
            CurrentBombObjects.Remove(BombsManager.CurrentBombObject);

            if (ChosenObstacle != null)
            {
                foreach (GenericShip ship in Roster.AllShips.Values)
                {
                    ShipObstacleDistance shipOstacleDist = new ShipObstacleDistance(ship, ChosenObstacle);
                    if (shipOstacleDist.Range < 2)
                    {
                        RegisterDetonationTriggerForShip(ship);
                    }
                }
            }

            PlayDetonationAnimSound(BombsManager.CurrentBombObject, DetonateObstacle);
        }

        private void DetonateObstacle()
        {
            //TODO: Animation
            if (ChosenObstacle != null) ObstaclesManager.DestroyObstacle(ChosenObstacle);

            BombsManager.ResolveDetonationTriggers();
        }

        private void StartSelectObstacle(object sender, System.EventArgs e)
        {
            ChosenObstacle = null;

            BombsManager.ToggleReadyToDetonateHighLight(true);

            SelectObstacleSubPhase subphase = Phases.StartTemporarySubPhaseNew<SelectObstacleSubPhase>(
                UpgradeInfo.Name,
                delegate {
                    BombsManager.ToggleReadyToDetonateHighLight(false);
                    Triggers.FinishTrigger();
                }
            );

            subphase.PrepareByParameters(
                SelectObstacle,
                TrySelectObstacle,
                HostShip.Owner.PlayerNo,
                true,
                UpgradeInfo.Name,
                "Select obstacle to destroy",
                this
            );

            subphase.Start();
        }

        private bool TrySelectObstacle(GenericObstacle obstacle)
        {
            BombObstacleDistance bombOstacleDist = new BombObstacleDistance(BombsManager.CurrentBomb, obstacle);
            return bombOstacleDist.Range < 2;
        }

        private void SelectObstacle(GenericObstacle obstacle)
        {
            Messages.ShowInfo("Obstacle was selected");
            ChosenObstacle = obstacle;

            SelectObstacleSubPhase.SelectObstacle();
        }

        // FIRST EDITION

        public override void ExplosionEffect(GenericShip ship, Action callBack)
        {
            DamageSourceEventArgs seismicDamage = new DamageSourceEventArgs()
            {
                Source = this,
                DamageType = DamageTypes.BombDetonation
            };

            ship.Damage.TryResolveDamage(1, seismicDamage, callBack);
        }

        public override void PlayDetonationAnimSound(GameObject bombObject, Action callBack)
        {
            BombsManager.CurrentBomb = this;
            bombObject.transform.Find("Explosion/Explosion").GetComponent<ParticleSystem>().Play();

            GameManagerScript.Wait(1, delegate { PlayDefferedSound(bombObject, callBack); });
        }

        private void PlayDefferedSound(GameObject bombObject, Action callBack)
        {
            Sounds.PlayBombSound(bombObject, "SeismicBomb");
            bombObject.transform.Find("Explosion/Ring").GetComponent<ParticleSystem>().Play();

            GameManagerScript.Wait(1.4f, delegate { callBack(); });
        }
    }
}