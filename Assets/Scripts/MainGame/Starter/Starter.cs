using System.Diagnostics;
using Game.Quest.Interface;
using GameConst;
using MainGame.Control.UI.PauseMenu;
using MainGame.ModLoader;
using MainGame.ModLoader.Glb;
using MainGame.Network;
using MainGame.Network.Event;
using MainGame.Network.Receive;
using MainGame.Network.Send;
using MainGame.Network.Send.SocketUtil;
using MainGame.Network.Settings;
using MainGame.Presenter.Chunk;
using MainGame.Presenter.Command;
using MainGame.Presenter.Inventory;
using MainGame.Presenter.Inventory.Receive;
using MainGame.Presenter.Inventory.Send;
using MainGame.Presenter.Loading;
using MainGame.Presenter.Player;
using MainGame.Presenter.Quest;
using MainGame.UnityView.Block;
using MainGame.UnityView.Chunk;
using MainGame.UnityView.Control.MouseKeyboard;
using MainGame.UnityView.Game;
using MainGame.UnityView.UI.CraftRecipe;
using MainGame.UnityView.UI.Inventory.Control;
using MainGame.UnityView.UI.Inventory.Element;
using MainGame.UnityView.UI.Inventory.View;
using MainGame.UnityView.UI.Inventory.View.HotBar;
using MainGame.UnityView.UI.Quest;
using MainGame.UnityView.UI.UIState;
using MainGame.UnityView.UI.UIState.UIObject;
using MainGame.UnityView.WorldMapTile;
using SinglePlay;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MainGame.Starter
{
    public class Starter : LifetimeScope
    {
        private string IPAddress = ServerConst.LocalServerIp;
        private int Port = ServerConst.LocalServerPort;
        
        private int PlayerId = ServerConst.DefaultPlayerId;
        
        private bool isLocal = false;
        private Process localServerProcess = null;

        public void SetProperty(MainGameStartProprieties proprieties)
        {
            IPAddress = proprieties.serverIp;
            Port = proprieties.serverPort;
            isLocal = proprieties.isLocal;
            
            PlayerId = proprieties.playerId;
            localServerProcess = proprieties.localServerProcess;
        }
        

        private IObjectResolver _resolver;
        
        [SerializeField] private WorldMapTileObject worldMapTileObject;
        
        [Header("InHierarchy")]
        [SerializeField] Camera mainCamera;

        [SerializeField] private GroundPlane groundPlane;
        [SerializeField] private BlockGameObject nothingIndexBlock;

        [SerializeField] private ChunkBlockGameObjectDataStore chunkBlockGameObjectDataStore;
        [SerializeField] private WorldMapTileGameObjectDataStore worldMapTileGameObjectDataStore;
        
        [SerializeField] private HotBarItemView hotBarItemView;
        [SerializeField] private BlockClickDetect blockClickDetect;
        [SerializeField] private CommandUIInput commandUIInput;
        [SerializeField] private DetectGroundClickToSendBlockPlacePacket detectGroundClickToSendBlockPlacePacket;
        [SerializeField] private SelectHotBarControl selectHotBarControl;
        [SerializeField] private PlayerPosition playerPosition;
        [SerializeField] private SelectHotBarView selectHotBarView;
        [SerializeField] private ItemRecipeView itemRecipeView;
        [SerializeField] private QuestUI QuestUI;
        
        [SerializeField] private GrabbedItemImagePresenter grabbedItemImagePresenter;

        [SerializeField] private UIStateControl uIStateControl;
        [SerializeField] private LoadingFinishDetector loadingFinishDetector;
        [SerializeField] private BlockInventoryObject blockInventoryObject;
        [SerializeField] private PlayerInventoryObject playerInventoryObject;
        [SerializeField] private PauseMenuObject pauseMenuObject;
        [SerializeField] private DeleteBarObject deleteBarObject;
        [SerializeField] private RecipeViewerObject recipeViewerObject;
        [SerializeField] private ItemRecipePresenter itemRecipePresenter;
        [SerializeField] private CraftRecipeItemListViewer craftRecipeItemListViewer;
        [SerializeField] private PlayerInventoryPresenter playerInventoryPresenter;
        [SerializeField] private PlayerInventorySlots playerInventorySlots;
        [SerializeField] private PlayerInventoryItemNamePresenter playerInventoryItemNamePresenter;
        [SerializeField] private RecipeViewerItemNamePresenter recipeViewerItemNamePresenter;
        [SerializeField] private QuestViewerObject questViewerObject;
        
        [SerializeField] private BlockPlacePreview blockPlacePreview;
        [SerializeField] private OreMapTileClickDetect oreMapTileClickDetect;
        [SerializeField] private SaveButton saveButton;
        [SerializeField] private BackToMainMenu backToMainMenu;

        [SerializeField] private PlayerInventorySlotsInputControl playerInventorySlotsInputControl;

        void Start()
        {
            var builder = new ContainerBuilder();
            //?????????????????????????????????????????????
            var singlePlayInterface = new SinglePlayInterface(ServerConst.ServerModsDirectory);
            builder.RegisterInstance(singlePlayInterface);
            builder.RegisterInstance(singlePlayInterface.QuestConfig);
            builder.RegisterInstance(singlePlayInterface.ItemConfig);
            builder.RegisterInstance(new ModDirectory(ServerConst.ServerModsDirectory));    
            
            //??????????????????????????????????????????????????????
            builder.RegisterInstance(new ServerProcessSetting(isLocal,localServerProcess));
            builder.RegisterInstance(new ConnectionServerConfig(IPAddress,Port));
            builder.RegisterInstance(new PlayerConnectionSetting(PlayerId));
            builder.RegisterEntryPoint<ConnectionServer>();
            builder.Register<SocketInstanceCreate, SocketInstanceCreate>(Lifetime.Singleton);
            builder.Register<AllReceivePacketAnalysisService, AllReceivePacketAnalysisService>(Lifetime.Singleton);
            builder.Register<ISocketSender, SocketSender>(Lifetime.Singleton);

            //????????????????????????????????????
            builder.Register<ReceiveChunkDataEvent>(Lifetime.Singleton);
            builder.Register<ReceiveMainInventoryEvent>(Lifetime.Singleton);
            builder.Register<ReceiveCraftingInventoryEvent>(Lifetime.Singleton);
            builder.Register<ReceiveBlockInventoryEvent>(Lifetime.Singleton);
            builder.Register<ReceiveGrabInventoryEvent>(Lifetime.Singleton);
            builder.Register<ReceiveInitialHandshakeProtocol>(Lifetime.Singleton); //?????????????????????????????????????????????
            builder.Register<ReceiveQuestDataEvent>(Lifetime.Singleton);
            
            //????????????????????????????????????
            builder.RegisterEntryPoint<RequestEventProtocol>(); //??????????????????????????????????????????????????????RegisterEntryPoint?????????
            builder.RegisterEntryPoint<InitialHandshakeProtocol>(); //????????????????????????????????????RegisterEntryPoint?????????
            builder.Register<SendPlayerPositionProtocolProtocol>(Lifetime.Singleton);
            builder.Register<RequestPlayerInventoryProtocol>(Lifetime.Singleton);
            builder.Register<SendPlaceHotBarBlockProtocol>(Lifetime.Singleton);
            builder.Register<RequestBlockInventoryProtocol>(Lifetime.Singleton);
            builder.Register<SendCommandProtocol>(Lifetime.Singleton);
            builder.Register<SendCraftProtocol>(Lifetime.Singleton);
            builder.Register<SendBlockInventoryOpenCloseControlProtocol>(Lifetime.Singleton);
            builder.Register<SendBlockRemoveProtocol>(Lifetime.Singleton);
            builder.Register<SendMiningProtocol>(Lifetime.Singleton);
            builder.Register<SendSaveProtocol>(Lifetime.Singleton);
            builder.Register<InventoryMoveItemProtocol>(Lifetime.Singleton);
            builder.Register<RequestQuestProgressProtocol>(Lifetime.Singleton);
            builder.Register<SendEarnQuestRewardProtocol>(Lifetime.Singleton);

            //?????????????????????UI??????????????????
            builder.Register<PlayerInventoryViewModel>(Lifetime.Singleton);
            builder.Register<PlayerInventoryViewModelController>(Lifetime.Singleton);
            builder.RegisterComponent(playerInventorySlotsInputControl);
            builder.RegisterComponent(playerInventoryPresenter);
            
            //?????????????????????????????????
            builder.RegisterEntryPoint<ChunkDataPresenter>();
            builder.RegisterEntryPoint<WorldMapTilePresenter>();
            builder.RegisterEntryPoint<DeleteBlockDetectToSendPacket>();
            builder.RegisterEntryPoint<MainInventoryViewPresenter>();
            builder.RegisterEntryPoint<CraftingInventoryViewPresenter>();
            builder.RegisterEntryPoint<BlockInventoryViewPresenter>();
            builder.RegisterEntryPoint<BlockInventoryRequestPacketSend>();
            builder.RegisterEntryPoint<PlayerInventoryRequestPacketSend>();
            builder.RegisterEntryPoint<PlayerInventoryMoveItemPacketSend>();
            builder.RegisterEntryPoint<CraftPacketSend>();
            builder.RegisterEntryPoint<PlayerPositionSender>();
            builder.RegisterEntryPoint<QuestUIPresenter>();
            
            //???????????????
            builder.Register<MoorestechInputSettings>(Lifetime.Singleton);
            
            //UI??????????????????
            builder.Register<UIStateDictionary>(Lifetime.Singleton);
            builder.Register<BlockInventoryState>(Lifetime.Singleton);
            builder.Register<GameScreenState>(Lifetime.Singleton);
            builder.Register<PauseMenuState>(Lifetime.Singleton);
            builder.Register<PlayerInventoryState>(Lifetime.Singleton);
            builder.Register<DeleteObjectInventoryState>(Lifetime.Singleton);
            builder.Register<BlockPlaceState>(Lifetime.Singleton);
            builder.Register<RecipeViewState>(Lifetime.Singleton);
            builder.Register<QuestViewerState>(Lifetime.Singleton);

            //mod????????????????????????????????????
            builder.Register<ItemImages>(Lifetime.Singleton);
            builder.Register<WorldMapTileMaterials>(Lifetime.Singleton);
            builder.Register<BlockObjects>(Lifetime.Singleton);
            
            

            //ScriptableObject?????????
            builder.RegisterInstance(worldMapTileObject);

            //Hierarchy????????????component
            builder.RegisterComponent(nothingIndexBlock);
            
            builder.RegisterComponent(chunkBlockGameObjectDataStore);
            builder.RegisterComponent(worldMapTileGameObjectDataStore);
            
            builder.RegisterComponent(oreMapTileClickDetect);
            builder.RegisterComponent(mainCamera);
            builder.RegisterComponent(groundPlane);
            builder.RegisterComponent(detectGroundClickToSendBlockPlacePacket);
            builder.RegisterComponent(grabbedItemImagePresenter);
            builder.RegisterComponent(commandUIInput);
            builder.RegisterComponent(hotBarItemView);
            builder.RegisterComponent(selectHotBarControl);
            builder.RegisterComponent(selectHotBarView);
            builder.RegisterComponent(itemRecipeView);
            builder.RegisterComponent(QuestUI);
            
            builder.RegisterComponent(uIStateControl);
            builder.RegisterComponent(loadingFinishDetector);
            builder.RegisterComponent(playerInventoryObject);
            builder.RegisterComponent(blockInventoryObject);
            builder.RegisterComponent(pauseMenuObject);
            builder.RegisterComponent(deleteBarObject);
            builder.RegisterComponent(recipeViewerObject);
            builder.RegisterComponent(saveButton);
            builder.RegisterComponent(backToMainMenu);
            builder.RegisterComponent(itemRecipePresenter);
            builder.RegisterComponent(craftRecipeItemListViewer);
            builder.RegisterComponent(playerInventorySlots);
            builder.RegisterComponent(playerInventoryItemNamePresenter);
            builder.RegisterComponent(recipeViewerItemNamePresenter);
            builder.RegisterComponent(questViewerObject);
            
            
            builder.RegisterComponent<IPlayerPosition>(playerPosition);
            builder.RegisterComponent<IBlockClickDetect>(blockClickDetect);
            builder.RegisterComponent<IBlockPlacePreview>(blockPlacePreview);
            
            



            //?????????????????????
            _resolver = builder.Build();
            _resolver.Resolve<ChunkBlockGameObjectDataStore>();
            _resolver.Resolve<DetectGroundClickToSendBlockPlacePacket>();
            _resolver.Resolve<CommandUIInput>();
            _resolver.Resolve<UIStateControl>();
            _resolver.Resolve<LoadingFinishDetector>();

        }

        protected override void OnDestroy()
        {
            _resolver.Dispose();
        }
    }
}
