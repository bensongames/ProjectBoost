using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{

    private new Rigidbody rigidbody;
    private AudioSource audioSource;
    [SerializeField] float levelLoadDelay = 2f;
    private int maxLevel = 0;
    private int currentLevel = 0;
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 1000f;
    [SerializeField] AudioClip thrustSound;
    [SerializeField] AudioClip levelUpSound;
    [SerializeField] AudioClip gameOverSound;
    [SerializeField] ParticleSystem thrustParticles;
    [SerializeField] ParticleSystem levelUpParticles;
    [SerializeField] ParticleSystem gameOverParticles;
    bool collisionsDisabled = false;

    private enum State
    {
        OK,
        LevelUp,
        GameOver
    }

    private State state = State.OK;

    // Start is called before the first frame update
     private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        currentLevel = SceneManager.GetActiveScene().buildIndex;
        maxLevel = SceneManager.sceneCountInBuildSettings-1;
    }

    // Update is called once per frame
     private void Update()
    {
        if (state != State.OK) return;
        RespondToThrustInput();
        RespondToRotateInput();
        if (Debug.isDebugBuild) RespondToDebugKeys();
    }
    // OnCollisionEnter is called when one object has begun touching another 
    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.OK || collisionsDisabled) return;
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                // Do nothing
                break;
            case "Finish":
                ChangeLevel(State.LevelUp, levelUpSound);
                break;
            default:
                ChangeLevel(State.GameOver, gameOverSound);
                break;
        }
    }

    private void ChangeLevel(State newState, AudioClip soundToPlay)
    {
        state = newState;
        audioSource.Stop();
        audioSource.PlayOneShot(soundToPlay);
        if (state == State.GameOver)
        {
            currentLevel = 0;
            gameOverParticles.Play();
        }
        else
        {
            if (currentLevel < maxLevel)
                currentLevel += 1;
            else
                currentLevel = 0;
            levelUpParticles.Play();
        }
            
        Invoke("LoadLevel", levelLoadDelay); 
    }

    private void LoadLevel()
    {
        SceneManager.LoadScene(currentLevel);
        state = State.OK;
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space)) // can thrust whilst rotating
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            thrustParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        float thrustThisFrame = mainThrust * Time.deltaTime;
        rigidbody.AddRelativeForce(Vector3.up * thrustThisFrame);
        thrustParticles.Play();
        if (!audioSource.isPlaying) // make sure only plays once
        {
            audioSource.PlayOneShot(thrustSound);
        }
    }

    private void RespondToRotateInput()
    {
        rigidbody.freezeRotation = true; // take manual control of rotation        
        float rotationThisFrame = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {            
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
        rigidbody.freezeRotation = false; // resume Unity rotation control
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        { 
            ChangeLevel(State.LevelUp, levelUpSound); 
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsDisabled = !collisionsDisabled;
        }
    }
        

}
