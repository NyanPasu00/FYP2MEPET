using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.CloudSave;

public static class CloudSaveManager
{
    private const string PET_DATA_KEY = "PET_DATA";

    // -------------------------------------------------------
    // SAVE JSON to cloud
    // -------------------------------------------------------
    public static async Task SavePetDataToCloud(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning("SavePetDataToCloud → json empty, skipping.");
            return;
        }

        try
        {
            var data = new Dictionary<string, object>
            {
                { PET_DATA_KEY, json }
            };

            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log("Cloud Save Success.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Cloud Save Error: " + ex.Message);
        }
    }

    // -------------------------------------------------------
    // LOAD JSON from cloud
    // -------------------------------------------------------
    public static async Task<string> LoadPetDataFromCloud()
    {
        try
        {
            var result = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "PET_DATA" });

            if (result.ContainsKey("PET_DATA"))
            {
                string json = result["PET_DATA"].ToString(); 
                Debug.Log("Cloud Load Success: " + json);
                return json;
            }

            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError("Cloud Load Error: " + ex.Message);
            return null;
        }
    }

}
