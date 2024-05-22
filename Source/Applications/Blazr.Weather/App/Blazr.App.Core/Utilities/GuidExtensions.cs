using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazr.App.Core;

public static class GuidExtensions
{
    public static string ToDisplayId(this Guid value)
        => value.ToString().Substring(0,8);
}
