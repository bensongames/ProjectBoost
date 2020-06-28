using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{

    private new Rigidbody rigidbody;
    private AudioSource audioSource;
    [SerializeField] int maxLevel = 1;
    [SerializeField] int currentLevel = 0;
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 1000f;
    [SerializeField] AudioClip thrustSound;
    [SerializeField] AudioClip winSound;
    [SerializeField] AudioClip looseSound;
    [SerializeField] ParticleSystem thrustParticles;
    [SerializeField] ParticleSystem winParticles;
    [SerializeField] ParticleSystem looseParticles;

    private enum State
    {
        OK,
        LevelUp,
        GameOver
    }

    private State state = State.OK;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state != State.OK) return;
        RespondToThrustInput();
        RespondToRotateInput();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.OK) return;
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                // Do nothing
                break;
            case "Finish":
                ChangeLevel(State.LevelUp, winSound);
                break;
            default:
                ChangeLevel(State.GameOver, looseSound);
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
            looseParticles.Play();
        } 
            
        else if(currentLevel < maxLevel)
        {
            currentLevel += 1;
            winParticles.Play();
        }
            
        Invoke("LoadLevel", 1f); //parameterise time
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
        if (!audioSource.isPlaying) // make sure only plays once
        {
            audioSource.PlayOneShot(thrustSound);
        }
        thrustParticles.Play();
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

}
