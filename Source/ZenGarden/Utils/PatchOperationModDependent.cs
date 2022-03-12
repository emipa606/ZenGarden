using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Verse;

namespace ZenGarden;

internal class PatchOperationModDependent : PatchOperation
{
    private List<string> modList;
    private string modName;

    protected override bool ApplyWorker(XmlDocument xml)
    {
        if (!modList.NullOrEmpty())
        {
            return ApplyWorker_Multiple();
        }

        if (modName.NullOrEmpty())
        {
            return false;
        }

        return ApplyWorker_Single();
    }


    private bool ApplyWorker_Multiple()
    {
        foreach (var name in modList)
        {
            if (ModsConfig.ActiveModsInLoadOrder.All(mod => mod.Name != name))
            {
                return false;
            }
        }

        return true;
    }


    private bool ApplyWorker_Single()
    {
        return ModsConfig.ActiveModsInLoadOrder.Any(mod => mod.Name == modName);
    }
}