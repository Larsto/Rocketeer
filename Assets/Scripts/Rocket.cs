using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

    Rigidbody rigidBody;
    AudioSource audioSource;
    Boolean collitionsAreDisabled = false;

    [SerializeField] float rocketRotation = 10;
    [SerializeField] float rocketThrust = 10;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip levelComplete;
    

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem mainSuccesParticles;
    [SerializeField] ParticleSystem mainDeathParticles;
    [SerializeField] ParticleSystem mainDeatFlameParticle;

    enum State { Alive, Dying, Transcending };
    State state = State.Alive;

    // Use this for initialization
    void Start () {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

    }
	
	// Update is called once per frame
	void Update () {
        if(state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
            
        }

        else if (state == State.Dying)
        {
            mainDeatFlameParticle.Play();
        }
        if (Debug.isDebugBuild)
        {
            RespondToDebugKey();
        }
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * rocketThrust * Time.deltaTime);
        if (!audioSource.isPlaying)

        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }

    private void RespondToRotateInput()
    {
        rigidBody.freezeRotation = true; // take manual control of rotation
        float rorationSpeed = rocketRotation * Time.deltaTime;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.forward * rorationSpeed);
        }

        else if (Input.GetKey(KeyCode.RightArrow))
        {
            
            transform.Rotate(-Vector3.forward * rorationSpeed);
        }
        rigidBody.freezeRotation = false; // resume physics control of rotation
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(state != State.Alive || collitionsAreDisabled)
        {
            return;
        }

        
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                Debug.Log("Collition");
                break;
            case "Finish":
                StartSuccess();
                break;
            default:
                StartDeath();
                break;
            }
        
    }

    private void StartDeath()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(deathSound);
        mainEngineParticles.Stop();
        mainDeathParticles.Play();
        state = State.Dying;
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    private void StartSuccess()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(levelComplete);
        mainEngineParticles.Stop();
        mainSuccesParticles.Play();
        state = State.Transcending;
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }

        SceneManager.LoadScene(nextSceneIndex);
        
    }

    private void RespondToDebugKey()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }

        else if (Input.GetKeyDown(KeyCode.C))
        {
            collitionsAreDisabled = !collitionsAreDisabled;
            Debug.Log(collitionsAreDisabled);
        }
    }
}
