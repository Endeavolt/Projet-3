﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseScope : MonoBehaviour
{
    public Material line;
    public GameObject bullet;
    public GameObject[] Ambout;
    public int numberAmbout;
    // public GameObject spawn;
    private EnnemiStock ennemiStock;
    [HideInInspector] public Vector3 direction;
    [HideInInspector] public Vector3 directionManette;
    [HideInInspector] public GameObject instanceBullet;
    private LineRenderer lineRenderer;
    private bool returnLine;
    private bool destructBool;
    private Vector3 ballPos;
    private Vector3 returnPos;
    private float distanceReturn;
    private Vector3 dirReturn;
    public float returnSpeed = 50;
    [Header("Tirer Sound")]
    [FMODUnity.EventRef]
    public string contact;
    private FMOD.Studio.EventInstance contactSound;
    public float volume = 10;
    [Header("Retour Sound")]
    [FMODUnity.EventRef]
    public string returnSound;
    private FMOD.Studio.EventInstance returnEvent;
    public float returnVolume = 10;
    [Header("Carateristique Bullet")]
    public float distance = 75;
    public float timerOfBullet = 0.5f;
    public float speedOfBullet;
    public float timeBetweenShoot=0.4f;
    public bool distanceDestruct;
    private float _timerOfBullet;
    private GameObject meshBullet;
    // Start is called before the first frame update
    void Start()
    {

        ennemiStock = GetComponent<EnnemiStock>();
        lineRenderer = GetComponent<LineRenderer>();
        contactSound = FMODUnity.RuntimeManager.CreateInstance(contact);
        contactSound.setVolume(volume);
        returnEvent = FMODUnity.RuntimeManager.CreateInstance(returnSound);
        returnEvent.setVolume(returnVolume);
        speedOfBullet = distance / timerOfBullet;
        returnSpeed = distance / timeBetweenShoot;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (numberAmbout == Ambout.Length - 1)
            {
                numberAmbout = 0;
            }
            else
            {
                numberAmbout++;
            }
        }


        direction = DirectionSouris();
        directionManette = DirectionManette();
        if (directionManette != Vector3.zero)
        {
            direction = Vector3.zero;
        }
        float input = Input.GetAxis("Attract1");
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || input != 0)
        {

            if (ennemiStock.ennemiStock == null && instanceBullet == null)
            {

                instanceBullet = Instantiate(bullet, transform.position + (direction + directionManette).normalized, Quaternion.identity);
                meshBullet = Instantiate(Ambout[numberAmbout], instanceBullet.transform.position, Quaternion.identity, instanceBullet.transform);
                float angle = Vector3.SignedAngle(transform.forward, (direction + directionManette).normalized, transform.up);

                Vector3 eulers = new Vector3(Ambout[numberAmbout].transform.eulerAngles.x, angle, Ambout[numberAmbout].transform.eulerAngles.z);
                meshBullet.transform.localRotation = Quaternion.Euler(eulers);

                _timerOfBullet = 0;


                Projectils projectils = instanceBullet.GetComponent<Projectils>();
                projectils.dir = (direction + directionManette).normalized;
                projectils.player = gameObject;
                projectils.lineRenderer = lineRenderer;
                projectils.speed = speedOfBullet;
                projectils.moveAlone = GetComponent<PlayerMoveAlone>();
                contactSound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

                contactSound.start();
                
            }

        }
        if (ennemiStock.ennemiStock == null && instanceBullet != null)
        {

            Projectils projectils = instanceBullet.GetComponent<Projectils>();
            if (!distanceDestruct)
            {
                if (_timerOfBullet > timerOfBullet)
                {
                    if (!projectils.returnBall)
                    {
                        projectils.returnBall = true;
                        projectils.dir = -projectils.dir;
                        projectils.speed = returnSpeed;


                    }
                    projectils.dir = transform.position - instanceBullet.transform.position;
                    float angle = Vector3.SignedAngle(transform.forward, projectils.dir, transform.up);
                    Vector3 eulers = new Vector3(meshBullet.transform.eulerAngles.x, angle, meshBullet.transform.eulerAngles.z);
                    meshBullet.transform.localRotation = Quaternion.Euler(eulers);

                }
                else
                {
                    _timerOfBullet += Time.deltaTime;
                }
            }
            else
            {
                if (Vector3.Distance(transform.position, instanceBullet.transform.position) >= distance && !projectils.returnBall)
                {
                    projectils.returnBall = true;
                    projectils.dir = -projectils.dir;
                    projectils.speed = returnSpeed;
                }
            }
            returnLine = true;
            destructBool = false;

            ballPos = instanceBullet.transform.position;

        }

        if (ennemiStock.ennemiStock == null && instanceBullet == null)
        //Return de line renderer après le tir;
        {
            if (returnLine)
            {
                if (!destructBool)
                {
                    returnPos = ballPos;
                    returnEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

                    returnEvent.start();
                    destructBool = true;
                }


                returnLine = false;
                if (lineRenderer.enabled)
                {
                    lineRenderer.enabled = false;
                }


            }
        }
        if (ennemiStock.ennemiStock == null && instanceBullet != null)
        {
            Projectils projectils = instanceBullet.GetComponent<Projectils>();
            if (projectils.returnBall)
            {
                projectils.dir = -(projectils.transform.position - transform.position);
                if (Vector3.Distance(transform.position, instanceBullet.transform.position) < 5)
                {
                    Destroy(instanceBullet);
                }
            }
        }
    }

    private void OnRenderObject()
    {

        if (ennemiStock.ennemiStock == null && instanceBullet == null)
        {
            GL.Begin(GL.LINES);
            line.SetPass(0);
            GL.Color(Color.red);
            GL.Vertex(transform.position);
            GL.Vertex(transform.position + (direction + directionManette).normalized * distance);
            GL.End();
        }

    }

    private void OnDrawGizmos()
    {
        GL.Begin(GL.LINES);
        line.SetPass(0);
        GL.Color(Color.red);
        GL.Vertex(transform.position);
        GL.Vertex(transform.position + (direction + directionManette).normalized * distance);
        GL.End();
    }
    private Vector3 DirectionSouris()
    {
        Ray camera = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, Vector3.zero);
        float rauEnter;

        if (ground.Raycast(camera, out rauEnter))
        {
            Vector3 pointToLook = camera.GetPoint(rauEnter);
            Vector3 posPlayer = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 dir = pointToLook - posPlayer;



            return dir;
        }
        else
        {
            return Vector3.zero;
        }
    }
    private Vector3 DirectionManette()
    {
        float aimHorizontal = Input.GetAxis("AimHorizontal1");
        float aimVertical = -Input.GetAxis("AimVertical1");

        Vector3 dir = new Vector3(aimHorizontal, 0, aimVertical);
        return dir.normalized;
    }
}
