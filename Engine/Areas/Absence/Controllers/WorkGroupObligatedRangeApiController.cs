
using Engine.Absence.Models;

using Engine.Areas.ReportGenerator.Controllers;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Engine.Attributes;
using Engine.Controllers.AbstractControllers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ViewModel.ActionTypes;
using ViewModel.Parameters;
using WebAppIDEEngine.Models;
using WebAppIDEEngine.Models.ICore;
using System.Collections.Specialized;
using Engine.Areas.JUiEngine.Controllers;
using Engine.Entities.Models.UiGeneratorModels;
using Engine.ServiceLayer.Systems.Engine;
using Engine.Service.AbstractControllers;
using WebGrease.Css.Extensions;
using WebAppIDEEngine.Models;
using System.Web.Http;
using ServiceLayer.Absence;


namespace Engine.Areas.Absence.Controllers
{
/// <summary>
    /// WorkGroupObligatedRangeApiController
    /// WorkGroupObligatedRangeApiController
    /// </summary>
    public class WorkGroupObligatedRangeApiController :BaseApiController<WorkGroupObligatedRange,CommonParameter>
    { 
public WorkGroupObligatedRangeApiController(){ 
    _engineService=new WorkGroupObligatedRangService();

}


} 
 } 
