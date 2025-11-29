using System;
using System.Collections.Generic;

namespace Cai_San_Thu_Vien.Models;

public partial class Item
{
    public int IId { get; set; }

    public string IName { get; set; } = null!;

    public string? IImg { get; set; }

    public int? IPrice { get; set; }

    public int? IKind { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<Quest> Quests { get; set; } = new List<Quest>();

    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}
