using System;
using System.Web.Razor.Parser.SyntaxTree;
using Core.Block.Config;
using Core.Block.Config.LoadConfig.Param;
using MainGame.UnityView.Chunk;
using MainGame.UnityView.Control.MouseKeyboard;
using MainGame.UnityView.UI.CraftRecipe;
using MainGame.UnityView.UI.UIState.UIObject;
using SinglePlay;
using UnityEngine;

namespace MainGame.UnityView.UI.UIState
{
    public class BlockInventoryState : IUIState
    {
        private readonly MoorestechInputSettings _inputSettings;
        private readonly BlockInventoryObject _blockInventory;
        private readonly CraftRecipeItemListViewer _craftRecipeItemListViewer;
        
        private readonly ItemRecipePresenter _itemRecipePresenter;
        
        private readonly IBlockClickDetect _blockClickDetect;
        private readonly ChunkBlockGameObjectDataStore _chunkBlockGameObjectDataStore;
        private readonly SinglePlayInterface _singlePlayInterface;
        public event Action<Vector2Int> OnOpenBlockInventory;
        public event Action OnCloseBlockInventory;

        public BlockInventoryState(MoorestechInputSettings inputSettings, BlockInventoryObject blockInventory,
            CraftRecipeItemListViewer craftRecipeItemListViewer,ItemRecipePresenter itemRecipePresenter,IBlockClickDetect blockClickDetect,ChunkBlockGameObjectDataStore chunkBlockGameObjectDataStore,SinglePlayInterface singlePlayInterface)
        {
            _craftRecipeItemListViewer = craftRecipeItemListViewer;
            _itemRecipePresenter = itemRecipePresenter;
            _blockClickDetect = blockClickDetect;
            _chunkBlockGameObjectDataStore = chunkBlockGameObjectDataStore;
            _singlePlayInterface = singlePlayInterface;
            _inputSettings = inputSettings;
            _blockInventory = blockInventory;
            blockInventory.gameObject.SetActive(false);
        }

        public bool IsNext()
        {
            return _inputSettings.UI.CloseUI.triggered || _inputSettings.UI.OpenInventory.triggered || _itemRecipePresenter.IsClicked;
        }

        public UIStateEnum GetNext()
        {
            if (_inputSettings.UI.CloseUI.triggered || _inputSettings.UI.OpenInventory.triggered)
            {
                return UIStateEnum.GameScreen;
            }

            if (_itemRecipePresenter.IsClicked)
            {
                return UIStateEnum.RecipeViewer;
            }

            return UIStateEnum.BlockInventory;
        }

        public void OnEnter(UIStateEnum lastStateEnum)
        {
            
            if (!_blockClickDetect.TryGetCursorOnBlockPosition(out var blockPos))
            {
                Debug.LogError("??????????????????????????????????????????????????????????????????UI??????????????????????????????????????????");
            }
            
            OnOpenBlockInventory?.Invoke(blockPos);
            
            //UI???????????????????????????????????????
            _craftRecipeItemListViewer.gameObject.SetActive(true);
            _blockInventory.gameObject.SetActive(true);

            
            //?????????????????????????????????????????????????????????
            if (!_chunkBlockGameObjectDataStore.ContainsBlockGameObject(blockPos))return;

            var id = _chunkBlockGameObjectDataStore.GetBlockGameObject(blockPos).BlockId;
            var config = _singlePlayInterface.BlockConfig.GetBlockConfig(id);
            
            switch (config.Type)
            {
                case VanillaBlockType.Chest:
                {
                    var configParam = config.Param as ChestConfigParam;
                    _blockInventory.SetOneSlotInventory(config.Name,configParam.ChestItemNum);
                    break;
                }
                case VanillaBlockType.Generator:
                {
                    var configParam = config.Param as PowerGeneratorConfigParam;
                    _blockInventory.SetOneSlotInventory(config.Name,configParam.FuelSlot);
                    break;
                }
                case VanillaBlockType.Machine:
                {
                    var configParam = config.Param as MachineBlockConfigParam;
                    _blockInventory.SetIOSlotInventory(config.Name,configParam.InputSlot,configParam.OutputSlot);
                    break;
                }
            }
        }

        public void OnExit()
        {
            OnCloseBlockInventory?.Invoke();
            
            _blockInventory.gameObject.SetActive(false);
            _craftRecipeItemListViewer.gameObject.SetActive(false);
        }
    }
}