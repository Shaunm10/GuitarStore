// -----------------------------------------------------------------------
// <copyright file="Guitar.cs" company="The Advisory Board Company">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NHibernate.GuitarStore.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Guitar
    {
        public virtual Guid Id { get; set; }
        public virtual string Type { get; set; }
        public virtual IList<Inventory> Inventory { get; set; }
    }
}
