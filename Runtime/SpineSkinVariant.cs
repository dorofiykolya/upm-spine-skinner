using UnityEngine;

namespace Spine.Unity.Skinner
{
    [System.Serializable]
    public class SpineSkinVariant
    {
        [Tooltip("Customization name. This is up to you.")]
        public string Name;

        [Tooltip("Path to folder in Resources")]
        public string Path;
    }
}