using System;
using System.Collections.Generic;

namespace Cai_San_Thu_Vien.Models;

public partial class Craft
{
    public int CId { get; set; }

    public int PId { get; set; }

    public int RcId { get; set; }

    public DateOnly? Time { get; set; }

    public virtual Play PIdNavigation { get; set; } = null!;

    public virtual Recipe Rc { get; set; } = null!;
}
