using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Score / Leaves")]
    public int currentScore = 0;

    // Feuilles pour le niveau courant
    public int leavesCollectedThisStage = 0;
    public int totalLeavesCollected = 0;
    public int pointsPerLeaf = 5;
    public int minLeavesToPass = 5;      
    public int minScoreToPass = 25;  

    // TOTAL de points de feuilles depuis le début de la RUN
    public int totalLeafPoints = 0;    

    public int deathCount = 0;   // nombre de morts total
    public int HighestStageUnlocked => highestStageUnlocked;
    public int DeathCount => deathCount;

    [Header("Stages")]
    public string[] stages = { "Stage1", "Stage2", "Stage3" };
    private int currentStageIndex = 0;

    // 🔹 Données sauvegardées (profil)
    private int savedHP = 3;
    private Vector2 savedPosition = Vector2.zero;
    private string savedStageName = "Stage1";
    private int highestStageUnlocked = 0;
    private bool loadingFromSave = false;

    // Clés PlayerPrefs
    private const string KEY_HP = "PlayerHP";
    private const string KEY_POS_X = "PlayerPosX";
    private const string KEY_POS_Y = "PlayerPosY";
    private const string KEY_STAGE = "LastStage";
    private const string KEY_SCORE = "LeafPoints";
    private const string KEY_HIGHEST_STAGE = "HighestStageUnlocked";
    private const string KEY_TOTAL_LEAVES = "TotalLeafPoints";
    private const string KEY_DEATHS = "DeathCount";


    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

        public void ResetRun()
    {
        currentScore = 0;
        leavesCollectedThisStage = 0;
        totalLeafPoints = 0;

        currentStageIndex = 0;
        highestStageUnlocked = 0;
        deathCount = 0;
    }


    public void AddLeaf()
    {
        // 1 feuille en plus dans ce niveau
        leavesCollectedThisStage++;

        // + points pour ce niveau
        currentScore += pointsPerLeaf;

        // + points pour la run (ce que le Diary affiche)
        totalLeafPoints += pointsPerLeaf;

        Debug.Log($"Stage leaves: {leavesCollectedThisStage}, stage points: {currentScore}, total points: {totalLeafPoints}");
    }


    public bool CanPassEndPoint()
    {
        return currentScore >= minScoreToPass;
        // Ou stricte avec le score :
        // return leavesCollectedThisStage >= minLeavesToPass && currentScore >= minScoreToPass;
    }

    public void GoToNextStage()
    {
        currentStageIndex++;

        if (currentStageIndex >= stages.Length)
        {
            SceneManager.LoadScene("MainMenu");
            return;
        }

        // Nouveau stage -> reset du niveau, pas du total
        leavesCollectedThisStage = 0;
        currentScore = 0;

        if (currentStageIndex > highestStageUnlocked)
            highestStageUnlocked = currentStageIndex;

        string nextStageName = stages[currentStageIndex];
        SceneManager.LoadScene(nextStageName);
    }

    public void GoToFirstStage()
    {
        currentStageIndex = 0;
        leavesCollectedThisStage = 0;
        currentScore = 0;

        SceneManager.LoadScene(stages[currentStageIndex]);
    }


    // =========================================================
    // SAVE / LOAD PROFIL AVEC PLAYERPREFS
    // =========================================================

    // Appelé quand on veut SAUVER le profil (depuis un stage ou le menu)
    public void SaveProfile()
    {
        // On récupère l'état du joueur actuel dans la scène
        var player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            savedHP = player.CurrentHP;   // propriété qu'on va ajouter
            savedPosition = player.transform.position;
            savedStageName = SceneManager.GetActiveScene().name;
        }

        // Sauvegarde dans PlayerPrefs
        PlayerPrefs.SetInt(KEY_HP, savedHP);
        PlayerPrefs.SetFloat(KEY_POS_X, savedPosition.x);
        PlayerPrefs.SetFloat(KEY_POS_Y, savedPosition.y);
        PlayerPrefs.SetString(KEY_STAGE, savedStageName);

        PlayerPrefs.SetInt(KEY_SCORE, currentScore);
        PlayerPrefs.SetInt(KEY_TOTAL_LEAVES, totalLeavesCollected);
        PlayerPrefs.SetInt(KEY_HIGHEST_STAGE, highestStageUnlocked);
        PlayerPrefs.SetInt(KEY_DEATHS, deathCount);


        PlayerPrefs.Save();

        Debug.Log("Profile saved.");
    }

    // Appelé quand on clique sur "Resume"
    public void ResumeGame()
    {
        if (!PlayerPrefs.HasKey(KEY_STAGE))
        {
            Debug.Log("No save found, starting new game instead.");
            NewGame();
            return;
        }

        // Lire les données
        savedHP = PlayerPrefs.GetInt(KEY_HP, 3);
        float x = PlayerPrefs.GetFloat(KEY_POS_X, 0f);
        float y = PlayerPrefs.GetFloat(KEY_POS_Y, 0f);
        savedPosition = new Vector2(x, y);

        savedStageName = PlayerPrefs.GetString(KEY_STAGE, "Stage1");
        currentScore = PlayerPrefs.GetInt(KEY_SCORE, 0);
        highestStageUnlocked = PlayerPrefs.GetInt(KEY_HIGHEST_STAGE, 0);
        totalLeavesCollected = PlayerPrefs.GetInt(KEY_TOTAL_LEAVES, 0);
        deathCount = PlayerPrefs.GetInt(KEY_DEATHS, 0);


        // On doit savoir quel index correspond au stage sauvé
        int idx = System.Array.IndexOf(stages, savedStageName);
        currentStageIndex = (idx >= 0) ? idx : 0;

        leavesCollectedThisStage = 0;  // on repart propre sur le compteur par stage

        loadingFromSave = true;
        SceneManager.LoadScene(savedStageName);
    }

    // Appelé quand on clique sur "New Game"
    public void NewGame()
    {
        // On efface TOUT pour repartir à zéro
        PlayerPrefs.DeleteAll();

        ResetRun();
        loadingFromSave = false;

        // On commence à Stage1
        currentStageIndex = 0;
        string firstStage = stages[currentStageIndex];
        SceneManager.LoadScene(firstStage);
    }

    // Utilisé par le Player pour savoir s'il doit charger les données
    public bool IsLoadingFromSave()
    {
        return loadingFromSave;
    }

    public Vector2 GetSavedPosition()
    {
        return savedPosition;
    }

    public int GetSavedHP()
    {
        return savedHP;
    }

    // Appelé une fois que le Player a lu les données
    public void OnPlayerLoadedFromSave()
    {
        loadingFromSave = false;
    }

    // Redémarrer uniquement le stage courant
    
    public void RestartCurrentStage()
    {
        // On NE veut PAS charger depuis une sauvegarde après ça
        loadingFromSave = false;

        // 🔹 1) Retirer du total ce que ce niveau a ajouté
        totalLeafPoints -= currentScore;
        if (totalLeafPoints < 0)
            totalLeafPoints = 0;

        // 🔹 2) Reset des compteurs du niveau
        leavesCollectedThisStage = 0;
        currentScore = 0;

        // 🔹 3) Effacer les PlayerPrefs si tu les utilises pour les feuilles / profil
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // 🔹 4) Recharger la scène actuelle
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
    }






    // Appelé par les boutons "Return to Main Menu" dans les stages
    public void ReturnToMainMenu()
    {
        SaveProfile();
        SceneManager.LoadScene("MainMenu");
    }

    private void OnApplicationQuit()
    {
        SaveProfile();
    }

    public void RegisterDeath()
    {
        deathCount++;
    }

}
