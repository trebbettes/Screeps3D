using Screeps3D.RoomObjects;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Screeps3D.Tools.Selection
{
    public class EditFlagPopup : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _flagName;
        [SerializeField] private FlagColorToggle _primaryFlagColor;
        [SerializeField] private FlagColorToggle _secondaryFlagColor;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _okButton;

        private bool _newFlag;
        private Flag _flag;


        private void Start()
        {
            // TODO: listen on change and check for valid name.
            _cancelButton.onClick.AddListener(CancelClicked);
            _okButton.onClick.AddListener(OkClicked);
            _flagName.onDeselect.AddListener(FlagNameDeselect);

            _primaryFlagColor.OnColorChange += PrimaryColorChange;
            _secondaryFlagColor.OnColorChange += SecondaryColorChange;
        }

        public void Load(Flag flag, bool newFlag = false)
        {
            _flag = flag;
            _newFlag = newFlag;

            // TODO: store initial values so we can reset the flag on cancel.

            if (newFlag)
            {
                // TODO: generate new valid name.
            }
            else
            {

                _flagName.text = flag.Name;
                _primaryFlagColor.SetColor((Constants.FlagColor)_flag.PrimaryColor);
                _secondaryFlagColor.SetColor((Constants.FlagColor)_flag.SecondaryColor);
            }
        }

        public void Save()
        {
            // TODO: call api to save flag changes
        }

        private void PrimaryColorChange(Constants.FlagColor flagColor)
        {
            Debug.Log("PrimaryColorChange");
        }
        private void SecondaryColorChange(Constants.FlagColor flagColor)
        {
            Debug.Log("SecondaryColorChange");
        }


        private void FlagNameDeselect(string text)
        {
            Debug.Log("FlagNameDeselect");
        }

        private void CancelClicked()
        {
            Debug.Log("Cancel Clicked");
        }

        private void OkClicked()
        {
            Debug.Log("Ok Clicked");
        }
    }
}