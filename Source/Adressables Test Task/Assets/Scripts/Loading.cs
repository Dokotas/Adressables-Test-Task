using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using System.Threading.Tasks;
using Stopwatch = System.Diagnostics.Stopwatch;

public class Loading : MonoBehaviour
{
    [SerializeField]
    private Slider m_LoadingSlider;

    [SerializeField]
    private GameObject m_PlayButton, m_LoadingText;

    private static AsyncOperationHandle<SceneInstance> m_SceneLoadOpHandle;

    private void Awake()
    {
        LoadScene("Level_0" + GameManager.s_CurrentLevel);
    }

    public async void LoadScene(string level)
    {
        long downloadSize = await AddressablesLoadProfiler.GetBundleSize(level);

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        m_SceneLoadOpHandle = Addressables.LoadSceneAsync(level, activateOnLoad: false);
        while (!m_SceneLoadOpHandle.IsDone)
        {
            m_LoadingSlider.value = m_SceneLoadOpHandle.PercentComplete;
            await Task.Yield();
        }

        stopwatch.Stop();
        float loadTime = stopwatch.ElapsedMilliseconds / 1000f;

        Debug.Log($"[AddressablesLoadProfiler] Время загрузки: {level} : {loadTime} секунд");
        Debug.Log($"[AddressablesLoadProfiler] Размер бандла: {level} : {AddressablesLoadProfiler.FormatBytes(downloadSize)}");

        m_PlayButton.SetActive(true);
        m_LoadingSlider.value= 1f;
    }

    public async void OpenLevel()
    {
        await m_SceneLoadOpHandle.Result.ActivateAsync();
        Debug.Log("Level_0" + GameManager.s_CurrentLevel + " opened");
    }
}
