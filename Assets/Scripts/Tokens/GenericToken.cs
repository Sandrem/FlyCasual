using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tokens
{

    public class GenericToken
    {
        protected GameManagerScript Game;

        public string Name;
        public bool Temporary = true;

        public GenericToken() {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        }

    }

}
