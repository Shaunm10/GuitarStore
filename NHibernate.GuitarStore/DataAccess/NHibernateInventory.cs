// -----------------------------------------------------------------------
// <copyright file="NHibernateInventory.cs" company="The Advisory Board Company">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------


using NHibernate.Impl;

namespace NHibernate.GuitarStore.DataAccess
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using NHibernate.Criterion;
    using NHibernate.GuitarStore.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class NHibernateInventory : NHibernateBase
    {
        public NHibernateInventory()
        {

        }

        /// <summary>
        /// Get's a list of Inventory items ordered by a property specified.
        /// </summary>
        /// <param name="orderBy">The property to order by</param>
        /// <returns>A list of Inventory items.</returns>
        public IList<Inventory> ExecuteICriteriaOrderBy(string orderBy)
        {
            using (ITransaction transaction = Session.BeginTransaction())
            {
                try
                {
                    IList<Inventory> result = Session.CreateCriteria<Inventory>()
                        .AddOrder(Order.Asc(orderBy))
                        .List<Inventory>();

                    transaction.Commit();
                    return result;

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Get's a list of Inventory items from a given TypeId.
        /// </summary>
        /// <param name="typeId">The typeId to be filtered by.</param>
        /// <returns>A list of inventory items.</returns>
        //public IList<Inventory> ExecuteICriteria(Guid typeId)
        //{
        //    using (ITransaction transaction = Session.BeginTransaction())
        //    {
        //        try
        //        {
        //            var result = Session.CreateCriteria<Inventory>()
        //                .Add(Restrictions.Eq("TypeId", typeId))
        //                .List<Inventory>();

        //            transaction.Commit();
        //            return result;
        //        }
        //        catch (Exception ex)
        //        {
        //            transaction.Rollback();
        //            throw;
        //        }
        //    }
        //}

        public bool DeleteInventoryItem(Guid id)
        {
            using (ITransaction transaction = Session.BeginTransaction())
            {
                try
                {
                    IQuery query = Session.CreateQuery("from Inventory where Id = :Id")
                        .SetGuid("Id", id);

                    // assumes 1 and only 1 entity will be returned.
                    var inventroy = query.List<Inventory>()[0];

                    Session.Delete(inventroy);
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        /// <summary>
        /// Get's a set of dynamic inventory objects.
        /// </summary>
        /// <param name="typeId">Optional parameter of typeId.</param>
        /// <param name="maxResult">Optional parameter of of the max results count.</param>
        /// <param name="firstResult">Optional parameter of the first result count.</param>
        /// <returns>A tuple containing the first property(int) of with a count of results, the second property a list of inventory.</returns>
        public Tuple<int, IList> GetDynamicInventory(Guid? typeId = null, int? maxResult = null, int? firstResult = null)
        {
            // always create a transaction
            using (var transaction = Session.BeginTransaction())
            {
                var hqlQuery = new StringBuilder();
                var rowCountQuery = new StringBuilder();

                // add the select clause
                hqlQuery.Append("select Id, Builder, Model, Qoh, Cost, Price, Recieved, Profit from Inventory ");
                rowCountQuery.Append("select count(*) from Inventory ");

                // if the typeId has been supplied, add it as a filter.
                if (typeId.HasValue)
                {
                    hqlQuery.Append(" where TypeId = :TypeId");
                    rowCountQuery.Append(" where TypeId = :TypeId");

                }

                // always add the order by clause.
                hqlQuery.Append(" order by Builder");

                try
                {
                    // this query will get the actual inventory results.
                    IQuery query = Session.CreateQuery(hqlQuery.ToString());

                    // query to get the inventory count
                    IQuery countQuery = Session.CreateQuery(rowCountQuery.ToString());

                    // if a maxResult was supplied
                    if (maxResult.HasValue)
                    {
                        query.SetMaxResults(maxResult.Value);
                    }

                    // if a firstResult was supplied
                    if (firstResult.HasValue)
                    {
                        query.SetFirstResult(firstResult.Value);
                    }

                    // if the typeId has been supplied, be sure to add this parameter to the HQL statement.
                    if (typeId.HasValue)
                    {
                        query.SetGuid("TypeId", typeId.Value);
                        countQuery.SetGuid("TypeId", typeId.Value);
                    }

                    IMultiQuery multiQuery = Session.CreateMultiQuery()
                        .Add("result", query)
                        .Add("RowCount", countQuery);

                    var rowCountResultList = multiQuery.GetResult("RowCount");
                    int rowCount = 0;

                    // if we got a list back
                    if (rowCountResultList is IList)
                    {
                        var rowCount64 = (Int64)((IList)rowCountResultList)[0];

                        rowCount = Convert.ToInt32(rowCount64);
                    }

                    var resultList = (IList)multiQuery.GetResult("result");

                    return new Tuple<int, IList>(rowCount, resultList);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchParameter"></param>
        /// <returns></returns>
        public IList ExecuteDetachedQuery(string searchParameter)
        {
            using (ITransaction transaction = Session.BeginTransaction())
            {
                try
                {
                    string hqlQuery = "select Builder, Model, Price, Id" +
                                  " from Inventory " +
                                  " where Model like :search " +
                                  " order by Builder";
                    IDetachedQuery detachedQuery = new DetachedQuery(hqlQuery)
                        .SetString("search", searchParameter);

                    IQuery executableQuery = detachedQuery.GetExecutableQuery(Session);
                    return executableQuery.List();

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Executes a named query
        /// </summary>
        /// <param name="queryName">The name of the named query</param>
        /// <returns>The result set.</returns>
        public IList ExecuteNamedQuery(string queryName)
        {
            using (var tran = Session.BeginTransaction())
            {
                try
                {
                    var query = Session.GetNamedQuery(queryName);
                    return query.List();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Get's a count of inventory items.
        /// </summary>
        /// <returns>The count of inventory items in the database.</returns>
        //public int GetTotalInventoryCount()
        //{
        //    using (var transaction = Session.BeginTransaction())
        //    {
        //        try
        //        {
        //            IQuery query = Session.CreateQuery("select count(*) from Inventory");

        //            var countOfInventoryItems = query.UniqueResult();

        //            return Convert.ToInt32(countOfInventoryItems);

        //        }
        //        catch (Exception ex)
        //        {
        //           transaction.Rollback();
        //            throw;
        //        }
        //    }
        //}
    }
}
