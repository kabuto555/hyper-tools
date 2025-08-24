using TMPro;
using UnityEngine;

namespace HyperTools.Popups
{
    public class SimplePopup : BasePopup<SimplePopup.Config>
    {
        [SerializeField] private TMP_Text titleLabel;
        [SerializeField] private TMP_Text messageLabel;
        [SerializeField] private TMP_Text buttonText;
        
        public class Config
        {
            public string Title;
            public string Message;
            public string ButtonText;
        }

        public override void Initialize(Config config)
        {
            base.Initialize(config);
            
            titleLabel.text = config.Title;
            messageLabel.text = config.Message;
            buttonText.text = config.ButtonText;
        }
    }
}
