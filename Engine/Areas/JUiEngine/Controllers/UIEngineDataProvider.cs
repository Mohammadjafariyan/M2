using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Engine.Entities.Models.Core.AppGeneration;
using Engine.Entities.Models.UiGeneratorModels;
using Engine.Service.AbstractControllers;
using Engine.Utitliy;
using ViewModel.ActionTypes;
using WebAppIDEEngine.Models;
using WebAppIDEEngine.Models.Core.QueryBuild;
using WebAppIDEEngine.Models.CoreEnum;
using WebAppIDEEngine.Models.UiGeneratorModels;

namespace Engine.Areas.JUiEngine.Controllers
{
    public interface IUiEngineDataProvider
    {
        EjTable GetTable(string tableName,
            ViewDataDictionary ViewData, HttpRequestBase Request, string SubSystemName, string ControllerName,
            string ControllerMethod,
            string ServiceMethodName, IDataTable debatable = null);

        void GetTable(EjTable table,
            ViewDataDictionary viewData, HttpRequestBase request, string subSystemName, string controllerName,
            string controllerMethod, IDataTable debatable = null);
    }

    public class UiEngineDataProvider : IUiEngineDataProvider
    {
        private Injector _injector = new Injector();
        private UiFormDataProvider _uiFormprovider = new UiFormDataProvider();
        
