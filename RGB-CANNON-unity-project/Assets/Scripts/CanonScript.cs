using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonScript : MonoBehaviour {

    public GameObject canonNozzle;
    public float rotationSpeed = 5f;
    Vector3 pos;
    Vector3 dir;

    bool isRed;
    bool isGreen;
    bool isBlue;

    float targetAngle = 0;
    float deltaAngle = 0;
    float newAngle = 0;
    float currentAngle;
    public float maxDegreesDelta;

    Colors color;
    public float hitDamage;
    public float maxCooldownTime = 3;
    string fireColor;

    public float lazerRange;
    LineRenderer laser;
    ParticleSystem lazerParticleSystem;
    RaycastHit hit;
    bool isShowingLaser = false;

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

    public Material[] lazerMaterialsRGBYCMW;

    StatsLoggerScript _GM;
    AudioManagerScript audioManager;

    bool decayIsPlaying;
    bool soundIsPlaying;

    string fireRedKeyName;

    // Use this for initialization
    void Start () {
        _GM = GameObject.FindGameObjectWithTag("Stats").GetComponent<StatsLoggerScript>();
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManagerScript>();
        pos = Camera.main.WorldToScreenPoint(transform.position);
        laser = GetComponentInChildren<LineRenderer>(true);
        lazerParticleSystem = GetComponentInChildren<ParticleSystem>(true);
        laser.enabled = false;

        CheckKeyBinding();

    }
	
	// Update is called once per frame
	void Update () {

        if (_GM.gameIsPlaying || _GM.overrideCanonControl)
        { 
            MoveNozzle();

            if (Input.GetButtonDown(fireRedKeyName) || Input.GetButtonDown("fireGreen") || Input.GetButtonDown("fireBlue"))
            {
                audioManager.Stop("LSDecay");
                decayIsPlaying = false;
                lazerParticleSystem.Play();
                if (!soundIsPlaying)
                {
                    audioManager.Play("LSAttack");
                    audioManager.PlayDelayed("LSSustain", "LSAttack");
                    soundIsPlaying = true;
                }
            }

            if ((Input.GetButton(fireRedKeyName) || Input.GetButton("fireGreen") || Input.GetButton("fireBlue"))/* & activeTime < maxActiveTime*/ )
            {
                if (Input.GetButton(fireRedKeyName))
                {
                    isRed = true;
                    color = Colors.red;
                }

                if (Input.GetButton("fireGreen"))
                {
                    isGreen = true;
                    if (isRed)
                    {
                        color = Colors.yellow;
                    }
                    else
                    {
                        color = Colors.green;
                    }
                }

                if (Input.GetButton("fireBlue"))
                {
                    isBlue = true;
                    color = Colors.blue;
                    if (isRed)
                    {
                        color = Colors.magenta;
                        if (isGreen)
                        {
                            color = Colors.white;
                        }
                    }

                    if (isGreen & !isRed)
                    {
                        color = Colors.cyan;
                    }
                }
            
                Shoot();

                isRed = false;
                isGreen = false;
                isBlue = false;

            }
            
            if ((!Input.GetButton(fireRedKeyName) & !Input.GetButton("fireGreen") & !Input.GetButton("fireBlue")) & !decayIsPlaying & soundIsPlaying)
            {
                audioManager.Play("LSDecay");
                decayIsPlaying = true;
            }

            if (!Input.GetButton(fireRedKeyName) & !Input.GetButton("fireGreen") & !Input.GetButton("fireBlue"))
            {
                lazerParticleSystem.Stop();
                audioManager.Stop("LSSustain");
                audioManager.Stop("LSAttack");
                soundIsPlaying = false;
            }

            

        }

        if (!_GM.gameIsPlaying)
        {
            lazerParticleSystem.Stop();
            CheckKeyBinding();
        }

    }

    void MoveNozzle()
    { 
        dir = Input.mousePosition - pos; //calculate direction vector
        targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90; //calculate targetAngle based on vector
        //Debug.Log(targetAngle);

        //keep targetAngle within range(=2*maxDegreeDelta);
        if (targetAngle > maxDegreesDelta || targetAngle < -180) // nozzle completely to left
        {
            targetAngle = maxDegreesDelta;
        }
        else if (targetAngle < -maxDegreesDelta)  // nozzle completely to right
        {
            targetAngle = -maxDegreesDelta;
        }

        if (currentAngle != targetAngle) //if rotation is needed
        {
            deltaAngle = rotationSpeed * Time.deltaTime; //calculate maxdeltaAngle
            
            if (Mathf.Abs(targetAngle-currentAngle) < deltaAngle) //if deltaAngle is more than the difference to targetAngle
            {
                deltaAngle = Mathf.Abs(targetAngle - currentAngle); //set deltaAngle to this difference
            }

            if (targetAngle > currentAngle) //if rotation is positive (clockwise)
            {
                newAngle = currentAngle + deltaAngle;
            }
            else //else rotation is negative (ccw)
            {
                newAngle = currentAngle - deltaAngle;
            }
            
            canonNozzle.transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(newAngle, Vector3.forward), maxDegreesDelta); //do the rotation
        }

        //Debug.Log(newAngle + " ; " + rotationSpeed);
        currentAngle = newAngle;
    }

    void Shoot()
    {
        //Debug.Log("Firing" + color);
        dir = canonNozzle.transform.rotation * Vector3.up;
        int layerMask = 1 << 1;
        layerMask = ~layerMask;
        StartCoroutine("ShowLaser");
        //Debug.Log(laser.enabled);

        if (Physics.Raycast(transform.position, dir, out hit, lazerRange, layerMask))
        {
            Debug.DrawRay(transform.position, dir * hit.distance, Color.blue);
            laser.SetPosition(1, new Vector3(0, 0, hit.distance));
            //Debug.Log("Did Hit, "+ hit.collider.gameObject.name);
            if (hit.collider.gameObject.tag == "Environment")
            {
                //Do nothing
            }
            else
            {
                EnemyScript enemyScript = hit.transform.GetComponent<EnemyScript>();
                enemyScript.TakeDamage(hitDamage, isRed, isGreen, isBlue);
            }
        }
        else
        {
            Debug.DrawRay(transform.position, dir * 1000, Color.white);
            laser.SetPosition(1, new Vector3(0, 0, 70));
            //Debug.Log("Did not Hit");
        }
    }

    IEnumerator ShowLaser()
    {
        if (isShowingLaser)
        {
           yield return null;
        }
        isShowingLaser = true;
        laser.material = lazerMaterialsRGBYCMW[(int)color];
        lazerParticleSystem.gameObject.transform.position = hit.point;
        lazerParticleSystem.gameObject.transform.rotation = Quaternion.LookRotation(hit.normal);
        lazerParticleSystem.gameObject.GetComponent<ParticleSystemRenderer>().material = lazerMaterialsRGBYCMW[(int)color];
        laser.enabled = true;

        yield return new WaitForEndOfFrame();

        ResetLaser();

        isShowingLaser = false;
    }

    void ResetLaser()
    {
        laser.enabled = false;
    }

    void CheckKeyBinding()
    {
        if (PlayerStats.UseQwerty)
        {
            fireRedKeyName = "fireRedQwerty";
        }
        else
        {
            fireRedKeyName = "fireRed";
        }
    }
}
