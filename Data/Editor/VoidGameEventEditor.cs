using CoolTools.Data;
using UnityEditor;

namespace CoolTools.Editor
{
    [CustomEditor(typeof(VoidEvent))]
    public class VoidGameEventEditor : GameEventSOEditor<VoidEvent, Void>
    {
        public override void TestRaise()
        {
            gameEvent.Raise();
        }
    }
}