        public void GetTable(EjTable table,
            ViewDataDictionary viewData, HttpRequestBase request, string subSystemName, string controllerName,
            string controllerMethod, IDataTable debatable = null)
        {

                    viewData[UiHomeController.ApiActionURL] = request?.ApplicationPath +
                                                              $@"{subSystemName}/api/{controllerName}/{
                                                                      controllerMethod
                                                                  }" +
                                                              request?.Url?.Query;
                    viewData[UiHomeController.ActionURL] = request?.ApplicationPath +
                                                           $@"{subSystemName}/{controllerName}/{controllerMethod}" +
                                                           request?.Url?.Query;
                    viewData[UiHomeController.TableObject] = table;
                    viewData[UiHomeController.DataTable] = debatable;
                    viewData[UiHomeController.UiTableItems] = table.UiTableItems;

        }
        
        
        public EjTable GetTable(string tableName,
            ViewDataDictionary viewData, HttpRequestBase Request, string SubSystemName, string ControllerName,
            string ControllerMethod,
            string ServiceMethodName, IDataTable debatable = null)
        {
            try
            {
                if (string.IsNullOrEmpty(tableName))
                {
                    throw new Exception("tableName is null");
                }

                tableName = tableName.ToLower().TrimEnd();


                using (var db = new EngineContext())
                {
                    var table = db.Tables.Include("TableMethods")
                        .Include("UiTableForms")
                        .Include("UiTableItems").Include("UiTableItems.UiItem")
                        .FirstOrDefault(t => t.Name.ToLower().TrimEnd() == tableName);
                    if (table == null)
                        throw new UiEngineException("جدول یافت نشد");


                    var searchForm = table.UiTableForms.FirstOrDefault();
                    if (searchForm != null)
                        _uiFormprovider.GetForm(searchForm.Name, viewData, isTableForm: true,
                            postType: UiFormControllerMethodType.Search);



                    if (debatable == null)
                    {
                        var methodId = table.TableMethods.Select(t => t.DefineControllerMethodId).First();

                        debatable = CallServiceMethod(methodId, Request.Form, Request.Params,
                            out SubSystemName, out ControllerName, out ControllerMethod
                            , out ServiceMethodName);
                    }


                    ControllerName = ControllerName.Replace("Controller", "");

                    viewData[UiHomeController.ApiActionURL] = Request.ApplicationPath +
                                                              $@"{SubSystemName}/api/{ControllerName}/{
                                                                      ControllerMethod
                                                                  }" +
                                                              Request.Url.Query;
                    viewData[UiHomeController.ActionURL] = Request.ApplicationPath +
                                                           $@"{SubSystemName}/{ControllerName}/{ControllerMethod}" +
                                                           Request.Url.Query;
                    viewData[UiHomeController.TableObject] = table;
                    viewData[UiHomeController.DataTable] = debatable;
                    viewData[UiHomeController.UiTableItems] = table.UiTableItems;

                    return table;
                }
            }
            catch (UiEngineException e)
            {
                throw e;
            }
        }

        public dynamic CallServiceMethod(long methodId, NameValueCollection formParams,
            NameValueCollection requestParams, out string SubSystemName, out string ControllerName,
            out string ControllerMethod, out string ServiceMethodName)
        {
            DefineControllerMethod method = null;
            string serviceName;
            List<AddParameterForm> queryAddParameterFields;

            //اطلاعات از دیتابیس
            using (var db = new EngineContext())
            {
                method = db.DefineControllerMethodes.Find(methodId);
                if (method == null)
                    throw new UiEngineException("جدول یافت نشد");

                serviceName = method.ServiceMethod.DefineService.Name;
                queryAddParameterFields =
                    method.ServiceMethod.Query.addParameterFields.ToList();

                SubSystemName = method.DefineController.SubSystem.Name;
                ControllerName = method.DefineController.Name;
                ControllerMethod = method.Name;
                ServiceMethodName = method.Name;
            }


            // ورودی های متد را از فرم ارسال شده یا از کوئری استرینگ می گیرد
            foreach (var field in queryAddParameterFields)
            {
                var name = field.nameInMethod.ToLower().TrimEnd();
                var formInput = formParams[name];
                var queryParam = requestParams[name];


                var val = formInput ?? queryParam;
                int n;

                var parsedVal = DetermineTypeAndGetValue(val, field);

                field.Value = parsedVal;
            }


            foreach (var field in queryAddParameterFields)
            {
                if (field.nullable && field.Value == null)
                {
                    throw new Exception("مقدار دهی درست نیست ، پارامتر نال پاس شده است که Nullable نیست");
                }
            }

            var vals = queryAddParameterFields.OrderBy(q => q.Order).Select(q => q.Value).ToList();

            // کال
            return InjectAndCall(serviceName, method.Name, vals.ToArray());
        }

        private object DetermineTypeAndGetValue(string val, AddParameterForm field)
        {
            object o = null;
            var isparsed = false;
            if (string.IsNullOrEmpty(val))
                return val;

            switch (field.typeInModel)
            {
                case PropertyType.Boolean:
                    bool d;
                    isparsed = bool.TryParse(val, out d);
                    o = d;
                    break;
                case PropertyType.Byte:
                    byte d1;
                    isparsed = byte.TryParse(val, out d1);
                    o = d1;
                    break;
                case PropertyType.Char:
                    char d2;
                    isparsed = char.TryParse(val, out d2);
                    o = d2;
                    break;
                case PropertyType.Decimal:
                    decimal d3;
                    isparsed = decimal.TryParse(val, out d3);
                    o = d3;
                    break;
                case PropertyType.Double:
                    double d4;
                    isparsed = double.TryParse(val, out d4);
                    o = d4;
                    break;
                case PropertyType.Int:
                    int d6;
                    isparsed = int.TryParse(val, out d6);
                    o = d6;
                    break;
                case PropertyType.Int16:
                    Int16 d7;
                    isparsed = Int16.TryParse(val, out d7);
                    o = d7;
                    break;
                case PropertyType.Int64:
                    Int64 d8;
                    isparsed = Int64.TryParse(val, out d8);
                    o = d8;
                    break;
                case PropertyType.Single:
                    Single d9;
                    isparsed = Single.TryParse(val, out d9);
                    o = d9;
                    break;
                case PropertyType.String:
                    o = val;
                    break;
                case PropertyType.ByteArray:
                    throw new NotImplementedException();
                    break;
                case PropertyType.TimeSpan:
                    TimeSpan d10;
                    isparsed = TimeSpan.TryParse(val, out d10);
                    o = d10;
                    break;

                case PropertyType.DateTime:
                    DateTime d11;
                    isparsed = DateTime.TryParse(val, out d11);
                    o = d11;
                    break;

                case PropertyType.Long:
                    long d12;
                    isparsed = long.TryParse(val, out d12);
                    o = d12;
                    break;
            }

            if (!isparsed)
            {
                throw new Exception("isparesed is false " + field.nameInMethod);
            }

            return o;
        }

        public dynamic InjectAndCall(string serviceName, string actionName, params object[] prameters)
        {
            var service = _injector.Inject<IBaseEngineService>(serviceName) as object;

            if (service == null)
                throw new Exception("service is null");
            var methodInfo = service.GetType().GetMethod(actionName);
            if (methodInfo == null)
                throw new Exception("method is not found ");

            var res = methodInfo.Invoke(service, prameters);
            if (!(res is IDataTable))
            {
                throw new Exception("خروجی متد باید از نوع IDataTable باشد");
            }

            return res;
        }
    }


    public class UiEngineException : Exception
    {
        public UiEngineException(string msg) : base(msg)
        {
        }
    }
}