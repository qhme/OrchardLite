//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Web.Mvc;
//using System.Security.Cryptography;
//using Orchard.Infrastructure;
//using Orchard;
//using Orchard.Caching;

//namespace Orchard.Core.Controllers
//{
//    /// <summary>
//    /// 阻止过度请求
//    /// </summary>
//    public class PreventSpamAttribute : ActionFilterAttribute
//    {
//        // 请求的间隔时间 (秒)
//        public int DelayRequest { get; set; }

//        // 过度请求时显示的错误信息
//        public string ErrorMessage { get; set; }

//        // This will store the URL to Redirect errors to
//        public string RedirectURL;

//        public PreventSpamAttribute()
//        {
//            DelayRequest = 2;
//            ErrorMessage = "Excessive Request Attempts Detected.";
//        }

//        public override void OnActionExecuting(ActionExecutingContext filterContext)
//        {
//            var workContext = EngineContext.Current.Resolve<IWorkContext>();
//            var cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("cache_static");

//            var request = filterContext.HttpContext.Request;
//            var targetInfo = request.RequestType + "_" + request.RawUrl.ToLower() + request.QueryString;
//            var customerGuid = workContext.CurrentUser.UserName;
//            var hashValue = string.Join("", MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(customerGuid + "_" + targetInfo)).Select(s => s.ToString("x2")));

//            // Checks if the hashed value is contained in the Cache (indicating a repeat request)
//            if (cacheManager.IsSet(hashValue))
//            {
//                // Adds the Error Message to the Model
//                filterContext.Controller.ViewData.ModelState.AddModelError("ExcessiveRequests", ErrorMessage);
//            }
//            else
//            {
//                cacheManager.Set(hashValue, 1, DelayRequest);
//            }

//            base.OnActionExecuting(filterContext);
//        }
//    }
//}
