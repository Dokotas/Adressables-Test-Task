using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

// Used for the Hat selection logic
public class PlayerConfigurator : MonoBehaviour
{
    [SerializeField]
    private Transform m_HatAnchor;

    private GameObject m_HatInstance;

    private AsyncOperationHandle<GameObject> m_HatLoadOpHandle;

    private int m_hatIndex = -1;

    void Start()
    {
        LoadInRandomHat();
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            LoadInRandomHat();
        }
    }

    private async void LoadInRandomHat()
    {
        int randomIndex = Random.Range(0, 6);
        if (randomIndex == m_hatIndex)
            randomIndex = (randomIndex + Random.Range(0, 6)) % 6;
        m_hatIndex = randomIndex;

        string hatAddress = string.Format("Hat{0:00}", m_hatIndex);

        if (m_HatLoadOpHandle.IsValid())
            Addressables.ReleaseInstance(m_HatLoadOpHandle);

        m_HatLoadOpHandle = await AddressablesLoadProfiler.LoadAssetAsync<GameObject>(hatAddress);
        if (m_HatLoadOpHandle.Status == AsyncOperationStatus.Succeeded)
        {
            if (m_HatInstance)
                Destroy(m_HatInstance);
            m_HatInstance = Instantiate(m_HatLoadOpHandle.Result, m_HatAnchor);
        }
    }
}
