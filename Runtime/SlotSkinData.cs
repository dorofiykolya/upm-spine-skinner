using System.IO;
using Spine.Unity;
using UnityEngine;

namespace Spine.Unity.Skinner
{

    [System.Serializable]
    public class SlotSkinData
    {
        [SpineSlot] public string Slot;

        [SpineAttachment(slotField: "Slot", skinField: "baseSkinName")]
        public string Key = "";

        /// <summary>
        /// Used only for editor where we could change via editor popup variant of customization
        /// </summary>
        [HideInInspector] public int CurrentIndexInEditorPopup = 0;

        public string SpritePath => $"{CustomPath}/{SpriteBaseName}";

        /// <summary>
        /// Substring used for path format "001/spritename" to remove part "001/" and get only sprite name. 
        /// </summary>
        public string SpriteBaseName => Path.GetFileName(Key);

        public string CustomPath;
    }
}