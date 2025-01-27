using System;
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
    public SandboxArea()
    {

    }

    public SandboxArea(int uniqueWords)
    {
        this.uniqueWords = uniqueWords;
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
    public bool adminStatus;
    public ImageTracking ImageTracking;
    public SandboxArea sandboxArea;
    public ChallengeArea challengeArea;
    public float totalHours;
    public string profilePicURL;
    public User()
    {

    }

    public User(string email, bool adminStatus, ImageTracking imageTracking, SandboxArea sandboxArea, ChallengeArea challengeArea, float totalHours, string profilePicURL)
    {
        this.email = email;
        this.adminStatus = adminStatus;
        this.ImageTracking = imageTracking; 
        this.sandboxArea = sandboxArea;
        this.challengeArea = challengeArea;
        this.totalHours = totalHours;
        this.profilePicURL = profilePicURL;
    }
}