using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public enum BuildingType { Wood, Brick, Metal, Explosive}
    public BuildingType type;
    public Material[] brickMaterials;
    public Material[] metalMaterials;

    private Rigidbody rb;
    private Renderer rend;

    public bool canWarpOnGround = false;
    public Transform warpTarget;

    private int health;
    public int point;
    public ParticleSystem explosionParticle;

    private GameManager gameManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (type != BuildingType.Explosive && !canWarpOnGround)
        {
            gameManager.RegisterBuilding();
        }

        switch (type)
        {
            case BuildingType.Wood: health = 1; break;
            case BuildingType.Brick: health = 2; break;
            case BuildingType.Metal: health = 3; break;
            case BuildingType.Explosive: health = 1; break;
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        if (canWarpOnGround && warpTarget != null && collision.collider.CompareTag("Ground"))
        {
            transform.position = warpTarget.position;
            canWarpOnGround = false;
        }
    }

    public void TakeHit()
    {
        health--;

        if (type == BuildingType.Brick && brickMaterials.Length > 0)
        {
            rend.material = brickMaterials[0];
        }

        if (type == BuildingType.Metal && metalMaterials.Length > 0)
        {
            int index = Mathf.Clamp(metalMaterials.Length - health, 0, metalMaterials.Length - 1);
            rend.material = metalMaterials[index];
        }

        if (type == BuildingType.Explosive)
        {
            if (explosionParticle != null)
            {
                Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation);
            }

            gameManager.GameOver();
            Destroy(gameObject);
            return;
        }

        if (canWarpOnGround)
        {
            if (explosionParticle != null)
            {
                Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation);
            }

            gameManager.GameOver();
            Destroy(gameObject);
            return;
        }

        if (health <= 0)
        {
            if (explosionParticle != null)
            {
                Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation);
            }

            gameManager.UpdateScore(point);

            if (type != BuildingType.Explosive)
            {
                gameManager.NotifyBuildingDestroyed();
            }

            Destroy(gameObject);
        }
    }


}
