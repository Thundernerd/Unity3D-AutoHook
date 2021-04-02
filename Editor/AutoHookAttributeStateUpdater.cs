using Sirenix.OdinInspector.Editor;

namespace TNRD.Autohook
{
    public class AutoHookAttributeStateUpdater : AttributeStateUpdater<AutoHookAttribute>
    {
        public override void OnStateUpdate() => Property.State.Visible = !Attribute.HideWhenFound;
    }
}