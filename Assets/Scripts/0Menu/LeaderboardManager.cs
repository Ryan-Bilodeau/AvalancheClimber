using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;

//Attach to MainCamera in 0Menu scene
//Manages leaderboard and player stats 
public class LeaderboardManager : MonoBehaviour
{
    public static int TotalWins;
    public static float QuickestWin;
    public static bool LoggedIn;

    private void Start()
    {
    #if !UNITY_EDITOR
        //Authenticate user
        if (!Social.localUser.authenticated)
        {
            Social.localUser.Authenticate(success => {
                if (success)
                {
                    LoggedIn = true;
                }
                else
                    LoggedIn = false;
            });
        }

        StartCoroutine(PostScoresToLeaderboards());
        StartCoroutine(CancelCoroutinesAfterDelay(5f));
    #endif
    }

    //Called when the is on the main menu and is logged in
    private IEnumerator PostScoresToLeaderboards()
    {
        for (;;)
        {
            if (LoggedIn)
            {
                TotalWins = PlayerPrefs.GetInt(PlayerPrefsKeys.TotalWins);
                QuickestWin = PlayerPrefs.GetFloat(PlayerPrefsKeys.QuickestWin);
                 
				Social.ReportScore((long)TotalWins, LeaderboardStrings.LeaderboardIDTotalWins, success => { });
				Social.ReportScore((long)QuickestWin, LeaderboardStrings.LeaderboardIDQuickestWin, success => { });
                break;
            }

            yield return new WaitForSeconds(.1f);
        }
    }

    private IEnumerator CancelCoroutinesAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        StopAllCoroutines();
    }
}
