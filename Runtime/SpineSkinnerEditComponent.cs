using UnityEngine;
using Spine.Unity;
using Spine.Unity.AttachmentTools;

namespace Spine.Unity.Skinner
{
    public class SpineSkinnerEditComponent : MonoBehaviour
    {
        public Material sourceMaterial;
        public SpineSkinnerComponent partsComponent;
        [SpineSkin] public string templateAttachmentsSkin;

        [Header("Apply skin options: ")] [SerializeField]
        private bool _useOnValidate = false;

        [SerializeField] private bool _useOnAwake = false;

        [Header("Runtime Repacking options: ")] [SerializeField]
        private bool _useRepack = false;

        private Texture2D _runtimeAtlas;
        private Material _runtimeMaterial;
        private Skin _customSkin;

        private readonly string _repackedSkinName = "repacked skin";
        private readonly string _customSkinName = "custom skin";
        private ISkeletonComponent _skeletonComponent;
        private ISkeletonAnimation _skeletonAnimation;

        private void OnValidate()
        {
            if (!_useOnValidate)
            {
                return;
            }

            if (sourceMaterial == null)
            {
                var skeletonAnimation = SkeletonComponent;
                sourceMaterial = skeletonAnimation.SkeletonDataAsset.atlasAssets[0].PrimaryMaterial;
            }

            Apply();
        }

        private void Awake()
        {
            if (!_useOnAwake)
            {
                return;
            }

            Apply();
        }

        public ISkeletonComponent SkeletonComponent =>
            _skeletonComponent ?? (_skeletonComponent = GetComponent<ISkeletonComponent>());

        public ISkeletonAnimation SkeletonAnimation =>
            _skeletonAnimation ?? (_skeletonAnimation = GetComponent<ISkeletonAnimation>());

        [ContextMenu("Set random")]
        public void SetRandomCustomizeToSlots()
        {
            for (int i = 0; i < partsComponent.Slots.Count; i++)
            {
                var slot = partsComponent.Slots[i];
                int j = Random.Range(0, partsComponent.SpineSkinner.Variants.Count);
                slot.CustomPath = partsComponent.SpineSkinner.Variants[j].Path;
                //in editor we should change index also, because of inspector change it in frame.
                slot.CurrentIndexInEditorPopup = j;
            }

            Apply();
        }

        public void Apply()
        {
            ISkeletonAnimation skeletonAnimation = SkeletonAnimation;
            Skeleton skeleton = skeletonAnimation.Skeleton;

            _customSkin = _customSkin ?? new Skin(name: _customSkinName);

            Skin templateSkin = skeleton.Data.FindSkin(templateAttachmentsSkin);

            _customSkin.AddSkin(templateSkin);

            for (int i = 0; i < partsComponent.Slots.Count; i++)
            {
                SlotSkinData slot = partsComponent.Slots[i];
                int index = skeleton.Data.FindSlot(slot.Slot).Index;
                Attachment templateAttachment = templateSkin.GetAttachment(index, slot.Key);
                Sprite sprite = Resources.Load<Sprite>(slot.SpritePath);

                if (sprite == null)
                {
                    Debug.LogError($"Error detected after trying of loading sprite with path: {slot.SpritePath}");
                    continue;
                }

                Attachment newAttachment =
                    templateAttachment.GetRemappedClone(sprite, sourceMaterial);
                _customSkin.SetAttachment(index, slot.Key, newAttachment);
            }

            if (_useRepack)
            {
                Skin repackedSkin = new Skin(_repackedSkinName);
                repackedSkin.AddSkin(skeleton.Data.DefaultSkin);
                repackedSkin.AddSkin(_customSkin);
                repackedSkin = repackedSkin.GetRepackedSkin(
                    newName: _repackedSkinName,
                    materialPropertySource: sourceMaterial,
                    outputMaterial: out _runtimeMaterial,
                    outputTexture: out _runtimeAtlas,
                    maxAtlasSize: 2048
                );
                skeleton.SetSkin(repackedSkin);
            }
            else
            {
                skeleton.SetSkin(_customSkin);
            }

            skeleton.SetSlotsToSetupPose();

            if (skeletonAnimation is SkeletonAnimation)
            {
                ((SkeletonAnimation)skeletonAnimation).Update(0);
            }
            else if (skeletonAnimation is SkeletonGraphic)
            {
                ((SkeletonGraphic)skeletonAnimation).Update(0);
            }

            //Resources.UnloadUnusedAssets();
        }
    }
}