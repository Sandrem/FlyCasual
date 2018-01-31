using System;
 using System.Collections;
 using System.Collections.Generic;
 using UnityEngine;
 using Upgrade;
 using Ship;
 using Bombs;
 
 namespace UpgradesList
 {
     public class IonBombs : GenericTimedBomb
     {
         GenericShip _ship = null;
 
         public IonBombs() : base()
         {
            Type = UpgradeType.Bomb;
             Name = "Ion Bombs";
             Cost = 2;
         
             bombPrefabPath = "Prefabs/Bombs/IonBomb";
 
             IsDiscardedAfterDropped = true;
         }
 
         public override void ExplosionEffect(GenericShip ship, Action callBack)
         {
            _ship = ship;

             ship.AssignedDamageDiceroll.AddDice(DieSide.Unknown);
             
             Triggers.RegisterTrigger(new Trigger()
             {
                 Name = "Suffer Ion Tokens From Bomb",
                 TriggerType = TriggerTypes.OnTokenIsAssigned,
                 TriggerOwner = ship.Owner.PlayerNo,
                 EventHandler = SufferIonBombTokens,
                  EventArgs = new DamageSourceEventArgs()
                     {
                         Source = this,
                         DamageType = DamageTypes.BombDetonation
                     }
             });
 
             Triggers.ResolveTriggers(TriggerTypes.OnTokenIsAssigned, Triggers.FinishTrigger);
 
        }

        private void SufferIonBombTokens(object sender, EventArgs e)
        {

            _ship.Tokens.AssignToken(new Tokens.IonToken(_ship), SufferSecondIonBombToken
             );

            _ship.ToggleIonized(true);

            Triggers.ResolveTriggers(TriggerTypes.OnTokenIsAssigned, Triggers.FinishTrigger);
            
        }
 
         private void SufferSecondIonBombToken()
         {
      
             _ship.Tokens.AssignToken(new Tokens.IonToken(_ship),
            delegate {
                 Messages.ShowInfoToHuman(string.Format("{0}: Dealt second ion token to {1}", Name, _ship.PilotName));
             }
         );
  
          }
 
         private void PlayDefferedSound(GameObject bombObject, Action callBack)
         {
             Sounds.PlayBombSound(bombObject, "IonBomb");
             bombObject.transform.Find("Explosion/Ring").GetComponent<ParticleSystem>().Play();
 
             GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
             Game.Wait(1.4f, delegate { callBack(); });
         }
 
    }
 }