using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableForLowQuality : MonoBehaviour
{
    void Start()
    {
        if (Options.Quality == 0) this.gameObject.SetActive(false);
    }
}
