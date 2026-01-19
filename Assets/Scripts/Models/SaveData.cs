[System.Serializable]
public class SaveData
{
    public int score;
    public int clicks;
    public int matchedPairs;
    public int rows;
    public int cols;

    public int[] matchedIndexes;   // which cards already solved
    public int[] cardIds;          // board layout
}
