using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Google;
using System.Threading.Tasks;

public class GoogleAuthentication : MonoBehaviour
{
    public string imageURL;

    public TMP_Text usernameText, userEmailText;

    public GameObject loginPanel, profilePanel;


    public Image profilePic;

    public string webClientId = "430376463104-nc6pao45e2r72kqusf5h2jcvg8l3mvca.apps.googleusercontent.com";

    private GoogleSignInConfiguration configuration;

    // Defer the configuration creation until Awake so the web Client ID
    // Can be set via the property inspector in the Editor.
    void Awake() {
      configuration = new GoogleSignInConfiguration {
            WebClientId = webClientId,
            RequestIdToken = true
      };
    }

    public void OnSignIn() {
      GoogleSignIn.Configuration = configuration;
      GoogleSignIn.Configuration.UseGameSignIn = false;
      GoogleSignIn.Configuration.RequestIdToken = true;
      GoogleSignIn.Configuration.RequestEmail = true;

      GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
        OnAuthenticationFinished, TaskScheduler.Default);
    }

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task) {
        if (task.IsFaulted) {
            using (IEnumerator<System.Exception> enumerator =
                    task.Exception.InnerExceptions.GetEnumerator()) {
            if (enumerator.MoveNext()) {
                GoogleSignIn.SignInException error =
                        (GoogleSignIn.SignInException)enumerator.Current;
                Debug.Log("Got Error: " + error.Status + " " + error.Message);
            } else {
                Debug.Log("Got Unexpected Exception?!?" + task.Exception);
            }
            }
        } else if(task.IsCanceled) {
            Debug.Log("Canceled");
        } else  {
            Debug.Log("Welcome: " + task.Result.DisplayName + "!");

            usernameText.text = "" + task.Result.DisplayName;
            userEmailText.text = "" + task.Result.Email;

            imageURL = task.Result.ImageUrl.ToString();
            loginPanel.SetActive(false);
            profilePanel.SetActive(true);
            StartCoroutine(LoadProfilePic());
        }
    }

    IEnumerator LoadProfilePic()
    {
        WWW www = new WWW(imageURL);
        yield return www;

        profilePic.sprite = Sprite.Create(www.texture, new Rect(0,0, www.texture.width, www.texture.height), new Vector2(0,0));
    }

    public void OnSignOut() {
      Debug.Log("Calling SignOut");

        usernameText.text = "";
        userEmailText.text = "";

        imageURL = "";
        loginPanel.SetActive(true);
        profilePanel.SetActive(false);

      GoogleSignIn.DefaultInstance.SignOut();
    }

}
