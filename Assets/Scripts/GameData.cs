using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Serializable]
public class GameData {
    public int highscore;

    public int shipTypeIndex;

    public int galacticCredits;
}