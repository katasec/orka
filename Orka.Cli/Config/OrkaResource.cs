using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orka.Cli.Config;

public class OrkaResource
{
    public string Name { get; set; } = "default";
    public string Provider { get; set; } = "";
    public Dictionary<string, string> Inputs { get; set; } = new();
    public List<string> DependsOn { get; set; } = new();
}