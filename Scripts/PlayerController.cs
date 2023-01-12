using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    private Animator playerAnim;
    private AudioSource playerAudio;

    private Vector3 startPos = new Vector3(-5, 0, 0);
    private Vector3 endPos = new Vector3(0, 0, 0);
    private float timeToLerp = 1f;
    private float lerpSpeed = 0;

    public ParticleSystem explosionParticle;
    public ParticleSystem dirtParticle;
    public AudioClip jumpSound;
    public AudioClip crashSound;

    public float jumpForce = 10;
    public float gravityModifier;
    public bool isOnGround = true;
    public bool isDoubleJump = false;
    public bool gameOver;
    public int score;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();
        Physics.gravity *= gravityModifier;
        score= 0;
        playerAnim.SetBool("Static_b", true);
        playerAnim.SetFloat("Speed_f", 0.3f);
        StartCoroutine(PlayIntro());
        StartCoroutine(CountDown());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)  && !gameOver)
        {
            if(isOnGround)
            {
                isOnGround = false;
                isDoubleJump= false;
                JumpFunc();
            }
            else if (!isDoubleJump && !isOnGround)
            {
                JumpFunc();
                isDoubleJump= true;
            }
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            playerAnim.speed = 2.5f;
        }
        else
        {
            playerAnim.speed = 1.5f;
        }
    }

    private void JumpFunc()
    {
            dirtParticle.Stop();
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            playerAnim.SetTrigger("Jump_trig");
            playerAudio.PlayOneShot(jumpSound, 1.0f);

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
            dirtParticle.Play();
        }
        else if(collision.gameObject.CompareTag("Obstacle")){
            Debug.Log("GAME OVER");
            gameOver = true;
            playerAnim.SetBool("Death_b", true);
            playerAnim.SetInteger("DeathType_int", 1);
            explosionParticle.Play(); 
            dirtParticle.Stop();
            playerAudio.PlayOneShot(crashSound,1.0f);
        }
    }

    IEnumerator PlayIntro()
    {
        gameOver = true;

        dirtParticle.Stop();

        // Lerping the player between two positions frame by frame
        while (lerpSpeed < timeToLerp)
        {
            transform.position = Vector3.Lerp(startPos, endPos, lerpSpeed / timeToLerp);
            
            Debug.Log(lerpSpeed);

            lerpSpeed += Time.deltaTime;


            yield return null;
        }

        // Snapping the player to the end position when close enough
        transform.position = endPos;

        dirtParticle.Play();

        gameOver = false;

        // Change from Walking animation to Running animation
        playerAnim.SetFloat("Speed_f", 1f);
    }

    IEnumerator CountDown()
    {
        Debug.Log("start");
        yield return new WaitForSeconds(5);
        Debug.Log("end");
    }

}
