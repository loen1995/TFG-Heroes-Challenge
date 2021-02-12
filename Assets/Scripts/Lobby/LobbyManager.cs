using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{

    [SerializeField] private TMP_Text player;
    [SerializeField] private TMP_Text gold;
    [SerializeField] private TMP_Text gems;
    [SerializeField] private TMP_Text trophys;
    [SerializeField] private TMP_Text arenaText;
    [SerializeField] private TMP_Text TrophysArenaText;

    [SerializeField] private Button buttonPlay;
    [SerializeField] private Button buttonShop;
    [SerializeField] private Button buttonArena;
    [SerializeField] private Button buttonHeroes;


    [SerializeField] private Button buttonMagicAura;
    [SerializeField] private Button buttonFireAura;
    [SerializeField] private Button buttonFrostAura;
    [SerializeField] private Button buttonEternalMagicAura;
    [SerializeField] private Button buttonEternalFireAura;
    [SerializeField] private Button buttonEternalFrostAura;

    [SerializeField] private Button none;
    [SerializeField] private Button magicAuraCharacter;
    [SerializeField] private Button fireAuraCharacter;
    [SerializeField] private Button frostAuraCharacter;
    [SerializeField] private Button eternalMagicAuraCharacter;
    [SerializeField] private Button eternalFireAuraCharacter;
    [SerializeField] private Button eternalFrostAuraCharacter;

    [SerializeField] private GameObject panelArena;
    [SerializeField] private GameObject panelHeroes;
    [SerializeField] private GameObject panelShop;
    [SerializeField] private GameObject arenaForest;
    [SerializeField] private GameObject arenaCathedral;
    [SerializeField] private GameObject redArrow;

    private string playerDB;
    private string goldDB;
    private string gemsDB;
    private string auraDB;
    private string heroesDB;
    private bool playerMagicAura;
    private bool playerFireAura;
    private bool playerFrostAura;
    private bool playerEternalMagicAura;
    private bool playerEternalFireAura;
    private bool playerEternalFrostAura;
    private bool playerMagicAuraDB;
    private bool playerFireAuraDB;
    private bool playerFrostAuraDB;
    private bool playerEternalMagicAuraDB;
    private bool playerEternalFireAuraDB;
    private bool playerEternalFrostAuraDB;
    private bool music = false;
    [SerializeField] private string trophysDB;


    public static int arena;
    public static string aura;



    //Firebase
    DatabaseReference myDBRef;

    //Photon
    private string userId;

    // Start is called before the first frame update
    void Start()
    {
        
        //Firebase
        myDBRef = FirebaseDatabase.DefaultInstance.RootReference;

        //Initalize UserId from Photon
        userId = PhotonNetwork.AuthValues.UserId;

        if(userId == null)
        {
            SceneManager.LoadScene("Launcher");
        }

        //Get user Data 
        FirebaseDatabase.DefaultInstance.GetReference("users").GetValueAsync().ContinueWith(task =>
        {
            DataSnapshot snapshot = task.Result;
            this.playerDB = snapshot.Child(userId).Child("username").Value.ToString();
            this.goldDB = snapshot.Child(userId).Child("gold").Value.ToString();
            this.gemsDB = snapshot.Child(userId).Child("gems").Value.ToString();
            this.trophysDB = snapshot.Child(userId).Child("trophys").Value.ToString();
            this.auraDB = snapshot.Child(userId).Child("currentAura").Value.ToString();

            //Select arena

            if (Int32.Parse(this.trophysDB) <= 99)
            {
                arena = 1;
            }

            if (Int32.Parse(this.trophysDB) >= 100)
            {
                arena = 2;
            }
        });

        //Auras sync
        syncData();
    }

    // Update is called once per frame
    void Update()
    {
        this.player.text = this.playerDB;
        this.gold.text = this.goldDB;
        this.gems.text = this.gemsDB;
        this.trophys.text = this.trophysDB;
        aura = this.auraDB;

        this.playerMagicAura = this.playerMagicAuraDB;
        this.playerFireAura =  this.playerFireAuraDB;
        this.playerFrostAura = this.playerFrostAuraDB;
        this.playerEternalMagicAura = this.playerEternalMagicAuraDB;
        this.playerEternalFireAura = this.playerEternalFireAuraDB;
        this.playerEternalFrostAura = this.playerEternalFrostAuraDB;

        //Arenas
        switch (arena)
        {
            case 1:
                arenaText.text = "Arena 1: FOREST";
                TrophysArenaText.text = "0 - 99";
                arenaForest.SetActive(true);
                arenaCathedral.SetActive(false);
                if (!music)
                {
                    FindObjectOfType<AudioManager>().Play("Forest");
                    FindObjectOfType<AudioManager>().Play("Waterfall");
                    music = true;
                }
                break;
            case 2:
                arenaText.text = "Arena 2: CATHEDRAL";
                TrophysArenaText.text = "100 - 199";
                arenaCathedral.SetActive(true);
                arenaForest.SetActive(false);
                
                if (!music)
                {
                    FindObjectOfType<AudioManager>().Play("Cathedral");
                    music = true;
                }
                break;
        }


        //Auras
        if (aura == "None")
        {
            none.gameObject.GetComponent<Image>().color = new Color(0, 255, 0, 255);
        }
        else
        {
            none.gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 255);
        }
        
        if (this.playerMagicAura)
        {
            buttonMagicAura.gameObject.GetComponent<Image>().color = new Color(0, 255, 0, 255);
            buttonMagicAura.interactable = false;

            if(aura != "MagicAuraCharacter")
            {
                magicAuraCharacter.gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 255);
                magicAuraCharacter.interactable = true;
            }
            else
            {
                magicAuraCharacter.gameObject.GetComponent<Image>().color = new Color(0, 255, 0, 255);
            }
        }
        else
        {
            magicAuraCharacter.gameObject.GetComponent<Image>().color = new Color(255, 0, 0, 255);
            magicAuraCharacter.interactable = false;
        }

        if (this.playerFireAura)
        {
            buttonFireAura.gameObject.GetComponent<Image>().color = new Color(0, 255, 0, 255);
            buttonFireAura.interactable = false;

            if (aura != "FireAuraCharacter")
            {
                fireAuraCharacter.gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 255);
                fireAuraCharacter.interactable = true;
            }
            else
            {
                fireAuraCharacter.gameObject.GetComponent<Image>().color = new Color(0, 255, 0, 255);
            }
        }
        else
        {
            fireAuraCharacter.gameObject.GetComponent<Image>().color = new Color(255, 0, 0, 255);
            fireAuraCharacter.interactable = false;
        }
        
        if (this.playerFrostAura)
        {
            buttonFrostAura.gameObject.GetComponent<Image>().color = new Color(0, 255, 0, 255);
            buttonFrostAura.interactable = false;

            if (aura != "FrostAuraCharacter")
            {
                frostAuraCharacter.gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 255);
                frostAuraCharacter.interactable = true;
            }
            else
            {
                frostAuraCharacter.gameObject.GetComponent<Image>().color = new Color(0, 255, 0, 255);
            }
        }
        else
        {
            frostAuraCharacter.gameObject.GetComponent<Image>().color = new Color(255, 0, 0, 255);
            frostAuraCharacter.interactable = false;
        }

        if (this.playerEternalMagicAura)
        {
            buttonEternalMagicAura.gameObject.GetComponent<Image>().color = new Color(0, 255, 0, 255);
            buttonEternalMagicAura.interactable = false;
            if (aura != "EternalMagicAuraCharacter")
            {
                eternalMagicAuraCharacter.gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 255);
                eternalMagicAuraCharacter.interactable = true;
            }
            else
            {
                eternalMagicAuraCharacter.gameObject.GetComponent<Image>().color = new Color(0, 255, 0, 255);
            }
        } else
        {
            eternalMagicAuraCharacter.gameObject.GetComponent<Image>().color = new Color(255, 0, 0, 255);
            eternalMagicAuraCharacter.interactable = false;
        }
        

        if (this.playerEternalFireAura)
        {
            buttonEternalFireAura.gameObject.GetComponent<Image>().color = new Color(0, 255, 0, 255);
            buttonEternalFireAura.interactable = false;
            if(aura != "EternalFireAuraCharacter")
            {
                eternalFireAuraCharacter.gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 255);
                eternalFireAuraCharacter.interactable = true;
            }
            else
            {
                eternalFireAuraCharacter.gameObject.GetComponent<Image>().color = new Color(0, 255, 0, 255);
            }
        }
        else
        {
            eternalFireAuraCharacter.gameObject.GetComponent<Image>().color = new Color(255, 0, 0, 255);
            eternalFireAuraCharacter.interactable = false;
        }
        
        if (this.playerEternalFrostAura)
        {
            buttonEternalFrostAura.gameObject.GetComponent<Image>().color = new Color(0, 255, 0, 255);
            buttonEternalFrostAura.interactable = false;
            if(aura != "EternalFrostAuraCharacter")
            {
                eternalFrostAuraCharacter.gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 255);
                eternalFrostAuraCharacter.interactable = true;
            }
            else
            {
                eternalFrostAuraCharacter.gameObject.GetComponent<Image>().color = new Color(0, 255, 0, 255);
            }
            
        }
        else
        {
            eternalFrostAuraCharacter.gameObject.GetComponent<Image>().color = new Color(255, 0, 0, 255);
            eternalFrostAuraCharacter.interactable = false;
        }
        
    }

    public void syncData()
    {
        //Get shop Data 
        FirebaseDatabase.DefaultInstance.GetReference("shop").GetValueAsync().ContinueWith(task =>
        {
            DataSnapshot snapshot = task.Result;
            this.playerMagicAuraDB = Boolean.Parse(snapshot.Child(userId).Child("MagicAura").Value.ToString());
            this.playerFireAuraDB = Boolean.Parse(snapshot.Child(userId).Child("FireAura").Value.ToString());
            this.playerFrostAuraDB = Boolean.Parse(snapshot.Child(userId).Child("FrostAura").Value.ToString());
            this.playerEternalMagicAuraDB = Boolean.Parse(snapshot.Child(userId).Child("EternalMagicAura").Value.ToString());
            this.playerEternalFireAuraDB = Boolean.Parse(snapshot.Child(userId).Child("EternalFireAura").Value.ToString());
            this.playerEternalFrostAuraDB = Boolean.Parse(snapshot.Child(userId).Child("EternalFrostAura").Value.ToString());
        });
    }

    public void signOut()
    {
        Firebase.Auth.FirebaseAuth.DefaultInstance.SignOut();
        arena = 0;
        SceneManager.LoadScene("Launcher");
    }

    public void selectArena()
    {
        buttonPlay.gameObject.SetActive(false);
        buttonShop.gameObject.SetActive(false);
        buttonHeroes.gameObject.SetActive(false);
        buttonArena.gameObject.SetActive(false);
        panelArena.SetActive(true);
        FindObjectOfType<AudioManager>().Play("Menus");
    }

    public void selectArena1()
    {
        FindObjectOfType<AudioManager>().Play("Menus");
        FindObjectOfType<AudioManager>().Stop("Cathedral");
        buttonPlay.gameObject.SetActive(true);
        buttonShop.gameObject.SetActive(true);
        buttonHeroes.gameObject.SetActive(true);
        buttonArena.gameObject.SetActive(true);
        buttonArena.GetComponent<Image>().sprite = Resources.Load<Sprite>("forest");
        buttonPlay.interactable = true;
        buttonPlay.gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 255);
        redArrow.SetActive(false);
        panelArena.SetActive(false);
        arena = 1;
        music = false;
    }

    public void selectArena2()
    {
        FindObjectOfType<AudioManager>().Play("Menus");
        FindObjectOfType<AudioManager>().Stop("Forest");
        FindObjectOfType<AudioManager>().Stop("Waterfall");
        buttonPlay.gameObject.SetActive(true);
        buttonShop.gameObject.SetActive(true);
        buttonHeroes.gameObject.SetActive(true);
        buttonArena.gameObject.SetActive(true);
        buttonArena.GetComponent<Image>().sprite = Resources.Load<Sprite>("castle");
        panelArena.SetActive(false);
        arena = 2;

        music = false;
        if (Int32.Parse(this.trophys.text) < 100)
        {
            redArrow.SetActive(true);
            buttonPlay.interactable = false;
            buttonPlay.gameObject.GetComponent<Image>().color = new Color(255, 0, 0, 255);
        }
        else
        {
            redArrow.SetActive(false);
            buttonPlay.interactable = true;
            buttonPlay.gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 255);
        }
    }

    public void openShop()
    {
        if (panelShop.activeSelf)
        {
            closeShop();
            return;
        }
        FindObjectOfType<AudioManager>().Play("Menus");
        buttonPlay.gameObject.SetActive(false);
        buttonArena.gameObject.SetActive(false);
        buttonHeroes.gameObject.SetActive(false);
        panelShop.SetActive(true);
    }

    public void closeShop()
    {
        FindObjectOfType<AudioManager>().Play("Menus");
        panelShop.SetActive(false); 
        buttonPlay.gameObject.SetActive(true);
        buttonHeroes.gameObject.SetActive(true);
        buttonArena.gameObject.SetActive(true);
    }

    public void buyAura(String nameAura)
    {
        bool opSuccess = false;
        String price = GameObject.Find(nameAura).GetComponent<TMP_Text>().text;
        if (nameAura.Contains("Eternal")){
            if(Int32.Parse(this.gemsDB) - Int32.Parse(price) >= 0)
            {
                FindObjectOfType<AudioManager>().Play("payGems");
                int gems2DB = Int32.Parse(this.gemsDB) - Int32.Parse(price);
                this.gemsDB = gems2DB.ToString();
                myDBRef.Child("users").Child(userId).Child("gems").SetValueAsync(gems2DB);
                opSuccess = true;
            }
        }
        else
        {
            if(Int32.Parse(this.goldDB) - Int32.Parse(price) >= 0)
            {
                FindObjectOfType<AudioManager>().Play("payCoins");
                int gold2DB = Int32.Parse(this.goldDB) - Int32.Parse(price);
                this.goldDB = gold2DB.ToString();
                myDBRef.Child("users").Child(userId).Child("gold").SetValueAsync(gold2DB);
                opSuccess = true;
            }
        }

        if (opSuccess)
        {
            //This function will create all the path even if it doesn't exist
            myDBRef.Child("shop").Child(userId).Child(nameAura).SetValueAsync("True");
            syncData();
        }
        else
        {
            FindObjectOfType<AudioManager>().Play("Wrong");
        }
    }

    public void openHeroes()
    {
        FindObjectOfType<AudioManager>().Play("Menus");
        panelHeroes.SetActive(true);
        buttonPlay.gameObject.SetActive(false);
        buttonShop.gameObject.SetActive(false);
        buttonArena.gameObject.SetActive(false);
        buttonHeroes.gameObject.SetActive(false);
    }
  
    public void selectAura(String nameAura)
    {
        FindObjectOfType<AudioManager>().Play("Menus");
        FirebaseDatabase.DefaultInstance.GetReference("/users/" + userId).Child("currentAura").SetValueAsync(nameAura);
        syncData();
        panelHeroes.SetActive(false);
        buttonPlay.gameObject.SetActive(true);
        buttonShop.gameObject.SetActive(true);
        buttonArena.gameObject.SetActive(true);
        buttonHeroes.gameObject.SetActive(true);
        aura = nameAura;
        this.auraDB = aura;
    }
}
