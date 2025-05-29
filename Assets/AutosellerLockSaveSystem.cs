// AutosellerLockSaveSystem.cs
using UnityEngine;
using CI.QuickSave;

[RequireComponent(typeof(AutosellerLock))]
public class AutosellerLockSaveSystem : MonoBehaviour
{
    [Tooltip("A unique key for this lock (e.g. MachineA, FrontGate).")]
    [SerializeField] private string saveKey = "DefaultLock";

    private const string ROOT = "AutosellerLocks";

    private AutosellerLock lockComp;

    [SerializeField] private bool reloadSave;

    private void Start()
    {

        lockComp = GetComponent<AutosellerLock>();


        if (reloadSave)
        { SaveState(); }

        // 1) Load saved state
        var fullRoot = $"{ROOT}_{saveKey}";
        if (QuickSaveReader.RootExists(fullRoot))
        {
            var reader = QuickSaveReader.Create(fullRoot);
            int  savedProgress  = reader.Read<int>("Progress");
            bool savedCompleted = reader.Read<bool>("Completed");
            lockComp.SetState(savedProgress, savedCompleted);
        }

        // 2) Subscribe to events to save on change
        lockComp.OnProgressChanged += SaveState;
        lockComp.OnCompleted       += SaveState;
    }

    /// <summary>
    /// Writes current lock state into QuickSave immediately.
    /// </summary>
    private void SaveState(int dummy = 0)
    {
        var fullRoot = $"{ROOT}_{saveKey}";
        var writer   = QuickSaveWriter.Create(fullRoot);

        // currentProgress & completion
        // We need to access lockComp's private _currentProgress/_isCompleted:
        // Either make public getters, or reflect:
        var progressField = typeof(AutosellerLock)
             .GetField("_currentProgress", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var completedField = typeof(AutosellerLock)
             .GetField("_isCompleted", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        int  prog = (int)progressField.GetValue(lockComp);
        bool comp = (bool)completedField.GetValue(lockComp);

        writer.Write("Progress",  prog)
              .Write("Completed", comp)
              .Commit();
    }

    private void OnDestroy()
    {
        // Unsubscribe
        lockComp.OnProgressChanged -= SaveState;
        lockComp.OnCompleted       -= SaveState;
    }
}
