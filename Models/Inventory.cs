using System;
using System.Collections.Generic;

namespace Cai_San_Thu_Vien.Models;

public partial class Inventory
{
    public int InId { get; set; }

    public int PId { get; set; }

    public int IId { get; set; }

    public int? Quantity { get; set; }

    public virtual Item IIdNavigation { get; set; } = null!;

    public virtual Play PIdNavigation { get; set; } = null!;
}
