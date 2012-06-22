// -----------------------------------------------------------------------
// <copyright file="SQLInterceptor.cs" company="The Advisory Board Company">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------


using System;

namespace NHibernate.GuitarStore.DataAccess.Utilities
{
    using NHibernate.SqlCommand;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>  
    [Serializable]
    public class SQLInterceptor : EmptyInterceptor, IInterceptor
    {
        SqlString IInterceptor.OnPrepareStatement(NHibernate.SqlCommand.SqlString sql)
        {
            Utils.NHibernateGeneratedSQL = sql.ToString();
            Utils.QueryCounter++;
            return sql;
        }
    }
}
