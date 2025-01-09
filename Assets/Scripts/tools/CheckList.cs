using System.Collections.Generic;
using System.Text;

public class CheckList
{
    private Dictionary<string, bool> list;

    public CheckList(List<string> ids)
    {
        list = new Dictionary<string, bool>();
        foreach (string id in ids)
        {
            list.Add(id, false);
        }
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        foreach (string key in list.Keys)
        {
            //if (list[key]) sb.AppendLine($" - {key})");
            string isCheck = list[key]?"Placé":"Non placé";
            sb.AppendLine($" - {key} ({isCheck})");
        }
        return sb.ToString();
    }

    public bool IsAllChecked()
    {
        return !list.ContainsValue(false);
    }

    public void CheckID(string id)
    {
        if (list.ContainsKey(id)) list[id] = true;
    }

    public void ToggleID(string id)
    {
        if (list.ContainsKey(id)) list[id] ^= true;
    }
}
