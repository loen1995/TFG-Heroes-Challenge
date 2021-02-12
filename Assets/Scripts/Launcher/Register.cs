using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Database;
using Firebase.Extensions;
using System.Text.RegularExpressions;
using System;

public class Register : MonoBehaviour
{
    //Unity Variables
    [SerializeField] private TMP_InputField emailField;
    [SerializeField] private TMP_InputField userField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private TMP_InputField verifyPasswordField;
    [SerializeField] private TMP_Text validationField;
    [Tooltip("The Ui Panel to let the user enter name, connect and play")]
    public GameObject loginPanel;
    [Tooltip("The Ui Panel to register the user")]
    public GameObject registerPanel;

    //Firebase variables
    protected Firebase.Auth.FirebaseAuth auth;
    protected Firebase.Auth.FirebaseUser user;
    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
    DatabaseReference myDBRef;

    //Code variables
    private string emailReg;
    private string userReg;
    private string passwordReg;
    public string code = "";

    //Class User
    public class User
    {
        public string username;
        public float gold;
        public float gems;
        public float trophys;
        public string currentCharacter;
        public string currentAura;

        public User()
        {
        }

        public User(string username, float gold, float gems, float trophys, string currentCharacter, string currentAura)
        {
            this.username = username;
            this.gold = gold;
            this.gems = gems;
            this.trophys = trophys;
            this.currentCharacter = currentCharacter;
            this.currentAura = currentAura;
        }
    }

    //Class Shop
    public class Shop
    {
        public string MagicAura;
        public string FireAura;
        public string FrostAura;
        public string EternalMagicAura;
        public string EternalFireAura;
        public string EternalFrostAura;

        

        public Shop()
        {
        }

