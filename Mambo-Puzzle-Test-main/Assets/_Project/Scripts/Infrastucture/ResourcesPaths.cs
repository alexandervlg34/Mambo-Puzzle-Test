using System.IO;
using UnityEngine;

namespace Infrastructure
{
    public static class ResourcesPaths
    {
        private const string baseUiPath = "UI";

        public static readonly string LevelsDataPath = Path.Combine(Application.streamingAssetsPath, "LevelsData.json");
        public static string GetUiWindowPath(string windowName) => Path.Combine(baseUiPath, windowName);
    }
}
