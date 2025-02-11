using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
public class UpdateProfile : MonoBehaviour
{
    DatabaseReference mDatabaseRef;

    public Controller controller;
    public WebCam webCam;
    public TMP_InputField username;

    public TextMeshProUGUI profileUsername;
    public TextMeshProUGUI profileEmail;
    //public Image profilePicture;
    public TextMeshProUGUI profileUniqueWords;
    public TextMeshProUGUI profileTotalAttempts;
    public TextMeshProUGUI profileTotalWordsPerAttempt;
    public TextMeshProUGUI profileFastestTimePerWord;
    public TextMeshProUGUI profileLeastMistakes;

    public string imageUrl;
    public RawImage targetImage;

    void Start()
    {
        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        Read();
    }

    IEnumerator DownloadImage(string url)
    {
        Debug.Log("Downloading profile image from: " + url);

        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download image: " + uwr.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                if (texture != null)
                {
                    targetImage.texture = texture;
                    Debug.Log("Profile image updated successfully!");
                }
                else
                {
                    Debug.LogError("Downloaded texture is null.");
                }
            }
        }
    }

    public void ReadAndUpdate()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        mDatabaseRef.Child("users").Child(userId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("error reading" + task.Exception);
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                Debug.Log("read");
                DataSnapshot snap = task.Result;

                foreach (var user in snap.Children)
                {
                    Debug.Log("User info: " + user.ToString());

                    string userDetails = user.GetRawJsonValue();
                    Debug.Log("raw json data of user " + userDetails);

                    User i = JsonUtility.FromJson<User>(snap.GetRawJsonValue());
                    Debug.LogFormat(userId);
                }

                User existingUser = JsonUtility.FromJson<User>(task.Result.GetRawJsonValue());

                User updatedUser = new User(existingUser.email, username.text, existingUser.adminStatus, existingUser.imageTracking, existingUser.sandboxArea, existingUser.challengeArea, webCam.profilePicURL);

                string json = JsonUtility.ToJson(updatedUser);
                mDatabaseRef.Child("users").Child(userId).SetRawJsonValueAsync(json).ContinueWithOnMainThread(updateTask =>
                {
                    if (updateTask.IsCompleted)
                    {
                        Debug.Log("Leaderboard updated successfully.");
                        controller.ToMainMenu();
                    }
                    else
                    {
                        Debug.LogError("Error updating leaderboard: " + updateTask.Exception);
                    }
                });
            }
        });
    }



    public void Read()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        mDatabaseRef.Child("users").Child(userId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("error reading" + task.Exception);
                return;
            }

            if (!task.Result.Exists)
            {
                Debug.LogError("No data found for user: " + userId);
                return;
            }
            DataSnapshot snap = task.Result;
            Debug.Log("User Data Retrieved: " + snap.GetRawJsonValue());

            // Deserialize user data
            User existingUser = JsonUtility.FromJson<User>(snap.GetRawJsonValue());

            // Deserialize SandboxArea and ChallengeArea separately
            string sandboxJson = snap.Child("sandboxArea").GetRawJsonValue();
            SandboxArea existingSandboxArea = sandboxJson != null ? JsonUtility.FromJson<SandboxArea>(sandboxJson) : new SandboxArea();

            string challengeJson = snap.Child("challengeArea").GetRawJsonValue();
            ChallengeArea existingChallengeArea = challengeJson != null ? JsonUtility.FromJson<ChallengeArea>(challengeJson) : new ChallengeArea();

            // Update UI on main thread
            imageUrl = existingUser.profilePicURL;
            profileUsername.text = existingUser.username + "'s Profile";
            profileEmail.text = existingUser.email;
            profileUniqueWords.text = "Unique Words Found: " + existingSandboxArea.uniqueWords.ToString();
            profileTotalAttempts.text = "Total Attempts: " + existingChallengeArea.totalAttempts.ToString();
            profileTotalWordsPerAttempt.text = "Highest Number of Words in 1 Attempt: " + existingChallengeArea.totalWordsPerAttempt.ToString();
            profileFastestTimePerWord.text = "Least Time Taken to Form a Word: " + existingChallengeArea.fastestTimePerWord.ToString("n2");
            profileLeastMistakes.text = "Least Number of Mistakes Made in 1 Attempt: " + existingChallengeArea.leastMistakes.ToString();

            Debug.Log("Profile UI Updated Successfully.");

            imageUrl = existingUser.profilePicURL;
            Debug.Log("Profile Pic URL from Firebase: " + existingUser.profilePicURL);

            if (!string.IsNullOrEmpty(imageUrl))
            {
                StartCoroutine(DownloadImage(imageUrl));
            }
            else
            {
                Debug.LogError("No profile picture URL found in Firebase.");
            }
        });
    }
}

