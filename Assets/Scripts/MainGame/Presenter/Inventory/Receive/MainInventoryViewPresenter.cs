using MainGame.Network.Event;
using MainGame.UnityView.UI.Inventory.Control;
using VContainer.Unity;

namespace MainGame.Presenter.Inventory.Receive
{
    //IInitializableがないとDIコンテナ作成時にインスタンスが生成されないので実装しておく
    public class MainInventoryViewPresenter : IInitializable
    {
        private readonly PlayerInventoryViewModelController _playerInventoryViewModelController;
        private readonly PlayerInventoryViewModel _playerInventoryViewModel;

        public MainInventoryViewPresenter(ReceiveMainInventoryEvent receiveMainInventoryEvent,PlayerInventoryViewModelController playerInventoryViewModelController,PlayerInventoryViewModel playerInventoryViewModel)
        {
            _playerInventoryViewModelController = playerInventoryViewModelController;
            _playerInventoryViewModel = playerInventoryViewModel;
            receiveMainInventoryEvent.OnMainInventoryUpdateEvent +=UpdateInventory;
            receiveMainInventoryEvent.OnMainInventorySlotUpdateEvent +=UpdateSlotInventory;
        }

        public void UpdateInventory(MainInventoryUpdateProperties properties)
        {
            _playerInventoryViewModel.SetMainInventory(properties.ItemStacks);
            for (int i = 0; i < properties.ItemStacks.Count; i++)
            {
                var id = properties.ItemStacks[i].ID;
                var count = properties.ItemStacks[i].Count;
                var slot = i;
                //View側を更新する
                _playerInventoryViewModelController.SetInventoryItem(slot,id,count);
            }
        }

        public void UpdateSlotInventory(MainInventorySlotUpdateProperties properties)
        {
            
            //View側を更新する
            _playerInventoryViewModelController.SetInventoryItem(properties.SlotId,properties.ItemStack.ID,properties.ItemStack.Count);
        }
        public void Initialize() { }
    }
}