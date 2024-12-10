using IPA.Loader;
using System.Linq;

namespace Bugsmash.Utilities;

internal static class PluginUtil
{
    public static bool IsAnyPluginsInstalled(string[] plugins) =>
        plugins.Where(x => PluginManager.GetPluginFromId(x) != null).Any();
}
