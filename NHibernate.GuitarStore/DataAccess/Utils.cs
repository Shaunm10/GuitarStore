// -----------------------------------------------------------------------
// <copyright file="Utils.cs" company="The Advisory Board Company">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------


namespace NHibernate.GuitarStore.DataAccess
{
    using NHibernate.AdoNet.Util;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Utils
    {
        public static string NHibernateGeneratedSQL { get; set; }
        public static int QueryCounter { get; set; }

        public static string FormatSQL()
        {
            BasicFormatter formatter = new BasicFormatter();
            return formatter.Format(NHibernateGeneratedSQL.ToUpper());
        }
    }
}
