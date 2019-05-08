using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;
using System.Linq;
using SubPhases;

namespace UpgradesList.SecondEdition
{
    public class ProximityMines : GenericContactMineSE
    {
        public ProximityMines() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Proximity Mines",
                UpgradeType.Bomb,
                cost: 6,
                charges: 2,
                cannotBeRecharged: true,
                seImageNumber: 66
            );

            bombPrefabPath = "Prefabs/Bombs/ProximityMine";
        }

        public override void ExplosionEffect(GenericShip ship, Action callBack)
        {
            Selection.ActiveShip = ship;
            Phases.StartTemporarySubPhaseOld(
                "Damage from " + UpgradeInfo.Name,
                typeof(SubPhases.SecondEdition.ProximityMinesCheckSubPhase),
                delegate {
                    Phases.FinishSubPhase(typeof(SubPhases.SecondEdition.ProximityMinesCheckSubPhase));
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

namespace SubPhases.SecondEdition
{
    public class ProximityMinesCheckSubPhase : DiceRollCheckSubPhase
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
            CurrentDiceRoll.AddDice(DieSide.Success);
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
            Messages.ShowInfo("Proximity Mines: The attacked ship suffered damage");

            DamageSourceEventArgs proximityDamage = new DamageSourceEventArgs()
            {
                Source = "Proximity Mines",
                DamageType = DamageTypes.BombDetonation
            };

            Selection.ActiveShip.Damage.TryResolveDamage(CurrentDiceRoll.DiceList, proximityDamage, CallBack);
        }

        private void NoDamage()
        {
            Messages.ShowInfoToHuman("The attacked ship suffered no damage");
            CallBack();
        }
    }
}