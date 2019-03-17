﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;
using System.Linq;
using SubPhases;

namespace UpgradesList.FirstEdition
{
    public class ProximityMines : GenericContactMineFE
    {
        public ProximityMines() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Proximity Mines",
                UpgradeType.Bomb,
                cost: 3
            );

            bombPrefabPath = "Prefabs/Bombs/ProximityMine";

            IsDiscardedAfterDropped = true;
        }

        public override void ExplosionEffect(GenericShip ship, Action callBack)
        {
            Selection.ActiveShip = ship;
            Phases.StartTemporarySubPhaseOld(
                "Damage from " + UpgradeInfo.Name,
                typeof(SubPhases.FirstEdition.ProximityMinesCheckSubPhase),
                delegate {
                    Phases.FinishSubPhase(typeof(SubPhases.FirstEdition.ProximityMinesCheckSubPhase));
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

namespace SubPhases.FirstEdition
{
    public class ProximityMinesCheckSubPhase : DiceRollCheckSubPhase
    {
        public override void Prepare()
        {
            DiceKind = DiceKind.Attack;
            DiceCount = 3;

            AfterRoll = FinishAction;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();

            CurrentDiceRoll.RemoveAllFailures();
            if (!CurrentDiceRoll.IsEmpty)
            {
                SufferDamage();
            }
            else
            {
                NoDamage();
            }

        }

        private void SufferDamage()
        {
            Messages.ShowError("Proximity Mines: The attacked ship suffered damage.");

            DamageSourceEventArgs proximityDamage = new DamageSourceEventArgs()
            {
                Source = "Proximity Mines",
                DamageType = DamageTypes.BombDetonation
            };

            Selection.ActiveShip.Damage.TryResolveDamage(CurrentDiceRoll.DiceList, proximityDamage, CallBack);
        }

        private void NoDamage()
        {
            Messages.ShowInfoToHuman("The attacked ship suffered no damage.");
            CallBack();
        }
    }
}