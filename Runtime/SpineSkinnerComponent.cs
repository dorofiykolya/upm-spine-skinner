using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity.Skinner
{
    public class SpineSkinnerComponent : MonoBehaviour
    {
        [SerializeField] private SpineSkinnerPreset spineSkinnerPreset;
        [SerializeField] private List<SlotSkinData> _slots;

        public List<SlotSkinData> Slots => _slots;
        public SpineSkinnerPreset SpineSkinner => spineSkinnerPreset;
    }
}