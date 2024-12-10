using IPA.Loader;
using System.Linq;

namespace Bugsmash.Utilities;

internal static class HarmonyUtil
{
    public static bool IsDisallowedPluginsInstalled(string[] plugins) =>
        plugins.Where(x => PluginManager.GetPluginFromId(x) != null).Any();
}
