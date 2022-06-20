using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Norsevar.MusicAndSFX
{
    public class VillageMusicTriggerScript : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                FMODEventEmittersTogglerScript.ActivateHubMusic();
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                FMODEventEmittersTogglerScript.DeactivateHubMusic();
            }
        }
    }
}
