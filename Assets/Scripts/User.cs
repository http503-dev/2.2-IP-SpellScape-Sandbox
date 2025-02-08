using System;
using System.Collections.Generic;
using JetBrains.Annotations;

[Serializable]
public class ImageTracking
{
    public string imageDifficultyLevel;

    public ImageTracking()
    {

    }

    public ImageTracking(string imageDifficultyLevel)
    {
        this.imageDifficultyLevel = imageDifficultyLevel;
    }
}
[Serializable]
public class SandboxArea
{
    public int uniqueWords;
    public List<string> uniqueWordsList;
    public SandboxArea()
    {

    }

    public SandboxArea(int uniqueWords, List<string> uniqueWordsList)
    {
        this.uniqueWords = uniqueWords;
        this.uniqueWordsList = uniqueWordsList ?? new List<string>();
    }
}


[Serializable]
public class ChallengeArea
{
    public float fastestTimePerWord;
    public int leastMistakes;
    public int totalAttempts;
    public int totalWordsPerAttempt;
    public string challengeDifficultyLevel;
    public ChallengeArea()
    {

    }

    public ChallengeArea(float fastestTimePerWord, int leastMistakes, int totalAttempts, int totalWordsPerAttempt, string challengeDifficultyLevel)
    {
        this.fastestTimePerWord = fastestTimePerWord;
        this.leastMistakes = leastMistakes;
        this.totalAttempts = totalAttempts;
        this.totalWordsPerAttempt = totalWordsPerAttempt;
        this.challengeDifficultyLevel = challengeDifficultyLevel;
    }
}

public class User
{
    public string email;
    public string username;
    public bool adminStatus;
    public ImageTracking imageTracking;
    public SandboxArea sandboxArea;
    public ChallengeArea challengeArea;
    public string profilePicURL;
    public User()
    {

    }

    public User(string email, string username, bool adminStatus, ImageTracking imageTracking, SandboxArea sandboxArea, ChallengeArea challengeArea, string profilePicURL)
    {
        this.email = email;
        this.username = username;
        this.adminStatus = adminStatus;
        this.imageTracking = imageTracking; 
        this.sandboxArea = sandboxArea;
        this.challengeArea = challengeArea;
        this.profilePicURL = profilePicURL;
    }
}