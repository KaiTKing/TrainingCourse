using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

//public class JsonDataManager 
//{
//    //用单例模式构造
//    private static JsonDataManager instance = null; 
//    private readonly Dictionary<int, WeaponData> weaponDict;
//    private JsonDataManager()
//    {
//        weaponDict = new Dictionary<int, WeaponData>();
//    }

//    //将数据存到字典里 键值对为<int,WeaponData>

//    //读取json文件将数据填入字典中
//}
public static class JsonDataManager
{
    private static Dictionary<int, WeaponData> weaponDict;

    static JsonDataManager()
    {
        weaponDict = new Dictionary<int, WeaponData>();
        LoadFromJson("weapons");
    }

    public static void AddWeaponData(int id, WeaponData data)
    {
        weaponDict[id] = data;
    }

    public static WeaponData GetWeaponData(int id)
    {
        WeaponData data;
        if (weaponDict.TryGetValue(id, out data)) {
            return data;
        }
        Debug.LogError($"武器ID{id}不存在!");
        return null;
    }

    public static void LoadFromJson(string filePath)
    {
        string jsonString = Resources.Load<TextAsset>(filePath).text;
        JsonData dataArray = JsonMapper.ToObject(jsonString);

        for (int i=0; i< dataArray.Count;i++)
        {
            int id = int.Parse(dataArray[i]["weaponid"].ToString());
            weaponDict.Add(id, new WeaponData(id, dataArray[i]["weaponname"].ToString(),
            dataArray[i]["weapontype"].ToString(),
            int.Parse(dataArray[i]["damage"].ToString()),
            float.Parse(dataArray[i]["shootinterval"].ToString()),
            int.Parse(dataArray[i]["nums"].ToString()),
            dataArray[i]["modelpath"].ToString(),
            dataArray[i]["imagepath"].ToString()));
        }
    }
}