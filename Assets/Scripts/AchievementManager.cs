using System.Linq;
using UnityEngine;
using System.Collections;

// AchievementManager contains Achievements, which players are able to earn through performing various actions
// in the game. Each Achievement specifies 

[System.Serializable]
public class Achievement
{
    public string Name;
    public string Description;
    public Texture2D IconIncomplete;
    public Texture2D IconComplete;
    public int RewardPoints;
    public float TargetProgress;
    public bool Secret;

    [HideInInspector]
    public bool Earned = false;
    private float currentProgress = 0.0f;

	// Returns true if this progress added results in the Achievement being earned.
    public bool AddProgress(float progress)
    {
        if (Earned)
        {
            return false;
        }

        currentProgress += progress;
        if (currentProgress >= TargetProgress)
        {
            Earned = true;
            return true;
        }

        return false;
    }

	// Returns true if this progress set results in the Achievement being earned.
    public bool SetProgress(float progress)
    {
        if (Earned)
        {
            return false;
        }

        currentProgress = progress;
        if (progress >= TargetProgress)
        {
            Earned = true;
            return true;
        }

        return false;
    }

	// Basic GUI for displaying an achievement. Has a different style when earned and not earned.
    public void OnGUI(Rect position, GUIStyle GUIStyleAchievementEarned, GUIStyle GUIStyleAchievementNotEarned)
    {
        GUIStyle style = GUIStyleAchievementNotEarned;
        if (Earned)
        {
            style = GUIStyleAchievementEarned;
        }

        GUI.BeginGroup(position);
        GUI.Box(new Rect(0.0f, 0.0f, position.width, position.height), "");

        if (Earned)
        {
            GUI.Box(new Rect(0.0f, 0.0f, position.height, position.height), IconComplete);
        }
        else
        {
            GUI.Box(new Rect(0.0f, 0.0f, position.height, position.height), IconIncomplete);
        }

        GUI.Label(new Rect(80.0f, 5.0f, position.width - 80.0f - 50.0f, 25.0f), Name, style);

        if (Secret && !Earned)
        {
            GUI.Label(new Rect(80.0f, 25.0f, position.width - 80.0f, 25.0f), "Description Hidden!", style);
            GUI.Label(new Rect(position.width - 50.0f, 5.0f, 25.0f, 25.0f), "???", style);
            GUI.Label(new Rect(position.width - 250.0f, 50.0f, 250.0f, 25.0f), "Progress Hidden!", style);
        }
        else
        {
            GUI.Label(new Rect(80.0f, 25.0f, position.width - 80.0f, 25.0f), Description, style);
            GUI.Label(new Rect(position.width - 50.0f, 5.0f, 25.0f, 25.0f), RewardPoints.ToString(), style);
            GUI.Label(new Rect(position.width - 250.0f, 50.0f, 250.0f, 25.0f), "Progress: [" + currentProgress.ToString("0.#") + " out of " + TargetProgress.ToString("0.#") + "]", style);
        }

        GUI.EndGroup();
    }
}

public class AchievementManager : MonoBehaviour
{
    public Achievement[] Achievements;
    public AudioClip EarnedSound;
    public GUIStyle GUIStyleAchievementEarned;
    public GUIStyle GUIStyleAchievementNotEarned;

    private int currentRewardPoints = 0;
    private int potentialRewardPoints = 0;
    private Vector2 achievementScrollviewLocation = Vector2.zero;

	void Start()
	{
	    ValidateAchievements();
        UpdateRewardPointTotals();
	}
	
    // Make sure some assumptions about achievement data setup are followed.
    private void ValidateAchievements()
    {
        ArrayList usedNames = new ArrayList();
        foreach (Achievement achievement in Achievements)
        {
            if (achievement.RewardPoints < 0)
            {
                Debug.LogError("AchievementManager::ValidateAchievements() - Achievement with negative RewardPoints! " + achievement.Name + " gives " + achievement.RewardPoints + " points!");
            }

            if (usedNames.Contains(achievement.Name))
            {
                Debug.LogError("AchievementManager::ValidateAchievements() - Duplicate achievement names! " + achievement.Name + " found more than once!");
            }
            usedNames.Add(achievement.Name);
        }
    }

    private Achievement GetAchievementByName(string achievementName)
    {
        return Achievements.FirstOrDefault(achievement => achievement.Name == achievementName);
    }

    private void UpdateRewardPointTotals()
    {
        currentRewardPoints = 0;
        potentialRewardPoints = 0;

        foreach (Achievement achievement in Achievements)
        {
            if (achievement.Earned)
            {
                currentRewardPoints += achievement.RewardPoints;
            }

            potentialRewardPoints += achievement.RewardPoints;
        }
    }

    private void AchievementEarned()
    {
        UpdateRewardPointTotals();
        AudioSource.PlayClipAtPoint(EarnedSound, Camera.main.transform.position);        
    }

    public void AddProgressToAchievement(string achievementName, float progressAmount)
    {
        Achievement achievement = GetAchievementByName(achievementName);
        if (achievement == null)
        {
            Debug.LogWarning("AchievementManager::AddProgressToAchievement() - Trying to add progress to an achievemnet that doesn't exist: " + achievementName);
            return;
        }

        if (achievement.AddProgress(progressAmount))
        {
            AchievementEarned();
        }
    }

    public void SetProgressToAchievement(string achievementName, float newProgress)
    {
        Achievement achievement = GetAchievementByName(achievementName);
        if (achievement == null)
        {
            Debug.LogWarning("AchievementManager::SetProgressToAchievement() - Trying to add progress to an achievemnet that doesn't exist: " + achievementName);
            return;
        }

        if (achievement.SetProgress(newProgress))
        {
            AchievementEarned();
        }
    }

	// Sets up a scrollview and fills it out with each Achievement.
	// Also displays the total number of reward points earned.
    void OnGUI()
    {
        float yValue = 5.0f;
        float achievementGUIWidth = 500.0f;

        GUI.Label(new Rect(200.0f, 5.0f, 200.0f, 25.0f), "-- Achievements --");

        achievementScrollviewLocation = GUI.BeginScrollView(new Rect(0.0f, 25.0f, achievementGUIWidth + 25.0f, 400.0f), achievementScrollviewLocation,
                                                            new Rect(0.0f, 0.0f, achievementGUIWidth, Achievements.Count() * 80.0f));

        foreach (Achievement achievement in Achievements)
        {
            Rect position = new Rect(5.0f, yValue, achievementGUIWidth, 75.0f);
            achievement.OnGUI(position, GUIStyleAchievementEarned, GUIStyleAchievementNotEarned);
            yValue += 80.0f;
        }

        GUI.EndScrollView();

        GUI.Label(new Rect(10.0f, 440.0f, 200.0f, 25.0f), "Reward Points: [" + currentRewardPoints + " out of " + potentialRewardPoints + "]");
    }
}
