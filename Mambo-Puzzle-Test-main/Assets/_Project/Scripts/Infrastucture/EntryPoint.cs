using Infrastructure.GameDataProviers;
using Match3;
using Match3.Data;
using Match3.Randomizers;
using Match3.Views;
using System;
using System.Collections.Generic;
using System.IO;
using UI.Infractructure;
using UI.Windows;
using UnityEngine;

namespace Infrastructure
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private TimeModeView _timeModeView;
        [SerializeField] private Match3Controller _match3Controller;
        private GameLoopController _gameLoopController;

        private void Awake()
        {
            Startup();
        }

        private void Startup()
        {
            Dictionary<Type, BaseWindow> windowByType = LoadWindowPrefabs();
            UIManager uiManager = new(windowByType, _canvas.transform);

            LevelsData levelsData = ReadJSONAndDeserialize<LevelsData>(ResourcesPaths.LevelsDataPath);
            ICellsRandomizer cellsRandomizer = new SystemRandomCellsRandomizer();
            IGameDataProvider gameDataProvider = new TimeModeGameDataProvider(levelsData, _timeModeView, cellsRandomizer);

            _gameLoopController = new GameLoopController(_match3Controller, uiManager, gameDataProvider);
            _gameLoopController.Start();
        }

        private T ReadJSONAndDeserialize<T>(string configPath)
        {
            string json = File.ReadAllText(configPath);
            return JsonUtility.FromJson<T>(json);
        }

        private T LoadAsset<T>(string path) where T : UnityEngine.Object
        {
            T resource = Resources.Load<T>(path);
            return resource;
        }

        private Dictionary<Type, BaseWindow> LoadWindowPrefabs()
        {
            Dictionary<Type, BaseWindow> windows = new();

            LoadSingleWindowPrefab<StartWindow>(windows);
            LoadSingleWindowPrefab<ResultsWindow>(windows);

            return windows;
        }

        private void LoadSingleWindowPrefab<T>(Dictionary<Type, BaseWindow> windows) where T : BaseWindow
        {
            Type type = typeof(T);
            string windowTypeName = type.Name;
            string windowPath = ResourcesPaths.GetUiWindowPath(windowTypeName);

            T window = LoadAsset<T>(windowPath);
            if (window == null)
            {
                throw new Exception($"There's no asset with type \"{windowTypeName}\" in the Resources folder " +
                    $"with following path: \"{windowPath}\"");
            }
            
            windows.Add(type, window);
        }
    }
}
