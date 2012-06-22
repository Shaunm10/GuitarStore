// -----------------------------------------------------------------------
// <copyright file="AuditDeleteEvent.cs" company="The Advisory Board Company">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------



namespace NHibernate.GuitarStore.DataAccess.Utilities
{
    using System;
    using log4net;
    using NHibernate.Event;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class AuditDeleteEvent : IPostDeleteEventListener
    {
        private readonly ILog log;

        public AuditDeleteEvent()
        {
            log = LogManager.GetLogger("DeleteAuditLog");
        }

        public void OnPostDelete(PostDeleteEvent @event)
        {
            log.Info(@event.Id.ToString() + " has been deleted.");
        }
    }
}
