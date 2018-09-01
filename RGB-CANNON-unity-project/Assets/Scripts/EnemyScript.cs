using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour {

    Colors color;
    Material lazerMaterial;
    Material particleMaterial;
    public float hitPoints;
    public float speed;
    public int xpPoints;

    Light pointLight;
    public int numOfOnlyPrimaryColoredEnemies;

    bool hasRed;
    bool hasGreen;
    bool hasBlue;

    enum Colors
    {
        red,
        green,
        blue,
        yellow,
        cyan,
        magenta,
        white
    }
        
    MeshRenderer meshRenderer;
    public Rigidbody rb;

    StatsLoggerScript _GM;
    AudioManagerScript audioManager;

    public ParticleSystem explosion;
    public SpawnSystemScript spawnSystem;

    public Material[] lazerMaterialsRGBYCMW;
    public Material[] particleMaterialsRGBYCMW;

    // Use this for initialization
    void Start () {
        _GM = GameObject.FindGameObjectWithTag("Stats").GetComponent<StatsLoggerScript>();
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManagerScript>();
        spawnSystem = GameObject.FindGameObjectWithTag("SpawnSystem").GetComponent<SpawnSystemScript>();
        Color lightColor = Color.gray;

        pointLight = GetComponentInChildren<Light>();
        meshRenderer = GetComponent<MeshRenderer>();
        if (spawnSystem.GetNumOfSpawnedEnemies < numOfOnlyPrimaryColoredEnemies)
        {
            color = (Colors)Random.Range(0, 3);
        }
        else if (spawnSystem.GetNumOfSpawnedEnemies == numOfOnlyPrimaryColoredEnemies)
        {
            color = (Colors)Random.Range(3, 6);
            _GM.firstSecondarySpawned = true;
        }
        else
        {

            color = (Colors)Random.Range(0, 7);
        }

        //Debug.Log("Enemy Color is " + color);

        switch (color)
        {
            case Colors.red:

                hasRed = true;
                lightColor = Color.red;

                break;
            case Colors.green:

                hasGreen = true;
                lightColor = Color.green;


                break;
            case Colors.blue:

                hasBlue = true;
                lightColor = Color.blue;
                break;
            case Colors.yellow:

                hasGreen = true;
                hasRed = true;
                lightColor = Color.yellow;

                break;
            case Colors.cyan:

                hasBlue = true;
                hasGreen = true;
                lightColor = Color.cyan;

                break;
            case Colors.magenta:

                hasBlue = true;
                hasRed = true;
                lightColor = Color.magenta;

                break;
            case Colors.white:

                hasBlue = true;
                hasRed = true;
                hasGreen = true;
                lightColor = Color.white;

                break;
            default:
                break;
        }

        lazerMaterial = lazerMaterialsRGBYCMW[(int)color];
        particleMaterial = particleMaterialsRGBYCMW[(int)color];
        //Debug.Log("Setting lightcolor to " + lightColor);
        pointLight.color = lightColor;

        meshRenderer.material = lazerMaterial;

    }
	
	// Update is called once per frame
	void Update () {
        if (_GM.gameIsPlaying)
        {
            MoveEnemy();

            if (transform.position.y < -2.5f)
            {
                _GM.GameOver();
            }
        }

        if (!_GM.gameIsPlaying)
        {
            rb.velocity = Vector3.zero;
        }
    }

    void MoveEnemy()
    {
        //Debug.Log("Moving Enemy . . .");
        Vector3 dirNormalized = Vector3.down.normalized;
        rb.velocity = dirNormalized * speed * Time.fixedDeltaTime;
    }

    public void TakeDamage(float amount, bool canonRed, bool canonGreen, bool canonBlue)
    {
        if (hasRed == canonRed && hasGreen == canonGreen && hasBlue == canonBlue)
        { 
            hitPoints -= amount;
            //enable particleEffect Coroutine
            if (hitPoints <= 0)
            {
                
                Die();

            }
        }
    }

    void Die()
    {
        _GM.AddXP(xpPoints);
        ParticleSystem newExplosion = Instantiate(explosion, transform);
        newExplosion.GetComponent<ParticleSystemRenderer>().material = particleMaterial;
        newExplosion.transform.parent = null;
        newExplosion.Play();
        audioManager.Play("Explosion1");
        DieSilently();
    }

    void DieSilently()
    {
        Destroy(gameObject);
    }
    
    
}
