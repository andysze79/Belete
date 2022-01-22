using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Sirenix.OdinInspector;

namespace Belete
{
    public class GameManager : MonoBehaviour, Belete.IGameStateEvent
    {
        public enum GameState {
            Adventure, Conversation, ViewingUI
        }
        public GameState m_CurrentGameState = GameState.Adventure;
        public List<Item> m_PlayerInventory = new List<Item>();
        [FoldoutGroup("Asset Bundle Settings")] public string m_SciptableObjAssetBundlePath = "Assets/AssetBundles/scriptableobj.itemlibrary";
        [FoldoutGroup("Asset Bundle Settings")] public string m_ItemLibraryName = "ItemLibrary";
        [FoldoutGroup("Asset Bundle Settings")] [ReadOnly] public AssetBundle SciptableObjAssetBundle;
        [FoldoutGroup("Asset Bundle Settings")] public ItemLibrary m_ItemLibrary;        
        [FoldoutGroup("Asset Bundle Settings")] public AudioLibrary m_AudioLibrary;
        [FoldoutGroup("Asset Bundle Settings")] public NoteBookPageLibrary m_NotebookPageLibrary;
        public GlobalSettings m_GlobalSettings;
        public AudioManager m_AudioManager;
        public UIManager m_UIManager;
        public FlowchartExecutor m_FlowchartExecutor;
        public LayerMask m_MouseHoverRaycastLayer;
        public Camera MainCam { get; set; }
        [SerializeField] private IInteractable CurrentInteractableObj;// { get; set; }        
        private bool activeCheckInteractiveEvent = true;
        private bool activeCheckHoverEvent = true;

        private static GameManager m_Instance;
        public static GameManager Instance
        {
            get
            {
                if (m_Instance != null)
                    return m_Instance;
                else
                {
                    m_Instance = GameObject.FindObjectOfType<GameManager>();
                    return m_Instance;
                }
            }

        }
        private List<IGameStateEvent> NotifyMembers = new List<IGameStateEvent>();
        public CinemachineBrain MainCamera { get; set; }
        public ItemTradingActions ItemTradingActions { get;set;}
        
