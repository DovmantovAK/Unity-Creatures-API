using System.Collections.Generic;
using System.IO;
using Unity.Creatures;
using UnityEditor;
using UnityEngine;

namespace Unity.Creatures.Database
{
    public class CreaturesDatabaseSO : ScriptableObject
    {
        public const string RESOURCES_PATH = "Databases/Creature Database";
        public const string FOLDER_PATH = "Assets/Resources/Databases";
        public const string ASSET_NAME = "Creature Database.asset";

        // Используем List для хранения существ
        public List<Creature> Creatures = new();

        private static CreaturesDatabaseSO _instance;
        public static CreaturesDatabaseSO Instance
        {
            get
            {
                if (_instance == null)
                    _instance = LoadOrCreateDatabase();

                return _instance;
            }
        }

        private static CreaturesDatabaseSO LoadOrCreateDatabase()
        {
            _instance = Resources.Load<CreaturesDatabaseSO>(RESOURCES_PATH);

            if (_instance != null)
                return _instance;

            _instance = CreateInstance<CreaturesDatabaseSO>();

            if (!Directory.Exists(FOLDER_PATH))
                Directory.CreateDirectory(FOLDER_PATH);

#if UNITY_EDITOR
            AssetDatabase.CreateAsset(_instance, FOLDER_PATH + "/" + ASSET_NAME);
            AssetDatabase.SaveAssets();
#endif
            return _instance;
        }

        // Добавление нового существа
        public void AddCreature(Creature creature)
        {
            if (!Creatures.Exists(c => c.Id == creature.Id))
            {
                Creatures.Add(creature);
                SaveDatabase();
            }
            else
            {
                Debug.LogWarning($"Существо с ID {creature.Id} уже существует.");
            }
        }

        // Удаление существа по его ID
        public void RemoveCreature(string creatureId)
        {
            int index = Creatures.FindIndex(c => c.Id == creatureId);
            if (index != -1)
            {
                Creatures.RemoveAt(index);
                SaveDatabase();
            }
            else
            {
                Debug.LogWarning($"Существо с ID {creatureId} не найдено.");
            }
        }

        // Обновление данных существа
        public void UpdateCreatureValues(string creatureId, Creature newCreatureValues)
        {
            int index = Creatures.FindIndex(c => c.Id == creatureId);
            if (index != -1)
            {
                Creatures[index] = newCreatureValues;
                SaveDatabase();
            }
            else
            {
                Debug.LogWarning($"Существо с ID {creatureId} не найдено.");
            }
        }

        // Поиск существа по ID
        public bool TryFindCreatureByID(string id, out Creature creature)
        {
            int index = Creatures.FindIndex(c => c.Id == id);
            if (index != -1)
            {
                creature = Creatures[index];
                return true;
            }

            creature = default;
            return false;
        }

        // Сохранение базы данных
        private void SaveDatabase()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
#endif
        }
    }
}
