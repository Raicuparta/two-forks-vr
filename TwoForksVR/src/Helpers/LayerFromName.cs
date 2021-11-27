using UnityEngine;

namespace TwoForksVr.Helpers
{
    public static class LayerFromName
    {
        // TODO: rewrite this to be helpful for culling masks too, not just individual layers.
    
        public static readonly int UI = LayerMask.NameToLayer("UI");
        public static readonly int MenuBackground = LayerMask.NameToLayer("MenuBackground");
        public static readonly int PlayerBody = 17;
    }
}
