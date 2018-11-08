﻿using Engine.DomainLayer.Models.Core.QueryBuild;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using WebAppIDEEngine.Models;
using WebAppIDEEngine.Models.Core;
using WebAppIDEEngine.Models.Core.QueryBuild;

namespace Engine.Areas.ReportGenerator.Controllers
{
    public class QueryController : ApiController
    {
        public EngineContext _context = new EngineContext();

        public void DeleteByIdHelper(long id)
        {
            var indb = _context.Queries.Where(q => q.Id == id).FirstOrDefault();
            if (indb == null)
                throw new Exception("پیدا نشد");


            if (indb.Actions.Count > 0)
                throw new Exception("این مورد در اکشن ها استفاده شده است");

            if (indb.ServiceMethods.Count > 0)
                throw new Exception("این مورد در متد سرویس ها اسفتاده شده است");

            // right props of right joins
            DeleteList(indb.models.SelectMany(j =>
                j.RightJoinTables.Where(r => r.rightProperty != null).Select(r => r.rightProperty)).ToList());

            // left props of right joins
            DeleteList(indb.models
                .SelectMany(j => j.RightJoinTables.Where(r => r.leftProperty != null).Select(r => r.leftProperty))
                .ToList());

            // left props of left joins
            DeleteList(indb.models
                .SelectMany(j => j.LeftJoinTables.Where(r => r.leftProperty != null).Select(r => r.leftProperty))
                .ToList());

            // right props of left joins
            DeleteList(indb.models.SelectMany(j =>
                j.LeftJoinTables.Where(r => r.rightProperty != null).Select(r => r.rightProperty)).ToList());

            // join tables
            DeleteList(indb.models.SelectMany(j => j.RightJoinTables).ToList());
            DeleteList(indb.models.SelectMany(j => j.LeftJoinTables).ToList());


            DeleteList(indb.addParameterFields);
            DeleteList(indb.selectedProperties);


            var queryProperties = _context.QueryProperties.Where(p => p.QueryId == indb.Id);
            foreach (var queryProperty in queryProperties)
            {
                _context.Entry(queryProperty).State = EntityState.Deleted;
            }

            DeleteList(indb.models);
            _context.Entry(indb).State = EntityState.Deleted;


            _context.SaveChanges();
        }

        [HttpPost]
        [Route("~/api/Query/DeleteById")]
        public CustomResult DeleteById(long id)
        {
            using (var tr = _context.Database.BeginTransaction())
            {
                try
                {
                    DeleteByIdHelper(id);
                    tr.Commit();
                    return new CustomResult {Message = "با موفقیت انجام شد ", Status = CustomResultType.success};
                }
                catch (Exception e)
                {
                    tr.Rollback();
                    return new CustomResult {Message = e.Message + "خطا در عملیات ", Status = CustomResultType.fail};
                }
            }
        }

        [HttpGet]
        [Route("~/api/Query/GetAll")]
        public CustomResult GetAll(int? lastIndex, int? count)
        {
            // خخالی بود تمامی را برگردان
            if (lastIndex == null || count == null)
                return new CustomResult
                {
                    result = _context.Queries.Select(q => new
                            {Id = q.Id, Name = q.Name, queryName = q.queryName, QueryType = q.QueryType, type = q.type})
                        .ToList(),
                    Status = CustomResultType.success
                };

            // مواظب خطا ها باش
            count = count == 0 ? 10 : count;
            var dt = _context.Queries.Select(q => new
                {Id = q.Id, Name = q.Name, queryName = q.queryName, QueryType = q.QueryType, type = q.type});
            if (lastIndex == 0)
            {
                dt = dt.Take(count.Value);
            }
            else
            {
                dt = dt.Skip(lastIndex.Value).Take(count.Value);
            }

            // با paging برگردان
            return new CustomResult {result = dt, Status = CustomResultType.success};
        }

