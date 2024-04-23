using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPowerup : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Player.JumpBoost();
    }
}
