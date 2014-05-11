using UnityEngine;
using System.Collections;

// This Hero class sets up a simple GUI and allows the player to click buttons to do things like buy potions
// and kill dragons. Hero also communicates with the AchievementManager to track progress.

public class Hero : MonoBehaviour
{
    public AchievementManager AchievementManager;

    private bool inStealthMode = false;
    private int currentLevel = 0;
    private int numRedPotions = 0;
    private int numBluePotions = 0;
    private int numGreenPotions = 0;
    
	void Start()
    {
        LevelUp();
	}

    private void LevelUp()
    {
        currentLevel++;
        AchievementManager.AddProgressToAchievement("Junior Hero", 1.0f);
        AchievementManager.AddProgressToAchievement("Experienced Hero", 1.0f);
    }
	
	void Update()
    {
        if (inStealthMode)
        {
            AchievementManager.AddProgressToAchievement("The Invisible Man", Time.deltaTime);
        }
	}

    private void PotionQuantitiesChanged()
    {
        AchievementManager.SetProgressToAchievement("Potion Hoarder", numRedPotions + numBluePotions + numGreenPotions);
        if (numRedPotions > 0 && numBluePotions > 0 && numGreenPotions > 0)
        {
            AchievementManager.SetProgressToAchievement("Potion Collector", 1.0f);
        }
    }

    private void BuyPotion(ref int potion)
    {
        potion++;
        AchievementManager.SetProgressToAchievement("Potion Owner", 1.0f);
        PotionQuantitiesChanged();
    }

    private void DrinkPotion(ref int potion)
    {
        potion--;
        AchievementManager.AddProgressToAchievement("Town Drunk", 1.0f);
        PotionQuantitiesChanged();
    }

    void OnGUI()
    {
        float xPosition = 600.0f;

        GUI.Label(new Rect(xPosition, 25.0f, 150.0f, 25.0f), "Level: " + currentLevel);
        if (GUI.Button(new Rect(xPosition + 100.0f, 25.0f, 100.0f, 25.0f), "Level Up!"))
        {
            LevelUp();
        }

        if (GUI.Button(new Rect(xPosition, 55.0f, 150.0f, 25.0f), "Kill a Goblin"))
        {
            AchievementManager.AddProgressToAchievement("Goblin Battle", 1.0f);
        }

        if (GUI.Button(new Rect(xPosition, 85.0f, 150.0f, 25.0f), "Kill the Red Dragon"))
        {
            AchievementManager.SetProgressToAchievement("Dragon Slayer", 1.0f);
        }

        inStealthMode = GUI.Toggle(new Rect(xPosition, 115.0f, 150.0f, 25.0f), inStealthMode, "Stealth Mode");

        GUI.Label(new Rect(xPosition, 150.0f, 150.0f, 25.0f), "Red Potions: " + numRedPotions);
        if (GUI.Button(new Rect(xPosition + 125.0f, 150.0f, 50.0f, 25.0f), "Buy") && numRedPotions < 9)
        {
            BuyPotion(ref numRedPotions);
        }
        if (GUI.Button(new Rect(xPosition + 180.0f, 150.0f, 50.0f, 25.0f), "Drink") && numRedPotions > 0)
        {
            DrinkPotion(ref numRedPotions);
        }

        GUI.Label(new Rect(xPosition, 180.0f, 150.0f, 25.0f), "Blue Potions: " + numBluePotions);
        if (GUI.Button(new Rect(xPosition + 125.0f, 180.0f, 50.0f, 25.0f), "Buy") && numBluePotions < 9)
        {
            BuyPotion(ref numBluePotions);
        }
        if (GUI.Button(new Rect(xPosition + 180.0f, 180.0f, 50.0f, 25.0f), "Drink") && numBluePotions > 0)
        {
            DrinkPotion(ref numBluePotions);
        }

        GUI.Label(new Rect(xPosition, 210.0f, 150.0f, 25.0f), "Green Potions: " + numGreenPotions);
        if (GUI.Button(new Rect(xPosition + 125.0f, 210.0f, 50.0f, 25.0f), "Buy") && numGreenPotions < 9)
        {
            BuyPotion(ref numGreenPotions);
        }
        if (GUI.Button(new Rect(xPosition + 180.0f, 210.0f, 50.0f, 25.0f), "Drink") && numGreenPotions > 0)
        {
            DrinkPotion(ref numGreenPotions);
        }
    }
}