        public Shop(string MagicAura, string FireAura, string FrostAura, string EternalMagicAura, string EternalFireAura, string EternalFrostAura)
        {
            this.MagicAura = MagicAura;
            this.FireAura = FireAura;
            this.FrostAura = FrostAura;
            this.EternalMagicAura = EternalMagicAura;
            this.EternalFireAura = EternalFireAura;
            this.EternalFrostAura = EternalFrostAura;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Photon auth Initialization
        PhotonNetwork.AuthValues = new AuthenticationValues();

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                InitializeFirebase();
                myDBRef = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("Firebase Init");
            }
            else
            {
                Debug.LogError(
                  "Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });

    }

    private void FixedUpdate()
    {

        //HANDLE EXCEPTIONS
        switch (code)
        {
            case "InvalidEmail":
                validationField.text = "Enter a valid email address";
                validationField.color = new Color32(255, 0, 0, 255);
                break;
            
            case "InvalidUser":
                validationField.text = "Enter a valid username";
                validationField.color = new Color32(255, 0, 0, 255);
                break;

            case "UserNotFound":
                validationField.text = "Enter a valid email address";
                validationField.color = new Color32(255, 0, 0, 255);
                break;

            case "EmptyPassword":
                validationField.text = "Password field is empty";
                validationField.color = new Color32(255, 0, 0, 255);
                break;

            case "InvalidPasswordMatch":
                validationField.text = "These password do not match";
                validationField.color = new Color32(255, 0, 0, 255);
                break;

            case "WeakPassword":
                validationField.text = "The password is Weak";
                validationField.color = new Color32(255, 0, 0, 255);
                break;
            
            case "EmailAlreadyInUse":
                validationField.text = "This Email already exist";
                validationField.color = new Color32(255, 0, 0, 255);
                break;

            case "Complete":
                validationField.text = "Register completed";
                validationField.color = new Color32(0, 255, 0, 255);
                break;

            case "":
                validationField.text = "";
                break;

        }
    }

    public void userValidation()
    {
        FindObjectOfType<AudioManager>().Play("Menus");
        if (emailField.text == null || emailField.text == "")
        {
            Debug.Log("EMAIL EMPTY");
            emailField.gameObject.GetComponent<Image>().color = new Color32(255, 10, 0, 255);
            code = "InvalidEmail";
            return;
        }
        else
        {
            emailField.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }

        if (!Validate(emailField.text))
        {
            Debug.Log("EMAIL NOT VALID");
            emailField.gameObject.GetComponent<Image>().color = new Color32(255, 10, 0, 255);
            code = "InvalidEmail";
            return;
        }
        else
        {
            emailField.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }

        if (userField.text == null || userField.text == "")
        {
            Debug.Log("USERNAME EMPTY");
            userField.gameObject.GetComponent<Image>().color = new Color32(255, 10, 0, 255);
            code = "InvalidUser";
            return;
        }
        else
        {
            userField.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }

        if (passwordField.text == null || passwordField.text == "")
        {
            Debug.Log("PASSWORD EMPTY");
            passwordField.gameObject.GetComponent<Image>().color = new Color32(255, 10, 0, 255);
            verifyPasswordField.gameObject.GetComponent<Image>().color = new Color32(255, 10, 0, 255); 
            code = "EmptyPassword";
            return;
        }
        else
        {
            passwordField.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            verifyPasswordField.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }

        if (passwordField.text != verifyPasswordField.text)
        {
            Debug.Log("PASSWORD DOESNT MATCH");
            passwordField.gameObject.GetComponent<Image>().color = new Color32(255, 10, 0, 255);
            verifyPasswordField.gameObject.GetComponent<Image>().color = new Color32(255, 10, 0, 255);
            code = "InvalidPasswordMatch";
            return;
        }
        else
        {
            passwordField.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            verifyPasswordField.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
        emailReg = emailField.text;
        userReg = userField.text;
        passwordReg = passwordField.text;

        createUser(emailReg, userReg, passwordReg);

    }



    public void createUser(string emailReg, string userReg, string passwordReg)
    {
        Debug.Log("ALL CORRECT");

        auth.CreateUserWithEmailAndPasswordAsync(emailReg, passwordReg).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    string authErrorCode = "";
                    Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                    if (firebaseEx != null)
                    {
                        authErrorCode = String.Format("AuthError.{0}: ",
                          ((Firebase.Auth.AuthError)firebaseEx.ErrorCode).ToString());
                    }
                    Debug.Log("number- " + authErrorCode + "the exception is- " + exception.ToString());
                    code = ((Firebase.Auth.AuthError)firebaseEx.ErrorCode).ToString();
                    Debug.Log(code);
                }
                return;
            }
            // Firebase user has been created.
            Firebase.Auth.FirebaseUser newUser = task.Result;
            writeNewUser(newUser.UserId, userReg, 100, 100, 0, "Axel", "None");
            writeNewShopUser(newUser.UserId, "False", "False", "False", "False", "False", "False");
            PhotonNetwork.AuthValues.UserId = newUser.UserId;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
            code = "Complete";

        });
    }

    private void writeNewUser(string userId, string name, float gold, float gems, float trophys, string currentCharacter, string currentAura)
    {
        User user = new User(name, gold, gems, trophys, currentCharacter, currentAura);
        string json = JsonUtility.ToJson(user);
        myDBRef.Child("users").Child(userId).SetRawJsonValueAsync(json);
    }
    private void writeNewShopUser(string userId, string MagicAura, string FireAura, string FrostAura, string EternalMagicAura, string EternalFireAura, string EternalFrostAura)
    {
        Shop shop = new Shop(MagicAura, FireAura, FrostAura, EternalMagicAura, EternalFireAura, EternalFrostAura);
        string shopJson = JsonUtility.ToJson(shop);
        myDBRef.Child("shop").Child(userId).SetRawJsonValueAsync(shopJson);
    }

    public void changeToLogin()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
    }

    void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    // Track state changes of the auth object.
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
            }
        }
    }

    public static bool Validate(string emailAddress)
    {
        var regex = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
        bool isValid = Regex.IsMatch(emailAddress, regex, RegexOptions.IgnoreCase);
        return isValid;
    }

    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }



}
