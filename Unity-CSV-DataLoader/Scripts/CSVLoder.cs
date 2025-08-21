using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public static class CSVLoder
{
    public static Dictionary<string, AttributesData> VanillaAttributesDataDic = new();//全域、開放所有人讀取的原始屬性字典
    public static Dictionary<EffectType, EffectData> EffectDataDic = new();//全域、開放所有人讀取的狀態資料字典


    public static void LoadVanillaAttributesDB()//讀取原始屬性資料，並放進VanillaAttributesDataDic
    {
        VanillaAttributesDataDic.Clear();
        List<Dictionary<string, object>> dataList = new();//所有屬性DB，其他的DB都會加進來
        List<Dictionary<string, object>> entityDataList = CSVReader.Read("EntityAttributesDB");//單位屬性DB
        List<Dictionary<string, object>> MobDataList = CSVReader.Read("MobAttributesDB");//雜魚屬性DB
        List<Dictionary<string, object>> effectDataList = CSVReader.Read("EffectAttributesDB");//狀態屬性DB
        List<Dictionary<string, object>> equipmentDataList = CSVReader.Read("EquipmentAttributesDB");//裝備屬性DB
        dataList.AddRange(entityDataList);
        dataList.AddRange(MobDataList);
        dataList.AddRange(effectDataList);
        dataList.AddRange(equipmentDataList);


        for (int i = 0; i < dataList.Count; i++)
        {
            AttributesData newAttributes = new();
            newAttributes.AttributeName = dataList[i]["AttributeName"].ToString();

            CheckNewAttributePlusOrMultiply(dataList[i]["Health"].ToString(), ref newAttributes.Health, ref newAttributes.Health_M);
            CheckNewAttributePlusOrMultiply(dataList[i]["Shield"].ToString(), ref newAttributes.Shield, ref newAttributes.Shield_M);
            CheckNewAttributePlusOrMultiply(dataList[i]["Mentality"].ToString(), ref newAttributes.Mentality, ref newAttributes.Mentality_M);
            CheckNewAttributePlusOrMultiply(dataList[i]["Range"].ToString(), ref newAttributes.Range, ref newAttributes.Range_M);
            CheckNewAttributePlusOrMultiply(dataList[i]["Attack"].ToString(), ref newAttributes.Attack, ref newAttributes.Attack_M);

            CheckNewAttributePlusOrMultiply(dataList[i]["AttackSpeed"].ToString(), ref newAttributes.AttackSpeed, ref newAttributes.AttackSpeed_M);
            CheckNewAttributePlusOrMultiply(dataList[i]["MoveSpeed"].ToString(), ref newAttributes.MoveSpeed, ref newAttributes.MoveSpeed_M);
            CheckNewAttributePlusOrMultiply(dataList[i]["Size"].ToString(), ref newAttributes.Size, ref newAttributes.Size_M);
            CheckNewAttributePlusOrMultiply(dataList[i]["Defense"].ToString(), ref newAttributes.Defense, ref newAttributes.Defense_M);
            CheckNewAttributePlusOrMultiply(dataList[i]["Resistance"].ToString(), ref newAttributes.Resistance, ref newAttributes.Resistance_M);

            newAttributes.InitialEffectDic = ConvertEffectStringToDic(dataList[i]["InitialEffectDic"].ToString());
            newAttributes.ResistEffectDic = ConvertEffectStringToDic(dataList[i]["ResistEffectDic"].ToString());
            newAttributes.ClaerEffectDic = ConvertEffectStringToDic(dataList[i]["ClaerEffectDic"].ToString());

            newAttributes.ExtraStackRatio = float.Parse(dataList[i]["ExtraStackRatio"].ToString());

            VanillaAttributesDataDic.Add(newAttributes.AttributeName, newAttributes);
            //屬性加算or乘算判別函式
            void CheckNewAttributePlusOrMultiply(string attributeString, ref float attribute, ref float attribute_M)//傳入加算&乘算變數參考，並修改對應變數
            {
                if (attributeString.Contains("%"))
                {
                    attribute_M = float.Parse(attributeString.Replace("%", "").Trim()) * 0.01f;

                }
                else
                {
                    attribute = float.Parse(attributeString);
                }
            }
            //狀態疊層轉換函式
            Dictionary<EffectType, float> ConvertEffectStringToDic(string allEffectString)
            {
                Dictionary<EffectType, float> newEffectDic = new();
                if (allEffectString.Trim() != "")
                {
                    string[] effectStringArray = allEffectString.Trim().Split('-');
                    foreach (string effectString in effectStringArray)
                    {
                        string[] parts = effectString.Split('*');
                        EffectType effectType = (EffectType)Enum.Parse(typeof(EffectType), parts[0]);
                        int stack = int.Parse(parts[1]);
                        newEffectDic.Add(effectType, stack);
                    }
                }
                return newEffectDic;
            }

            Debug.Log($"已載入 原始屬性 {newAttributes.AttributeName} CSV資料");
        }
    }
    public static void LoadEffectDataDB()//讀取狀態資料，並放進EffectDataDic
    {
        EffectDataDic.Clear();
        List<Dictionary<string, object>> dataList = CSVReader.Read("EffectDB");//狀態資料DB

        foreach (var dic in dataList)
        {
            EffectData effectDataNew = new();

            effectDataNew.ThisEffectType = (EffectType)Enum.Parse(typeof(EffectType), dic["ThisEffectType"].ToString());
            effectDataNew.StackDuration = float.Parse(dic["StackDuration"].ToString());
            effectDataNew.MaxStack = int.Parse(dic["MaxStack"].ToString());
            effectDataNew.StackedModify = int.Parse(dic["StackedModify"].ToString());
            effectDataNew.RemoveStacks = int.Parse(dic["RemoveStacks"].ToString());

            effectDataNew.IsEffectShow = bool.Parse(dic["IsEffectShow"].ToString());
            effectDataNew.IsSourceDependent = bool.Parse(dic["IsSourceDependent"].ToString());
            effectDataNew.IsStackResetTimer = bool.Parse(dic["IsStackResetTimer"].ToString());
            effectDataNew.IsOverStack = bool.Parse(dic["IsOverStack"].ToString());


            EffectDataDic.Add(effectDataNew.ThisEffectType, effectDataNew);
            Debug.Log($"已載入 狀態 {effectDataNew.ThisEffectType.ToString()} CSV資料");
        }
    }

    public static List<StageUnitData> LoadStageUnitDB(string filePath)
    {
        List<Dictionary<string, object>> dataList = CSVReader.Read(filePath);
        List<StageUnitData> StageUnitDataList = new();

        for (int i = 0; i < dataList.Count; i++)
        {
            StageUnitData stageUnitData = new();

            stageUnitData.UnitName = dataList[i]["UnitName"].ToString();
            stageUnitData.WaveNumber = int.Parse(dataList[i]["WaveNumber"].ToString());
            stageUnitData.SummonTime = float.Parse(dataList[i]["SummonTime"].ToString());
            stageUnitData.Faction = (UnitFaction)Enum.Parse(typeof(UnitFaction), dataList[i]["Faction"].ToString());
            stageUnitData.Behavior = (UnitBehavior)Enum.Parse(typeof(UnitBehavior), dataList[i]["Behavior"].ToString());

            int checkPointStartNumber = 5;//excel內檢查點的起始位置
            var listFromDic = dataList[i].ToList();//把字典轉成鍵值對列表
            for (int j = checkPointStartNumber; j < dataList[i].Count; j++)//遍歷鍵值對
            {
                string ckString = listFromDic[j].Value.ToString();//把對應CK資料轉成字串
                if (ckString.ToString() != "")//檢查點，沒填資料就可結束
                {
                    Vector2 ckPos = new Vector2(0, 0);
                    int ckDpNumber = 0;
                    float stayTime = 0;

                    if (ckString.ToString().Contains("("))//如果含有括號，代表有填停留時間
                    {
                        string[] parts = ckString.Split('(');//string用"("分成兩塊，前面是座標orDP編號，後面是需停留到第幾秒(波次開始後)

                        if (parts[0].Contains("*"))//含有"*"，表示此檢查點為座標
                        {
                            string[] pos = parts[0].Trim().Split('*');
                            ckPos = new Vector2(float.Parse(pos[0].Trim()), float.Parse(pos[1].Trim()));
                        }
                        else//表示此檢查點為DP
                        {
                            ckDpNumber = int.Parse(parts[0].Trim());
                        }

                        stayTime = float.Parse(parts[1].Trim());
                    }
                    else//沒括號，沒停留時間
                    {
                        if (ckString.Contains("*"))//含有"*"，表示此檢查點為座標
                        {
                            string[] pos = ckString.Trim().Split('*');
                            ckPos = new Vector2(float.Parse(pos[0].Trim()), float.Parse(pos[1].Trim()));
                        }
                        else//表示此檢查點為DP
                        {
                            ckDpNumber = int.Parse(ckString.Trim());
                        }
                    }

                    stageUnitData.CheckPointDataList.Add(new CheckPointData(ckPos, ckDpNumber, stayTime));
                }
                else break;
            }

            StageUnitDataList.Add(stageUnitData);
        }

        return StageUnitDataList;
    }
}
