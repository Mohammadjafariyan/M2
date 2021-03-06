using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using AppSourceGenerator;
using Engine.Controllers.AbstractControllers.AttributeBased;
using Engine.Entities.Models.Core.AppGeneration;
using ServiceLayer.Systems;
using ViewModel.Parameters;
using WebAppIDEEngine.Models;

namespace Engine.Areas.AppGeneration.Controllers
{
    public class ExportController : AppController<SubSystem, CommonParameter>
    {
        IGenerator g = new MvcProjectGenerator();
  
        public ExportController()
        {
            this._engineService = new SubSystemService();
        }


        [HttpPost]
        public ActionResult Export(string filepath, long subsystemId,
            int type)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                ViewBag.alertmsg = "مسیر را انتخاب کنید";
                return View("GetDataTable");
            }


            try
            {
                using (var EngineContext = new EngineContext())
                {
                    
                    var path = filepath;
                    g.CreateIsNotExist(path);
                    g.LoadProject(path);

                    var subsystem = EngineContext.SubSystem.Where(d => d.Id == subsystemId).ToList();

                    if (type == 1)
                    {
                        g.MakeSubsystems(subsystem);
                    
                        var d = subsystem.Select(s => s.DefineControllers.ToList()).FirstOrDefault();
                        g.MakeControllers(d);
                   
                        var d1 = subsystem.Select(s => s.DefineServices.ToList()).FirstOrDefault();
                        g.MakeServices(d1);
                   
                        var d3 = subsystem.Select(s => s.DefineControllers.ToList()).FirstOrDefault();
                        g.MakeApiControllers(d3);

                        g.MakeViews(d3);

                        var d4 = EngineContext.Models.ToList();
                        g.MakeModels(d4);


                        g.RegisterServices(d1, "IBaseEngineService");

                        ViewBag.successmsg = "با موفقیت ایجاد شد";
                        return View("GetDataTable",null);

                    }
                    else if (type == 2)
                    {
                        throw new Exception("این نوع پیاده سازی نشده است");
                    }
                    else
                    {
                        throw new Exception("ساپورت نشده");
                    }

                }

            }
            catch (Exception e)
            {
                ViewBag.alertmsg = e.Message;
                return View("GetDataTable");
            }
        }
    }
}