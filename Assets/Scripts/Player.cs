﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private Rigidbody rb;

    private float currentTime;

    private bool smash, invincible;

    public AudioClip bounceOffClip, deadClip, winClip, destroyClip, invincibleDestroyClip;

    private int currentBrokenStacks, totalStacks;

    public GameObject invincibleObj;
    public Image invincibleFill;
    public GameObject fireEffect, winEffect, splashEffect;

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
                if (!fireEffect.activeInHierarchy)
                    fireEffect.SetActive(true);
            }
            else
            {
                if (fireEffect.activeInHierarchy)
                    fireEffect.SetActive(false);

                if (smash)
                    currentTime += Time.deltaTime * .8f;
                else
                    currentTime -= Time.deltaTime * .5f;
            }

            if (currentTime >= 0.3f || invincibleFill.color == Color.red)
                invincibleObj.SetActive(true);
            else
                invincibleObj.SetActive(false);

            if (currentTime >= 1)
            {
                currentTime = 1;
                invincible = true;
                invincibleFill.color = Color.red;
            }
            else if (currentTime <= 0)
            {
                currentTime = 0;
                invincible = false;
                invincibleFill.color = Color.white;
            }

            if (invincibleObj.activeInHierarchy)
                invincibleFill.fillAmount = currentTime / 1;
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

            if(collision.gameObject.tag != "Finish")
            {
                GameObject splash = Instantiate(splashEffect);
                splash.transform.SetParent(collision.transform);
                splash.transform.localEulerAngles = new Vector3(90, Random.Range(0, 359), 0);
                float randomScale = Random.Range(0.18f, 0.25f);
                splash.transform.localScale = new Vector3(randomScale, randomScale, 1);
                splash.transform.position = new Vector3(transform.position.x, transform.position.y - 0.22f, transform.position.z);
                splash.GetComponent<SpriteRenderer>().color = transform.GetChild(0).GetComponent<MeshRenderer>().material.color;
            }
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
                    playerState = PlayerState.Died;
                    rb.isKinematic = true;
                    transform.GetChild(0).gameObject.SetActive(false);
                    //ScoreManager.instance.ResetScore();
                    SoundManager.instance.PlaySoundFX(deadClip, 0.5f);
                }
            }
            
        }

        FindObjectOfType<GameUI>().LevelSliderFill(currentBrokenStacks / (float)totalStacks);

        if(collision.gameObject.tag == "Finish" && playerState == PlayerState.Playing)
        {
            playerState = PlayerState.Finish;
            SoundManager.instance.PlaySoundFX(winClip, 0.7f);
            GameObject win = Instantiate(winEffect);
            win.transform.SetParent(Camera.main.transform);
            win.transform.localPosition = Vector3.up * 1.5f;
            win.transform.eulerAngles = Vector3.zero;
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
