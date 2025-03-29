using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    private bool hasHitGround = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Destroyable"))
        {
            Building building = other.GetComponent<Building>();
            if (building != null)
            {
                building.TakeHit();
            }
            else
            {
                Destroy(other.gameObject);
            }
        }
        else if (other.CompareTag("Ground") && !hasHitGround)
        {
            hasHitGround = true;

            GameManager gm = FindObjectOfType<GameManager>();
            if (gm != null)
            {
                gm.LoseHealth();
            }
        }

        Destroy(gameObject);
    }
}