        private void Awake()
        {
            MainCam = Camera.main;
            MainCamera = Camera.main.GetComponent<CinemachineBrain>();
            ItemTradingActions = GetComponent<ItemTradingActions>();
            if(m_ItemLibrary == null) LoadAssetsBundle();
            m_UIManager = GameManager.FindObjectOfType<UIManager>();
            m_FlowchartExecutor = GameObject.FindObjectOfType<FlowchartExecutor>();
            
            AddNotifyMember(this);

            GameStateAdventure();
        }
        private void OnEnable()
        {
            InputManager.Instance.GetInteractAValue += CheckInteract;

            EventHandler.WhenStartConversation += GameStateConversation;
            EventHandler.WhenEndConversation += GameStateAdventure;
            EventHandler.WhenEndConversation += ResetCurrentInteractableObj;

            EventHandler.WhenEndPuzzleConversation += ResetCurrentInteractableObj;

            EventHandler.WhenEndPuzzleInvestigation += GameStateViewingUI;
            EventHandler.WhenEndPuzzleInvestigation += ResetCurrentInteractableObj;

            m_UIManager.WhenUIChange += ChangeGameState;
            m_UIManager.m_UIOrganizer.WhenUIChange += ChangeGameState;
        }
        private void OnDisable()
        {
            InputManager.Instance.GetInteractAValue -= CheckInteract;

            EventHandler.WhenStartConversation -= GameStateConversation;
            EventHandler.WhenEndConversation -= GameStateAdventure;
            EventHandler.WhenEndConversation -= ResetCurrentInteractableObj;

            EventHandler.WhenEndPuzzleConversation -= ResetCurrentInteractableObj;

            EventHandler.WhenEndPuzzleInvestigation -= GameStateViewingUI;
            EventHandler.WhenEndPuzzleInvestigation -= ResetCurrentInteractableObj;

            m_UIManager.WhenUIChange -= ChangeGameState;
            m_UIManager.m_UIOrganizer.WhenUIChange -= ChangeGameState;
        }
        private void Update()
        {
            if (activeCheckHoverEvent)
                CheckHoverEvent();
        }
        private void LoadAssetsBundle() {
            SciptableObjAssetBundle = AssetBundle.LoadFromFile(m_SciptableObjAssetBundlePath);            

            if (SciptableObjAssetBundle == null)
            {
                Debug.Log("Failed to load AssetBundle!");
                return;
            }

            var asset = SciptableObjAssetBundle.LoadAsset<ItemLibrary>(m_ItemLibraryName);
            print(asset.name);
            m_ItemLibrary = asset;
        }
        #region observer
        public void AddNotifyMember(IGameStateEvent member) {
            NotifyMembers.Add(member);
        }
        public void RemoveNotifyMember(IGameStateEvent member) {
            if(NotifyMembers.Contains(member))
                NotifyMembers.Remove(member);
        }
        private void CallNotifyMembers(GameState state) {
            for (int i = 0; i < NotifyMembers.Count; i++)
            {
                NotifyMembers[i].OnNotifyGameStateChanged(state);
            }
        }
        #endregion
        #region Game State Control
        private void ChangeGameState(GameState state)
        {            
            m_CurrentGameState = state;
            CallNotifyMembers(m_CurrentGameState);
        }
        private void GameStateConversation() {
            m_CurrentGameState = GameState.Conversation;
            CallNotifyMembers(m_CurrentGameState);
        }
        private void GameStateAdventure()
        {
            m_CurrentGameState = GameState.Adventure;
            CallNotifyMembers(m_CurrentGameState);
        }
        private void GameStateViewingUI()
        {
            m_CurrentGameState = GameState.ViewingUI;
            CallNotifyMembers(m_CurrentGameState);
        }
        void IGameStateEvent.OnNotifyGameStateChanged(GameManager.GameState state)
        {
            switch (state)
            {
                case GameManager.GameState.Adventure:
                    ActivateInteractiveFunction();
                    break;
                case GameManager.GameState.Conversation:
                    DeactivateInteractiveFunction();                    
                    break;
                case GameManager.GameState.ViewingUI:
                    ActivateInteractiveFunction();
                    break;
                default:
                    break;
            }
        }
        #endregion
        #region Flow Chart Func
        public void CheckUseItem()
        {
            if (CurrentInteractableObj == null) return;
            
            m_UIManager.SwitchUseItemUI(true);
        }
        #endregion
        #region Screen Input Module
        private void CheckInteract(bool value)
        {
            if(!activeCheckInteractiveEvent) return;
            if (!value || CurrentInteractableObj == null) return;

            CurrentInteractableObj.OnInteract();
            //ChangeInteractableObj(null);
        }        
        private bool CheckHoverEvent()
        {
            var mouseInput = Input.mousePosition;
            RaycastHit hitInfo;
            var raycastResult = Physics.Raycast(MainCam.ScreenPointToRay(mouseInput), out hitInfo, 1000, m_MouseHoverRaycastLayer);
            var result = false;

            Debug.DrawLine(
                MainCam.ScreenPointToRay(mouseInput).origin,
                MainCam.ScreenPointToRay(mouseInput).origin + 100 * MainCam.ScreenPointToRay(mouseInput).direction,
                Color.red);

            if (raycastResult)
            {
                if (!hitInfo.collider.TryGetComponent(out IInteractable interactable))
                {
                    ChangeInteractableObj(null);
                    return false;
                }

                ChangeInteractableObj(interactable);
                interactable.OnHoverEnter();

                result = true;
            }
            else
            {
                // When exit from Hover
                ChangeInteractableObj(null);
            }

            return result;
        }
        private void ResetCurrentInteractableObj() {
            if (CurrentInteractableObj != null) CurrentInteractableObj.OnHoverExit();
            CurrentInteractableObj = null;
        }
        public void ChangeInteractableObj(IInteractable target)
        {
            if (CurrentInteractableObj != null) CurrentInteractableObj.OnHoverExit();
            CurrentInteractableObj = target;
        }
        private void ActivateInteractiveFunction()
        {
            activeCheckInteractiveEvent = true;
            activeCheckHoverEvent = true;
        }
        private void DeactivateInteractiveFunction() { 
            activeCheckInteractiveEvent = false;
            activeCheckHoverEvent = false;        
        }
        #endregion
    }
    public interface IGameStateEvent {
        void OnNotifyGameStateChanged(GameManager.GameState state);
    }
}
