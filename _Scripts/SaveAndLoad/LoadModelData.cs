using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MameshibaGames.Kekos.RuntimeExampleScene.Spawner;
using MameshibaGames.Kekos.CharacterEditorScene.Customization;
using MameshibaGames.Common.Helpers;

public class LoadModelData : MonoBehaviour
{
    [Header("BODY DATABASES")]
    [SerializeField]
    private PartDatabase torsoDatabase;

    [SerializeField]
    private PartDatabase legsDatabase;

    [SerializeField]
    private PartDatabase feetDatabase;

    [SerializeField]
    private PartDatabase handsDatabase;

    [SerializeField]
    private PartDatabase fullTorsoDatabase;

    [SerializeField]
    private PartDatabase torsoPropDatabase;

    [Header("HEAD AND SKIN DATABASES")]
    [SerializeField]
    private PartObjectDatabase hairDatabase;

    [SerializeField]
    private PartObjectDatabase capDatabase;

    [SerializeField]
    private PartObjectDatabase glassesDatabase;

    [SerializeField]
    private EyesDatabase eyesDatabase;

    [SerializeField]
    private SkinDatabase skinDatabase;

    [SerializeField]
    private PartDatabase eyebrowsDatabase;

    [SerializeField]
    private GameObject _currentMeshes;
    [SerializeField]
    private GameObject _currentSkeleton;

    private const string _HeadModelName = "Bip001 Head";
    private void Awake()
    {
        LoadSavedCharacter();
    }

    public void LoadSavedCharacter()
    {
        string savedCharacterJson = PlayerPrefs.GetString("KEKOS_SavedCharacter", "");
        if (string.IsNullOrEmpty(savedCharacterJson)) return;

        InstantiatorPart instantiatorPart = new InstantiatorPart();
        FilterPart filterPart = new FilterPart();
        EyesPart eyesPart = new EyesPart();
        SkinPart skinPart = new SkinPart();
        FaceCharacteristicsPart faceCharacteristicsPart = new FaceCharacteristicsPart();

        SaveCustomizationModel saveCustomizationModel = JsonUtility.FromJson<SaveCustomizationModel>(savedCharacterJson);

        filterPart.FilterItem(torsoDatabase, saveCustomizationModel.torsoSelected, _currentMeshes);
        filterPart.FilterItem(legsDatabase, saveCustomizationModel.legsSelected, _currentMeshes);
        filterPart.FilterItem(feetDatabase, saveCustomizationModel.feetSelected, _currentMeshes);
        filterPart.FilterItem(handsDatabase, saveCustomizationModel.handsSelected, _currentMeshes);
        filterPart.FilterItem(fullTorsoDatabase, saveCustomizationModel.fullTorsoSelected, _currentMeshes,
            extraLogicIfFound: RemoveTorsoAndLegs);
        filterPart.FilterItem(torsoPropDatabase, saveCustomizationModel.torsoPropSelected, _currentMeshes);

        int capItemCode = instantiatorPart.CreateItem(capDatabase, saveCustomizationModel.capSelected, _HeadModelName,
            _currentSkeleton);
        if (capItemCode != 52)
            instantiatorPart.CreateItem(hairDatabase, saveCustomizationModel.hairSelected, _HeadModelName, _currentSkeleton,
                true, capItemCode == 51 ? 1 : 0);
        instantiatorPart.CreateItem(glassesDatabase, saveCustomizationModel.glassesSelected, _HeadModelName, _currentSkeleton,
            true);
        eyesPart.ModifiyItem(eyesDatabase, saveCustomizationModel.eyesSelected, _currentMeshes);
        skinPart.ModifyItem(skinDatabase, saveCustomizationModel.skinBaseSelected, saveCustomizationModel.skinDetailSelected,
            _currentMeshes);
        filterPart.FilterItem(eyebrowsDatabase, saveCustomizationModel.eyebrowsSelected, _currentMeshes, true);
        faceCharacteristicsPart.ModifyItem(saveCustomizationModel.faceCharacteristicsSelected, _currentMeshes);
    }
    private void RemoveTorsoAndLegs()
    {
        List<Transform> filteredItemPieces = _currentMeshes.transform.All(x =>
            x.gameObject.name.StartsWith("TOR_") || x.gameObject.name.StartsWith("LEG_"));

        filteredItemPieces.ForEach(x => Destroy(x.gameObject));
    }
}
