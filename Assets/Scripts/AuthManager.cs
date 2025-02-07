using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using System;
using Firebase;
using static UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics.HapticsUtility;

public class AuthManager : MonoBehaviour
{
    public TMP_InputField emailUpField;
    public TMP_InputField passwordUpField;
    public TMP_InputField emailInField;
    public TMP_InputField passwordInField;

    public TMP_Text signUpErrorText;
    public TMP_Text signInErrorText;

    Firebase.Auth.FirebaseAuth auth;
    DatabaseReference mDatabaseRef;

    private string userId;

    FirebaseUser user;

    public Controller controller;

    private void Awake()
    {
        auth = FirebaseAuth.DefaultInstance;
        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    private void Update()
    {
        if (auth.CurrentUser != null)
        {
            string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
            userId = auth.CurrentUser.UserId;
        }
    }

    public void SignUp()
    {
        string email = emailUpField.text.Trim();
        string password = passwordUpField.text.Trim();

        if (string.IsNullOrEmpty(email) || !IsValidEmail(email))
        {
            SignUpErrorMessage("The email you entered is invalid. Please enter a valid email address.");
            return;
        }

        if (string.IsNullOrEmpty(password) || password.Length < 6)
        {
            SignUpErrorMessage("Password must be at least 6 characters long.");
            return;
        }

        CheckIfEmailExists(email, password);
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var mail = new System.Net.Mail.MailAddress(email);
            return mail.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private void CheckIfEmailExists(string email, string password)
    {
        DatabaseReference usersRef = FirebaseDatabase.DefaultInstance.RootReference.Child("users");

        usersRef.OrderByChild("email").EqualTo(email).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Error checking email: " + task.Exception);
                SignUpErrorMessage("A network error occurred. Please try again later.");
                return;
            }

            if (task.Result.Exists)
            {
                SignUpErrorMessage("This email is already registered. Please use another email or sign in instead.");
            }
            else
            {
                SignUpUser(email, password);
            }
        });
    }

    private void SignUpUser(string email, string password)
    {
        Debug.Log("SignUpUser function triggered");

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Sorry, there was an error creating your new account, ERROR: " + task.Exception);

                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    FirebaseException firebaseEx = exception as FirebaseException;
                    if (firebaseEx != null)
                    {
                        var errorCode = (AuthError)firebaseEx.ErrorCode;
                        switch (errorCode)
                        {
                            case AuthError.EmailAlreadyInUse:
                                Debug.LogWarning("The email is already in use.");
                                SignUpErrorMessage("This email is already registered. Please use another email or sign in instead.");
                                break;

                            case AuthError.WeakPassword:
                                Debug.LogWarning("The password is too weak.");
                                SignUpErrorMessage("Your password is too weak. Please choose a stronger password.");
                                break;

                            case AuthError.InvalidEmail:
                                Debug.LogWarning("Invalid email format.");
                                SignUpErrorMessage("The email you entered is invalid. Please enter a valid email address.");
                                break;

                            case AuthError.NetworkRequestFailed:
                                Debug.LogWarning("Network error occurred.");
                                SignUpErrorMessage("A network error occurred. Please check your connection and try again.");
                                break;

                            default:
                                Debug.LogError("Unknown error occurred: " + firebaseEx.Message);
                                SignUpErrorMessage("An unexpected error occurred. Please try again.");
                                break;
                        }
                    }
                }

                return;
            }
            else if (task.IsCompleted)
            {
                FirebaseUser newUser = task.Result.User;
                userId = newUser.UserId;

                Debug.Log("Welcome " + newUser.Email);

                CreateNewUser(userId, emailUpField.text, false, "Easy", 0, 0f, 0, 0, 0, "Easy", 0f, "");
                Debug.Log("new user created");
                if (controller == null)
                {
                    Debug.Log("controller is null");
                }
                controller.ToMainMenu();
                Debug.Log("starting game");
            }
            else
            {
                return;
            }
        });

        if (auth == null)
        {
            Debug.LogError("Auth service is not initialized!");
            return;
        }
    }

    private void SignUpErrorMessage(string message)
    {
        if (signUpErrorText.text != message)
        {
            signUpErrorText.text = message;
            signUpErrorText.enabled = true;
        }
    }

    private void CreateNewUser(string userId, string email, bool adminStatus, string imageDifficultyLevel, int uniqueWords, float fastestTimePerWord, int leastMistakes, int totalAttempts, int totalWordsPerAttempt, string challengeDifficultyLevel, float totalHours, string profilePicURL)
    {
        //User user = new User(email, uniqueWords, fastestTimePerWord, leastMistakes, totalAttempts, profilePicURL);
        ImageTracking imageTracking = new ImageTracking(imageDifficultyLevel);
        SandboxArea sandbox = new SandboxArea(uniqueWords);
        ChallengeArea challenge = new ChallengeArea(fastestTimePerWord, leastMistakes, totalAttempts, totalWordsPerAttempt, challengeDifficultyLevel);
        User user = new User(email, adminStatus, imageTracking, sandbox, challenge, totalHours, profilePicURL);

        string json = JsonUtility.ToJson(user, true);
        Debug.Log("Attempting to write to database..." + json);
        mDatabaseRef.Child("users").Child(userId).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Database write failed: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                Debug.Log("User data successfully written to database.");
            }
        });
    }

    public void SignIn()
    {
        string email = emailInField.text.Trim();
        string password = passwordInField.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            SignInErrorMessage("Email and password fields must not be empty.");
            return;
        }

        if (string.IsNullOrEmpty(password) || password.Length < 6)
        {
            SignInErrorMessage("Password must be at least 6 characters long.");
            return;
        }
        CheckSignInEmail(email, password);
    }

    private void CheckSignInEmail(string email, string password)
    {
        DatabaseReference usersRef = FirebaseDatabase.DefaultInstance.RootReference.Child("users");

        usersRef.OrderByChild("email").EqualTo(email).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Error checking email: " + task.Exception);
                SignUpErrorMessage("A network error occurred. Please try again later.");
                return;
            }

            if (task.Result.Exists)
            {
                SignInUser(email, password);
            }
            else
            {
                SignInErrorMessage("This email is not registered. Please use a registered email or sign up instead.");
            }
        });
    }

    private void SignInUser(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Sign in failed: " + task.Exception);

                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    FirebaseException firebaseEx = exception as FirebaseException;
                    if (firebaseEx != null)
                    {
                        var errorCode = (AuthError)firebaseEx.ErrorCode;
                        switch (errorCode)
                        {
                            case AuthError.InvalidEmail:
                                SignInErrorMessage("The email you entered is invalid. Please check and try again.");
                                break;

                            case AuthError.UserNotFound:
                                Debug.LogWarning("No account found with this email.");
                                SignInErrorMessage("This email is not registered. Please sign up first.");
                                break;

                            case AuthError.WrongPassword:
                                Debug.LogWarning("Incorrect password.");
                                SignInErrorMessage("The password you entered is incorrect. Please try again.");
                                break;

                            case AuthError.NetworkRequestFailed:
                                Debug.LogWarning("Network error occurred.");
                                SignInErrorMessage("A network error occurred. Please check your connection and try again.");
                                break;

                            default:
                                Debug.LogError("Unknown error occurred: " + firebaseEx.Message);
                                SignInErrorMessage("An unexpected error occurred. Please try again later.");
                                break;
                        }
                    }
                }
                return;
            }
            else if (task.IsCompleted)
            {
                FirebaseUser loggedInUser = task.Result.User;
                userId = loggedInUser.UserId;
                Debug.Log("Successfully logged in! Welcome back " + loggedInUser.Email);
                controller.ToMainMenu();
            }
        });
    }

    private void SignInErrorMessage(string message)
    {
        if (signInErrorText.text != message)
        {
            signInErrorText.text = message;
            signInErrorText.enabled = true;
        }
    }
}
