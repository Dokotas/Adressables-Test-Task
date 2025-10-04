using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static int s_CurrentLevel = 0;

    public static int s_MaxAvailableLevel = 5;

    // The value of -1 means no hats have been purchased
    public static int s_ActiveHat = 0;

    [SerializeField] private Image m_gameLogoImage;
    [SerializeField] private AssetReference m_LogoAssetReference;
    private static AsyncOperationHandle<SceneInstance> m_SceneLoadOpHandle;
    private AsyncOperationHandle<Sprite> m_loadHandle;

    public void Awake()
    {
        // Caching.ClearCache();
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async void Start()
    {
        if (m_LogoAssetReference.RuntimeKeyIsValid())
            m_loadHandle = await AddressablesLoadProfiler.LoadAssetAsync<Sprite>(m_LogoAssetReference);

        if (m_loadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            m_gameLogoImage.sprite = m_loadHandle.Result;
        }
    }

    private void OnDestroy()
    {
        if (m_loadHandle.IsValid())
        {
            // Addressables.Release(m_loadHandle);
        }
    }

    public void OnEnable()
    {
        // When we go to the 
        s_CurrentLevel = 0;
    }


    public void ExitGame()
    {
        s_CurrentLevel = 0;
    }

    public void SetCurrentLevel(int level)
    {
        s_CurrentLevel = level;
    }

    public static void LoadNextLevel()
    {
        m_SceneLoadOpHandle = Addressables.LoadSceneAsync("LoadingScene", activateOnLoad: true);
    }

    public static void LevelCompleted()
    {
        s_CurrentLevel++;

        // Just to make sure we don't try to go beyond the allowed number of levels.
        s_CurrentLevel = s_CurrentLevel % s_MaxAvailableLevel;

        LoadNextLevel();
    }

    public static void ExitGameplay()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
