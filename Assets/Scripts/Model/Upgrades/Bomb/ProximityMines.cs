using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;
using Bombs;
using RuleSets;

namespace UpgradesList
{

    public class ProximityMines : GenericContactMine, ISecondEditionUpgrade
    {

        public ProximityMines() : base()
        {
            Types.Add(UpgradeType.Bomb);
            Name = "Proximity Mines";
            Cost = 6;

            bombPrefabPath = "Prefabs/Bombs/ProximityMine";

            IsDiscardedAfterDropped = true;
        }

        public void AdaptUpgradeToSecondEdition()
        {
            IsDiscardedAfterDropped = false;
            UsesCharges = true;

            MaxCharges = 2;
            Cost = 6;

            SEImageNumber = 66;
        }


        public override void ExplosionEffect(GenericShip ship, Action callBack)
        {
            Selection.ActiveShip = ship;
            Phases.StartTemporarySubPhaseOld(
                "Damage from " + Name,
                typeof(SubPhases.ProximityMinesCheckSubPhase),
                delegate {
                    Phases.FinishSubPhase(typeof(SubPhases.ProximityMinesCheckSubPhase));
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
            Messages.ShowError("Proximity Mines: ship suffered damage");

            DamageSourceEventArgs proximityDamage = new DamageSourceEventArgs()
            {
                Source = "Proximity Mines",
                DamageType = DamageTypes.BombDetonation
            };

            Selection.ActiveShip.Damage.TryResolveDamage(CurrentDiceRoll.DiceList, proximityDamage, CallBack);
        }

        private void NoDamage()
        {
            Messages.ShowInfoToHuman("No damage");
            CallBack();
        }
    }

}
