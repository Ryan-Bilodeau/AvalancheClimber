using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Not attached to anything
//Pretty obvious what this is
public static class PlayerPrefsKeys
{
	public static string MuteState = "MuteState";
	public static string TotalWins = "TotalWins";
	public static string QuickestWin = "QuickestWin";
    public static string Ropes = "Ropes";
}

public static class LeaderboardStrings
{
	public static string LeaderboardIDTotalWins = "YouAreFallingTotalWins";
	public static string LeaderboardIDQuickestWin = "YouAreFallingQuickestWin";
}
