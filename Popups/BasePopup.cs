using System;
using UnityEngine;
using UnityEngine.UI;

namespace HyperTools.Popups
{
    public class BasePopup : MonoBehaviour
    {
        [SerializeField] protected Button scrimButton;
        [SerializeField] protected Button closeButton;
        
        public event Action OnClose;
        
        protected virtual void Awake()
        {
            if (scrimButton != null)
            {
                scrimButton.onClick.AddListener(ClosePopup);
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(ClosePopup);
            }
        }
        
        protected virtual void ClosePopup()
        {
            OnClose?.Invoke();
            Destroy(gameObject);
        }
    }

    public abstract class BasePopup<TConfig> : BasePopup
    {
        protected TConfig PopupConfig;
        
        public virtual void Initialize(TConfig config)
        {
            PopupConfig = config;
        }
    }
    
    public abstract class BasePopup<TConfig, TResult> : BasePopup<TConfig>
    {
        protected TResult Result;

        public event Action<TResult> OnCloseWithResult;

        public override void Initialize(TConfig config)
        {
            base.Initialize(config);
            Result = default;
        }

        protected override void ClosePopup()
        {
            OnCloseWithResult?.Invoke(Result);
            base.ClosePopup();
        }
    }
}
