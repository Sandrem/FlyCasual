﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;
using Bombs;
using RuleSets;
using SubPhases;
using Obstacles;
using BoardTools;

namespace UpgradesList
{

    public class SeismicCharges : GenericTimedBomb, ISecondEditionUpgrade
    {
        private GenericObstacle ChosenObstacle;

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
            IsDiscardedAfterDropped = false;
            UsesCharges = true;

            MaxCharges = 2;
            Cost = 3;
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
                Name,
                delegate {
                    BombsManager.ToggleReadyToDetonateHighLight(false);
                    Triggers.FinishTrigger();
                }
            );

            subphase.PrepareByParameters(
                SelectObstacle,
                TrySelectObstacle,
                Host.Owner.PlayerNo,
                true,
                Name,
                "Select obstacle to destroy",
                ImageUrl
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
