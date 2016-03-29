using System.Collections.Generic;

public static class AchievementsInfo  {

    private static Dictionary<int, string> _achievements = new Dictionary<int, string>
    {
        { 0, "Reach top speed" },
        { 1, "Collect 50 coins" },
        { 2, "Crash you car" }
    };

    public static int GetTotalAchievementsCount()
    {
        return _achievements.Count;
    }

    public static string GetAchievementDescription(int achievementId)
    {
        if(_achievements.ContainsKey(achievementId))
        {
            string achievementDescription;
            _achievements.TryGetValue(achievementId, out achievementDescription);
            return achievementDescription;
        }

        return "";
    }

}
