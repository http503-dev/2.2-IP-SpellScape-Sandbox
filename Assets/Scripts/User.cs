using System;

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
    public ChallengeArea()
    {

    }

    public ChallengeArea(float fastestTimePerWord, int leastMistakes, int totalAttempts)
    {
        this.fastestTimePerWord = fastestTimePerWord;
        this.leastMistakes = leastMistakes;
        this.totalAttempts = totalAttempts;
    }
}

public class User
{
    public string email;
    public bool adminStatus;
    public SandboxArea sandboxArea;
    public ChallengeArea challengeArea;
    public float totalHours;
    public string profilePicURL;
    public string difficultyLevel;
    public User()
    {

    }

    public User(string email, bool adminStatus, SandboxArea sandboxArea, ChallengeArea challengeArea, float totalHours, string profilePicURL, string difficultyLevel)
    {
        this.email = email;
        this.adminStatus = adminStatus;
        this.sandboxArea = sandboxArea;
        this.challengeArea = challengeArea;
        this.totalHours = totalHours;
        this.profilePicURL = profilePicURL;
        this.difficultyLevel = difficultyLevel;
    }
}