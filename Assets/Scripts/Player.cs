using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody rb;

    private float currentTime;

    private bool smash, invincible;

    public AudioClip bounceOffClip, deadClip, winClip, destroyClip, invincibleDestroyClip;

    private int currentBrokenStacks, totalStacks;
    public enum PlayerState
    {
        Prepare,
        Playing,
        Died,
        Finish
    }

    [HideInInspector]
    public PlayerState playerState = PlayerState.Prepare;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentBrokenStacks = 0;
    }

    private void Start()
    {
        totalStacks = FindObjectsOfType<StackController>().Length;
    }
    void Update()
    {
        if (playerState == PlayerState.Playing)
        {
            if (Input.GetMouseButtonDown(0))
            {
                smash = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                smash = false;
            }

            if (invincible)
            {
                currentTime -= Time.deltaTime * .35f;
            }
            else
            {
                if (smash)
                    currentTime += Time.deltaTime * .8f;
                else
                    currentTime -= Time.deltaTime * .5f;
            }

            //UI check

            if (currentTime >= 1)
            {
                currentTime = 1;
                invincible = true;
            }
            else if (currentTime <= 0)
            {
                currentTime = 0;
                invincible = false;
            }
        }

        if(playerState == PlayerState.Prepare)
        {
            if (Input.GetMouseButtonDown(0))
                playerState = PlayerState.Playing;
        }
        if (playerState == PlayerState.Finish)
        {
            if (Input.GetMouseButtonDown(0))
                FindObjectOfType<LevelSpawner>().NextLevel();
        }
    }

    private void FixedUpdate()
    {
        if (playerState == PlayerState.Playing)
        {
            if (Input.GetMouseButton(0))
            {
                smash = true;
                rb.velocity = new Vector3(0, -100 * Time.fixedDeltaTime * 7, 0);
            }
        }

        if (rb.velocity.y > 5)
            rb.velocity = new Vector3(rb.velocity.x, 5, rb.velocity.z);
    }

    public void IncreaseBrokenStacks()
    {
        currentBrokenStacks++;
        if (!invincible)
        {
            ScoreManager.instance.AddScore(1);
            SoundManager.instance.PlaySoundFX(destroyClip, 0.5f);
        }
        else
        {
            ScoreManager.instance.AddScore(2);
            SoundManager.instance.PlaySoundFX(invincibleDestroyClip, 0.7f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!smash)
        {
            rb.velocity = new Vector3(0, 50 * Time.deltaTime * 5, 0);
            SoundManager.instance.PlaySoundFX(bounceOffClip, 0.5f);
        }
        else
        {
            if (invincible)
            {
                if (collision.gameObject.tag == "enemy" || 
                    collision.gameObject.tag == "plane")
                {
                    collision.transform.parent.GetComponent<StackController>().ShatterAllParts();
                }
            }
            else
            {
                if (collision.gameObject.tag == "enemy")
                {
                    collision.transform.parent.GetComponent<StackController>().ShatterAllParts();
                }

                if (collision.gameObject.tag == "plane")
                {
                    print("Game Over");
                    ScoreManager.instance.ResetScore();
                    SoundManager.instance.PlaySoundFX(deadClip, 0.5f);
                }
            }
            
        }

        FindObjectOfType<GameUI>().LevelSliderFill(currentBrokenStacks / (float)totalStacks);

        if(collision.gameObject.tag == "Finish" && playerState == PlayerState.Playing)
        {
            playerState = PlayerState.Finish;
            SoundManager.instance.PlaySoundFX(winClip, 0.7f);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!smash || collision.gameObject.tag == "Finish")
        {
            rb.velocity = new Vector3(0, 50 * Time.deltaTime * 5, 0);
        }
    }
}
