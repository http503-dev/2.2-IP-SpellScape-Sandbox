public class User
{
    public string email;
    public int uniqueWords;
    public float fastestTimePerWord;
    public int leastMistakes;
    public int totalAttempts;

    public User()
    {

    }

    public User(string email, int uniqueWords, float fastestTimePerWord, int leastMistakes, int totalAttempts)
    {
        this.email = email;
        this.uniqueWords = uniqueWords;
        this.fastestTimePerWord = fastestTimePerWord;
        this.leastMistakes = leastMistakes;
        this.totalAttempts = totalAttempts;
    }
}