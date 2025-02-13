using UnityEngine;

using MFPC.Utils;
using MFPC.Input;
using MFPC.Input.PlayerInput;
using MFPC.Input.SettingsInput;

namespace MFPC
{
    public class MFPCBootstrap : MonoBehaviour
    {
        [SerializeField] private InputConfig inputConfigData;
        [SerializeField] private PlayerInputTuner playerInputTuner;
        [SerializeField] private SettingsInputTuner settingsInputTuner;
        [SerializeField] private Player player;
        [SerializeField] private Settings settings;

        private ReactiveProperty<InputType> currentInputType;

        private void Awake()
        {
            currentInputType = new ReactiveProperty<InputType>(inputConfigData.GetCurrentInputType());
            
            playerInputTuner.Initialize(currentInputType);
            player.Initialize(playerInputTuner);
            settings.Initialize(playerInputTuner, settingsInputTuner, currentInputType);
        }
    }
}