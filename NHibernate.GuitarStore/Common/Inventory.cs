// -----------------------------------------------------------------------
// <copyright file="Inventory.cs" company="The Advisory Board Company">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NHibernate.GuitarStore.Common
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Inventory
    {
        public virtual Guid Id { get; set; }
        public virtual Guid TypeId { get; set; }
        public virtual string Builder { get; set; }
        public virtual string Model { get; set; }
        public virtual int? Qoh { get; set; }
        public virtual decimal? Cost { get; set; }
        public virtual decimal? Price { get; set; }
        public virtual DateTime? Recieved { get; set; } 
        public virtual decimal? Profit { get; set; }
    }
}
