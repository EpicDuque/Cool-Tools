using CoolTools.Data;
using UnityEditor;

namespace CoolTools.Editor
{
    [CustomEditor(typeof(IntEvent))]
    public class IntGameEventEditor : GameEventSOEditor<IntEvent, int>
    {
    }
}