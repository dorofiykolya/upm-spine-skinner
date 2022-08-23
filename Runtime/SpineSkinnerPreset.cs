using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity.Skinner
{
   [CreateAssetMenu(fileName = "Hero Visual Info Container",
      menuName = "Spine Customization Holders/Hero Visual Info Container")]
   public class SpineSkinnerPreset : ScriptableObject
   {
      [SerializeField] private List<SpineSkinVariant> _variants;

      public List<SpineSkinVariant> Variants => _variants;
   }
}