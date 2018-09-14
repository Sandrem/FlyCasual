﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;
using System.Linq;

namespace UpgradesList
{

    public class ClusterMines : GenericContactMine
    {

        public ClusterMines() : base()
        {
            Types.Add(UpgradeType.Bomb);
            Name = "Cluster Mines";
            Cost = 4;

            bombPrefabPath = "Prefabs/Bombs/ClusterMinesCentral";

            bombSidePrefabPath = "Prefabs/Bombs/ClusterMinesSide";
            bombSideDistanceX = 2.362f;
            bombSideDistanceZ = 0.0764f;

            IsDiscardedAfterDropped = true;
        }

        public override void ExplosionEffect(GenericShip ship, Action callBack)
        {
            Selection.ActiveShip = ship;
            Phases.StartTemporarySubPhaseOld(
                "Damage from " + Name,
                typeof(SubPhases.ClusterMinesCheckSubPhase),
                delegate {
                    Phases.FinishSubPhase(typeof(SubPhases.ClusterMinesCheckSubPhase));
                    callBack();
                });
        }

        public override void PlayDetonationAnimSound(GameObject bombObject, Action callBack)
        {
            int random = UnityEngine.Random.Range(1, 8);
            Sounds.PlayBombSound(bombObject, "Explosion-" + random);
            bombObject.transform.Find("Explosion/Explosion").GetComponent<ParticleSystem>().Play();
            bombObject.transform.Find("Explosion/Ring").GetComponent<ParticleSystem>().Play();

            GameManagerScript.Wait(1, delegate { callBack(); });
        }

    }

}

namespace SubPhases
{

    public class ClusterMinesCheckSubPhase : DiceRollCheckSubPhase
    {

        public override void Prepare()
        {
            DiceKind = DiceKind.Attack;
            DiceCount = 2;

            AfterRoll = FinishAction;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();

            CurrentDiceRoll.RemoveAllFailures();
            if (!CurrentDiceRoll.IsEmpty)
            {
                foreach (Die die in CurrentDiceRoll.DiceList)
                {
                    if (die.Side == DieSide.Crit) die.SetSide(DieSide.Success);
                }

                SufferDamage();
            }
            else
            {
                NoDamage();
            }

        }

        private void SufferDamage()
        {
            Messages.ShowError("Cluster Mines: ship suffered damage");

            DamageSourceEventArgs clustermineDamage = new DamageSourceEventArgs()
            {
                Source = "Cluster Mines",
                DamageType = DamageTypes.BombDetonation
            };

            Selection.ActiveShip.Damage.TryResolveDamage(CurrentDiceRoll.DiceList, clustermineDamage, CallBack);
        }

        private void NoDamage()
        {
            Messages.ShowInfoToHuman("No damage");
            CallBack();
        }
    }

}
