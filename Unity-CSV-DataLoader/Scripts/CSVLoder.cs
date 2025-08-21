using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public static class CSVLoder
{
    public static Dictionary<string, AttributesData> VanillaAttributesDataDic = new();//����B�}��Ҧ��HŪ������l�ݩʦr��
    public static Dictionary<EffectType, EffectData> EffectDataDic = new();//����B�}��Ҧ��HŪ�������A��Ʀr��


    public static void LoadVanillaAttributesDB()//Ū����l�ݩʸ�ơA�é�iVanillaAttributesDataDic
    {
        VanillaAttributesDataDic.Clear();
        List<Dictionary<string, object>> dataList = new();//�Ҧ��ݩ�DB�A��L��DB���|�[�i��
        List<Dictionary<string, object>> entityDataList = CSVReader.Read("EntityAttributesDB");//����ݩ�DB
        List<Dictionary<string, object>> MobDataList = CSVReader.Read("MobAttributesDB");//�����ݩ�DB
        List<Dictionary<string, object>> effectDataList = CSVReader.Read("EffectAttributesDB");//���A�ݩ�DB
        List<Dictionary<string, object>> equipmentDataList = CSVReader.Read("EquipmentAttributesDB");//�˳��ݩ�DB
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
            //�ݩʥ[��or����P�O�禡
            void CheckNewAttributePlusOrMultiply(string attributeString, ref float attribute, ref float attribute_M)//�ǤJ�[��&�����ܼưѦҡA�íק�����ܼ�
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
            //���A�|�h�ഫ�禡
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

            Debug.Log($"�w���J ��l�ݩ� {newAttributes.AttributeName} CSV���");
        }
    }
    public static void LoadEffectDataDB()//Ū�����A��ơA�é�iEffectDataDic
    {
        EffectDataDic.Clear();
        List<Dictionary<string, object>> dataList = CSVReader.Read("EffectDB");//���A���DB

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
            Debug.Log($"�w���J ���A {effectDataNew.ThisEffectType.ToString()} CSV���");
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

            int checkPointStartNumber = 5;//excel���ˬd�I���_�l��m
            var listFromDic = dataList[i].ToList();//��r���ন��ȹ�C��
            for (int j = checkPointStartNumber; j < dataList[i].Count; j++)//�M����ȹ�
            {
                string ckString = listFromDic[j].Value.ToString();//�����CK����ন�r��
                if (ckString.ToString() != "")//�ˬd�I�A�S���ƴN�i����
                {
                    Vector2 ckPos = new Vector2(0, 0);
                    int ckDpNumber = 0;
                    float stayTime = 0;

                    if (ckString.ToString().Contains("("))//�p�G�t���A���A�N���񰱯d�ɶ�
                    {
                        string[] parts = ckString.Split('(');//string��"("��������A�e���O�y��orDP�s���A�᭱�O�ݰ��d��ĴX��(�i���}�l��)

                        if (parts[0].Contains("*"))//�t��"*"�A��ܦ��ˬd�I���y��
                        {
                            string[] pos = parts[0].Trim().Split('*');
                            ckPos = new Vector2(float.Parse(pos[0].Trim()), float.Parse(pos[1].Trim()));
                        }
                        else//��ܦ��ˬd�I��DP
                        {
                            ckDpNumber = int.Parse(parts[0].Trim());
                        }

                        stayTime = float.Parse(parts[1].Trim());
                    }
                    else//�S�A���A�S���d�ɶ�
                    {
                        if (ckString.Contains("*"))//�t��"*"�A��ܦ��ˬd�I���y��
                        {
                            string[] pos = ckString.Trim().Split('*');
                            ckPos = new Vector2(float.Parse(pos[0].Trim()), float.Parse(pos[1].Trim()));
                        }
                        else//��ܦ��ˬd�I��DP
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
