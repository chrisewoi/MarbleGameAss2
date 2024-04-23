using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPowerup : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Player.SpeedBoost();
    }

}
