using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using System;
using Firebase;
using Firebase.Unity.Editor;

public class Login : MonoBehaviour
{


    [SerializeField] private TMP_InputField emailField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private TMP_Text validationField;
    //[SerializeField ]private Launcher myLauncher;
    [Tooltip("The Ui Panel to let the user enter name, connect and play")]
    public GameObject loginPanel;
    [Tooltip("The Ui Panel to register the user")]
    public GameObject registerPanel;
    [Tooltip("The UI Label to inform the user that the connection is in progress")]
    public GameObject progressLabel;

    protected Firebase.Auth.FirebaseAuth auth;
    protected Firebase.Auth.FirebaseUser user;
    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;

    private string userLog;
    private string passwordLog;
    public string code = "";

    private bool connect = false;

    // Start is called before the first frame update
    void Start()
    {
        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://loentfg.firebaseio.com/");

        //Photon auth Initialization
        PhotonNetwork.AuthValues = new AuthenticationValues();

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                InitializeFirebase();
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
                break;

            case "UserNotFound":
                validationField.text = "Enter a valid email address";
                break;
            
            case "EmptyPassword":
                validationField.text = "Password field is empty";
                break;
            
            case "WrongPassword":
                validationField.text = "These credentials do not match";
                break;

            case "":
                validationField.text = "";
                break;
                
        }

        if (connect)
        {
            goLobby();
        }
        
    }

    public void userValidation()
    {
        FindObjectOfType<AudioManager>().Play("Menus");
        auth.SignOut();
        if (emailField.text == null || emailField.text == "")
        {
            emailField.gameObject.GetComponent<Image>().color = new Color32(255, 10, 0, 255); ;
            code = "InvalidEmail";
            return;
        }
        else
        {
            emailField.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }

        if (passwordField.text == null || passwordField.text == "")
        {
            passwordField.gameObject.GetComponent<Image>().color = new Color32(255, 10, 0, 255); ;
            code = "EmptyPassword";
            return;
        }
        else
        {
            passwordField.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }


        userLog = emailField.text;
        passwordLog = passwordField.text;

        LoginUser(userLog, passwordLog);

    }


    public void LoginUser(string userLog, string passwordLog)
    {
        auth.SignInWithEmailAndPasswordAsync(userLog, passwordLog).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
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
            Firebase.Auth.FirebaseUser newUser = task.Result;
            PhotonNetwork.AuthValues.UserId = newUser.UserId;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
            auth.StateChanged += AuthStateChanged;
            AuthStateChanged(this, null);
        });
    }

    public void changeToRegister()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
    }

    void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        Firebase.Auth.FirebaseAuth.DefaultInstance.SignOut();
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        PhotonNetwork.AuthValues.UserId = auth.CurrentUser.UserId;
        //auth.SignOut(); //DISCONNECT AT THE BEGGINING 4 TEST !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
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
                connect = true;
                
            }
        }
    }

    private void goLobby()
    {
        progressLabel.SetActive(true);
        loginPanel.SetActive(false);
        registerPanel.SetActive(false);
        SceneManager.LoadScene("Lobby");
    }

    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }

}
