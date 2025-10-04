using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Diagnostics;
using System.Threading.Tasks;

using SceneInstance = UnityEngine.ResourceManagement.ResourceProviders.SceneInstance;
using Debug = UnityEngine.Debug;

public static class AddressablesLoadProfiler
{
    public static async Task<AsyncOperationHandle<T>> LoadAssetAsync<T>(string address) where T : Object
    {
        long downloadSize = await GetBundleSize(address);

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        var handle = Addressables.LoadAssetAsync<T>(address);
        await handle.Task;

        stopwatch.Stop();
        float loadTime = stopwatch.ElapsedMilliseconds / 1000f;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"[AddressablesLoadProfiler] Ассет загружен: {address} ({typeof(T).Name})");
        }
        else
        {
            Debug.LogError($"[AddressablesLoadProfiler] Ошибка загрузки: {address} - {handle.OperationException}");
        }
        Debug.Log($"[AddressablesLoadProfiler] Время загрузки: {address} : {loadTime} секунд");
        Debug.Log($"[AddressablesLoadProfiler] Размер бандла: {address} : {FormatBytes(downloadSize)}");

        return handle;
    }

    public static async Task<AsyncOperationHandle<T>> LoadAssetAsync<T>(AssetReference assetReference) where T : Object
    {
        long downloadSize = await GetBundleSize(assetReference);

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        var handle = Addressables.LoadAssetAsync<T>(assetReference);
        await handle.Task;

        stopwatch.Stop();
        float loadTime = stopwatch.ElapsedMilliseconds / 1000f;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"[AddressablesLoadProfiler] Ассет загружен: {assetReference.RuntimeKey} ({typeof(T).Name})");
        }
        else
        {
            Debug.LogError($"[AddressablesLoadProfiler] Ошибка загрузки: {assetReference.RuntimeKey} - {handle.OperationException}");
        }

        Debug.Log($"[AddressablesLoadProfiler] Время загрузки: {assetReference.RuntimeKey} : {loadTime} секунд");
        Debug.Log($"[AddressablesLoadProfiler] Размер бандла: {assetReference.RuntimeKey} : {FormatBytes(downloadSize)}");

        return handle;
    }

    public static async Task<AsyncOperationHandle<SceneInstance>> LoadSceneAsync(string sceneName, bool activateOnLoad = true)
    {

        long downloadSize = await GetBundleSize(sceneName);

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        var handle = Addressables.LoadSceneAsync(sceneName, activateOnLoad: activateOnLoad);
        await handle.Task;

        stopwatch.Stop();
        float loadTime = stopwatch.ElapsedMilliseconds / 1000f;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"[AddressablesLoadProfiler] Сцена загружена: {sceneName}");
        }
        else
        {
            Debug.LogError($"[AddressablesLoadProfiler] Ошибка загрузки сцены: {sceneName} - {handle.OperationException}");
        }

        Debug.Log($"[AddressablesLoadProfiler] Время загрузки: {sceneName} : {loadTime} секунд");
        Debug.Log($"[AddressablesLoadProfiler] Размер бандла: {sceneName} : {FormatBytes(downloadSize)}");

        return handle;
    }

    public static async Task<long> GetBundleSize(AssetReference assetReference)
    {
        var sizeHandle = Addressables.GetDownloadSizeAsync(assetReference);
        await sizeHandle.Task;
        long downloadSize = sizeHandle.Result;
        Addressables.Release(sizeHandle);
        return downloadSize;
    }

    public static async Task<long> GetBundleSize(string adress)
    {
        var sizeHandle = Addressables.GetDownloadSizeAsync(adress);
        await sizeHandle.Task;
        long downloadSize = sizeHandle.Result;
        Addressables.Release(sizeHandle);
        return downloadSize;
    }

    public static string FormatBytes(long bytes)
    {
        if (bytes == 0) return "0 B";

        string[] suffixes = { "B", "KB", "MB", "GB" };
        int counter = 0;
        decimal number = bytes;

        while (Mathf.Round((float)number) >= 1000 && counter < suffixes.Length - 1)
        {
            number /= 1024;
            counter++;
        }

        return $"{number:F2} {suffixes[counter]}";
    }
}