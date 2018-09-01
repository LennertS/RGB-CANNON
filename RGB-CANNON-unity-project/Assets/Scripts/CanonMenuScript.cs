using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonMenuScript : MonoBehaviour {

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
    AudioManagerScript audioManager;

    // Use this for initialization
    void Start () {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManagerScript>();
        pos = Camera.main.WorldToScreenPoint(transform.position);
        laser = GetComponentInChildren<LineRenderer>(true);
        lazerParticleSystem = GetComponentInChildren<ParticleSystem>(true);
        laser.enabled = false;

    }
	
	// Update is called once per frame
	void Update () {

        
        MoveNozzle();

        if (Input.GetMouseButtonDown(0))
        {
            audioManager.Stop("LSDecay");
            lazerParticleSystem.Play();
            audioManager.Play("LSAttack");
            audioManager.PlayDelayed("LSSustain", "LSAttack");
        }

        if (Input.GetMouseButton(0))
        {
            color = (Colors)(int)Random.Range((int)Colors.red, (int)Colors.white);
            Shoot();            
        }

        if (Input.GetMouseButtonUp(0))
        {
            audioManager.Play("LSDecay");
        }

        if (!Input.GetMouseButton(0))
        {
            lazerParticleSystem.Stop();
            audioManager.Stop("LSSustain");
            audioManager.Stop("LSAttack");
        }

    }

    void MoveNozzle()
    { 
        dir = Input.mousePosition - pos; //calculate direction vector
        targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90; //calculate targetAngle based on vector
        //Debug.Log(targetAngle);

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
}
