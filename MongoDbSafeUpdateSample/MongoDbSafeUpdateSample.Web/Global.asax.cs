using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using AutoMapper;
using MongoDbSafeUpdateSample.Web.Data;
using MongoDbSafeUpdateSample.Web.Resources;

namespace MongoDbSafeUpdateSample.Web
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            // AutoMapper Config
            Mapper.CreateMap<Category, CategoryResource>();
        }
    }
}