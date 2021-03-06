﻿using Engine.Attributes;
using Engine.DomainLayer.Models.Core.QueryBuild;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Engine.Entities.Models.Core.AppGeneration;
using Engine.Entities.Models.Core.QueryBuild;
using WebAppIDEEngine.Models.Core.QueryBuild;
using WebAppIDEEngine.Models.CoreEnum;
using WebAppIDEEngine.Models.ICore;

namespace WebAppIDEEngine.Models.Core
{

    /// <summary>
    /// کوئری که از سمت انگولار می آید
    /// </summary>
    public class Query : BaseEntity
    {
        public override string Name { get; set; }
        public string LinQ { get; set; }
        public string LinQJoin { get; set; }
        public QueryType QueryType { get; set; }
        public virtual ICollection<Action> Actions{ get; set; }
        //public ICollection<Where> Wheres { get; set; }
        //public ICollection<Sort> Sorts { get; set; }
        //public ICollection<Result> Results { get; set; }
        //public ICollection<SelectColumn> SelectColumns { get; set; }
        //public ICollection<Parameter> Parameters { get; set; }

        public Query()
        {
            this.Actions=new List<Action>();
            this.ServiceMethods = new List<ServiceMethod>();
            this.addParameterFields = new List<AddParameterForm>();
         //   this.joinTables = new List<JoinTable>();
            this.models = new List<QueryModel>();
            this.selectedProperties = new List<QueryProperty>();
            WhereComputeButtons=new List<ComputeButton>();
            
        }


        /// <summary>   
        /// نوع کوئری
        /// </summary>
        public QueryViewModelType type { get; set; }
        
        /// <summary>
        /// جداول استفاده شده
        /// </summary>
        public virtual ICollection<QueryModel> models { get; set; }

        /// <summary>
        /// پروپرتی های انتخاب شده
        /// </summary>
        public virtual ICollection<QueryProperty> selectedProperties { get; set; }

        
        /// <summary>
        /// بوتن های محاسباتی کوئری در قسمت فیلتر
        /// </summary>
        public virtual ICollection<ComputeButton> WhereComputeButtons { get; set; }

        
        
        /*/// <summary>
        /// جدول اصلی
        /// </summary>
        public virtual QueryModel mainTable { get; set; }


        /// <summary>
        /// جدول اصلی
        /// </summary>
        public long? mainTableId { get; set; }*/


        [NotMapped]
        /// <summary>
        /// جداول جوین شده
        /// </summary>
        public virtual ICollection<JoinTable> joinTables { get; set; }



        /// <summary>
        /// پارامتر های تعریف شده
        /// </summary>
        public virtual ICollection<AddParameterForm> addParameterFields { get; set; }
        public virtual ICollection<ServiceMethod> ServiceMethods { get; set; }
        
        
        
        public string WhereStatement { get; set; }
        public string SQL { get; set; }
        public string queryName { get; set; }
    }
}
