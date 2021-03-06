using System;
using System.Collections.Generic;
using System.Linq;
using Core.Item;
using MainGame.Basic;
using SinglePlay;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MainGame.UnityView.UI.CraftRecipe
{
    public class ItemRecipePresenter : MonoBehaviour
    {
        private readonly Dictionary<int, List<Recipe>> _itemIdToRecipe = new();
        
        
        private  ItemRecipeView _itemRecipeView;
        
        public bool IsClicked => _isClickedCount == 0 || _isClickedCount == 1;
        private int _isClickedCount = -1;

        [Inject]
        public void Construct(CraftRecipeItemListViewer craftRecipeItemListViewer,SinglePlayInterface singlePlayInterface,ItemRecipeView itemRecipeView)
        {
            //レシピ表示用のDictionaryを構築する
            var craftRecipe = singlePlayInterface.CraftingConfig.GetCraftingConfigList();
            var machineRecipe = singlePlayInterface.MachineRecipeConfig.GetAllRecipeData();

            //手元クラフトの構築
            foreach (var recipe in craftRecipe)
            {
                if (!_itemIdToRecipe.TryGetValue(recipe.Result.Id,out var list))
                {
                    list = new();
                    _itemIdToRecipe[recipe.Result.Id] = list;
                }
                
                list.Add(new Recipe(recipe.Items,recipe.Result,RecipeType.Craft));
            }
            
            
            //機械レシピの構築
            foreach (var recipe in machineRecipe)
            {
                var resultItem = recipe.ItemOutputs.Select(o => o.OutputItem).ToList();
                foreach (var output in recipe.ItemOutputs)
                {
                    if (!_itemIdToRecipe.TryGetValue(output.OutputItem.Id,out var list))
                    {
                        list = new();
                        _itemIdToRecipe[output.OutputItem.Id] = list;
                    }
                    
                    list.Add(new Recipe(recipe.ItemInputs,resultItem,RecipeType.Machine,recipe.BlockId));
                }
            }
            
            
            
            //イベントをサブスクライブ
            craftRecipeItemListViewer.OnItemListClick += OnItemListClick;
            itemRecipeView.OnCraftSlotClick += OnItemListClick;
            _itemRecipeView = itemRecipeView;
        }

        private void OnItemListClick(int itemId)
        {
            if (!_itemIdToRecipe.ContainsKey(itemId))
            {
                return;
            }

            _isClickedCount = 0;
            
            //TODO 複数レシピに対応させる
            var recipe = _itemIdToRecipe[itemId][0];
            if (recipe.RecipeType == RecipeType.Craft)
            {
                _itemRecipeView.SetCraftRecipe(recipe.ItemStacks,recipe.ResultItem[0]);
            }
            else if (recipe.RecipeType == RecipeType.Machine) 
            {
                _itemRecipeView.SetMachineCraftRecipe(recipe.ItemStacks,recipe.ResultItem[0],recipe.BlockId);
            }
        }

        private void Update()
        {
            // ButtonがクリックされたことをUpdate内で確認したいのでクリックされてから2フレームはtrueとする
            if (_isClickedCount == 0 || _isClickedCount == 1)
            {
                _isClickedCount++;
            }
            if (_isClickedCount == 2)
            {
                _isClickedCount = -1;
            }
        }
    }

    class Recipe
    {
        public readonly List<ItemStack> ItemStacks = new();
        public readonly List<ItemStack> ResultItem = new();
        public readonly RecipeType RecipeType;
        public readonly int BlockId;

        public Recipe(List<IItemStack> itemStacks, List<IItemStack> resultItem, RecipeType recipeType,int blockId)
        {
            foreach (var item in itemStacks)
            {
                ItemStacks.Add(new ItemStack(item.Id,item.Count));
            }
            foreach (var item in resultItem)
            {
                ResultItem.Add(new ItemStack(item.Id,item.Count));
            }

            BlockId = blockId;
            RecipeType = recipeType;
        }
        public Recipe(List<IItemStack> itemStacks, IItemStack resultItem, RecipeType recipeType)
        {
            foreach (var item in itemStacks)
            {
                ItemStacks.Add(new ItemStack(item.Id,item.Count));
            }
            ResultItem.Add(new ItemStack(resultItem.Id,resultItem.Count));
            
            RecipeType = recipeType;
        }
    }

    enum RecipeType
    {
        Craft,
        Machine
    }
}