        [HttpGet]
        [Route("~/api/Query/LoadQuery")]
        public CustomResult LoadQuery(long id)
        {
            try
            {
                var finded = _context.Queries.Find(id);
                if (finded == null)
                    throw new Exception("کوئری یافت نشد");


                finded.selectedProperties = finded.selectedProperties.Where(s => s.onOutPut).ToList();
                var json = Newtonsoft.Json.JsonConvert.SerializeObject
                    (finded, new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
                finded = JsonConvert.DeserializeObject<Query>(json);
                return new CustomResult {result = finded, Status = CustomResultType.success};
            }
            catch (Exception e)
            {
                return new CustomResult {Message = e.Message + " خطا در عملیات ", Status = CustomResultType.fail};
            }
        }


        public void SetState<T>(List<T> ts, EntityState state) where T : class
        {
            foreach (var lp in ts)
            {
                _context.Entry(lp).State = state;
            }
        }


        public Query InsertQurey(Query q,EngineContext _engineContext)
        {
            var query = _context.Queries.Create();
            _engineContext.Queries.Add(query);
            _engineContext.Entry(query).CurrentValues.SetValues(q);

            foreach (var queryModel in q.models)
            {
                var _queryModel = new QueryModel();
                query.models.Add(queryModel);

                /*var ljoins=queryModel.LeftJoinTables.ToList();
                var rjoins=queryModel.RightJoinTables.ToList();
                
                ljoins.AddRange(rjoins);
                
                foreach (var joinTable in ljoins)
                {
                    _queryModel.RightJoinTables.Add(joinTable);
                    joinTable.leftTable = q.models.First(m => m.uniqId == joinTable.leftTableUniqId);
                    joinTable.rightTable = q.models.First(m => m.uniqId == joinTable.rightTableUniqId);

                    if (joinTable.rightProperty != null)
                    {
                        joinTable.rightProperty.PropertyId = joinTable.rightProperty.Property.Id;
                        joinTable.rightProperty.Property = null;
                    }

                    if (joinTable.leftProperty != null)
                    {
                        joinTable.leftProperty.PropertyId = joinTable.leftProperty.Property.Id;
                        joinTable.leftProperty.Property = null;
                    }
                }


                foreach (var joinTable in queryModel.LeftJoinTables.ToList())
                {
                    _queryModel.LeftJoinTables.Add(joinTable);
                    joinTable.leftTable = q.models.First(m => m.uniqId == joinTable.leftTableUniqId);
                    joinTable.rightTable = q.models.First(m => m.uniqId == joinTable.rightTableUniqId);

                    if (joinTable.rightProperty != null)
                    {
                        joinTable.rightProperty.PropertyId = joinTable.rightProperty.Property.Id;
                        joinTable.rightProperty.Property = null;
                    }

                    if (joinTable.leftProperty != null)
                    {
                        joinTable.leftProperty.PropertyId = joinTable.leftProperty.Property.Id;
                        joinTable.leftProperty.Property = null;
                    }
                }*/
            }

            var ljoins=q.models.SelectMany(m=>m.LeftJoinTables).ToList();
            var rjoins=q.models.SelectMany(m=>m.RightJoinTables).ToList();
            
            ljoins.AddRange(rjoins);

            var joins=ljoins.DistinctBy(l => l.uniqId).ToList();
                
            foreach (var joinTable in joins)
            {
                _queryModel.RightJoinTables.Add(joinTable);
                joinTable.leftTable = q.models.First(m => m.uniqId == joinTable.leftTableUniqId);
                joinTable.rightTable = q.models.First(m => m.uniqId == joinTable.rightTableUniqId);

                if (joinTable.rightProperty != null)
                {
                    joinTable.rightProperty.PropertyId = joinTable.rightProperty.Property.Id;
                    joinTable.rightProperty.Property = null;
                }

                if (joinTable.leftProperty != null)
                {
                    joinTable.leftProperty.PropertyId = joinTable.leftProperty.Property.Id;
                    joinTable.leftProperty.Property = null;
                }
            }

            if (!q.models.Any(m => m.IsMainTable))
                throw new Exception("جدول اصلی داده نشده است");

            if (q.models.Count(m => m.IsMainTable) > 1)
                throw new Exception("جدول اصلی بیش از یک است");
            foreach (var addParameterField in q.addParameterFields)
            {
                query.addParameterFields.Add(addParameterField);
            }

            foreach (var querySelectedProperty in q.selectedProperties)
            {
                query.selectedProperties.Add(querySelectedProperty);
            }

            return query;
        }

        private void InsertQureyBackup(Query q)
        {
            var query = _context.Queries.Create();
            _context.Queries.Add(query);
            _context.Entry(query).CurrentValues.SetValues(q);

            foreach (var queryModel in q.models)
            {
                var _queryModel = new QueryModel();
                query.models.Add(queryModel);

                foreach (var joinTable in queryModel.RightJoinTables.ToList())
                {
                    _queryModel.RightJoinTables.Add(joinTable);
                    joinTable.leftTable = q.models.First(m => m.uniqId == joinTable.leftTableUniqId);
                    joinTable.rightTable = q.models.First(m => m.uniqId == joinTable.rightTableUniqId);


                    if (joinTable.rightProperty != null)
                    {
                        joinTable.rightProperty.PropertyId = joinTable.rightProperty.Property.Id;
                        joinTable.rightProperty.Property = null;
                    }

                    if (joinTable.leftProperty != null)
                    {
                        joinTable.leftProperty.PropertyId = joinTable.leftProperty.Property.Id;
                        joinTable.leftProperty.Property = null;
                    }
                }


                foreach (var joinTable in queryModel.LeftJoinTables.ToList())
                {
                    _queryModel.LeftJoinTables.Add(joinTable);
                    joinTable.leftTable = q.models.First(m => m.uniqId == joinTable.leftTableUniqId);
                    joinTable.rightTable = q.models.First(m => m.uniqId == joinTable.rightTableUniqId);

                    if (joinTable.rightProperty != null)
                    {
                        joinTable.rightProperty.PropertyId = joinTable.rightProperty.Property.Id;
                        joinTable.rightProperty.Property = null;
                    }

                    if (joinTable.leftProperty != null)
                    {
                        joinTable.leftProperty.PropertyId = joinTable.leftProperty.Property.Id;
                        joinTable.leftProperty.Property = null;
                    }
                }
            }

            if (!q.models.Any(m => m.IsMainTable))
                throw new Exception("جدول اصلی داده نشده است");

            if (q.models.Count(m => m.IsMainTable) > 1)
                throw new Exception("جدول اصلی بیش از یک است");
            foreach (var addParameterField in q.addParameterFields)
            {
                query.addParameterFields.Add(addParameterField);
            }

            foreach (var querySelectedProperty in q.selectedProperties)
            {
                query.selectedProperties.Add(querySelectedProperty);
            }

            // return query;
        }

        [HttpPost]
        [Route("~/api/Query/SaveQuery")]
        public CustomResult SaveQuery(Query query)
        {
            try
            {
                if (query.Id == 0)
                {
                    query = InsertQurey(query,_context);
                    _context.SaveChanges();
                }
                else
                {
                    var indb = _context.Queries.Where(q => q.Id == query.Id).FirstOrDefault();
                    if (indb == null)
                        throw new Exception("پیدا نشد");


                    if (indb.Actions.Count > 0)
                        throw new Exception("این مورد در اکشن ها استفاده شده است");

                    if (indb.ServiceMethods.Count > 0)
                        throw new Exception("این مورد در متد سرویس ها اسفتاده شده است");


                    DeleteByIdHelper(query.Id);
                    _context.SaveChanges();

                    using (var db = new EngineContext())
                    {
                        query.Id = 0;
                        query = InsertQurey(query,db);
                        db.Database.Log = (s) => { Debug.Write(s); };
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                return new CustomResult {Message = e.Message + "خطا در عملیات ", Status = CustomResultType.fail};
            }

            return new CustomResult
                {Message = "عملیات با موفقیت انجام شد ", Status = CustomResultType.success, result = query.Id};
        }


        public void DeleteList<T>(ICollection<T> list) where T : class
        {
            foreach (var item in list.ToList())
            {
                _context.Entry<T>(item).State = EntityState.Deleted;
            }
        }
    }
}