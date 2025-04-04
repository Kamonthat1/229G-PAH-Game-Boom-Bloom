using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RockLauncher : MonoBehaviour
{
    public GameObject rockPrefab;
    public Transform launchPoint;
    public float launchForce = 100f;
    public float maxLaunchForce = 2000f;
    public float chargeSpeed = 100f;

    [Header("Movement Settings")]
    public float moveSpeed = 3f;   
    public float minY = 0f;       
    public float maxY = 5f;       

    public int maxAmmo = 5;
    private int currentAmmo;
    public TextMeshProUGUI ammoText;

    private float chargeTime = 0f;
    public Slider chargeSlider;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        currentAmmo = maxAmmo;
        UpdateAmmoUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager == null || !gameManager.IsGameActive())
            return;

        float verticalInput = Input.GetAxis("Vertical");
        Vector3 newPos = transform.position + Vector3.up * verticalInput * moveSpeed * Time.deltaTime;

        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);

        transform.position = newPos;

        if (Input.GetKey(KeyCode.Space))
        {
            chargeTime += Time.deltaTime * chargeSpeed;
            launchForce = Mathf.Min(chargeTime, maxLaunchForce);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            chargeTime = 0f;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (currentAmmo > 0)
            {
                GameObject rock = Instantiate(rockPrefab, launchPoint.position, Quaternion.identity);
                Rigidbody rb = rock.GetComponent<Rigidbody>();

                float mass = rb.mass;
                float acceleration = chargeTime * chargeSpeed;
                Vector3 force = launchPoint.forward * (mass * acceleration);

                rb.AddForce(force);

                currentAmmo--;
                UpdateAmmoUI();
            }
            else
            {
                gameManager.LoseStarAndRestartLevel();
            }

            chargeTime = 0f;
        }


        if (chargeSlider != null)
        {
            chargeSlider.value = launchForce / maxLaunchForce;
        }
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = "" + currentAmmo;
        }
    }
}
