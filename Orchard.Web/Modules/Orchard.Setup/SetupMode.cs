using Autofac;
using System;
using Orchard.Caching;
using Orchard.ContentManagement.Records;
using Orchard.Core.Settings.Services;
using Orchard.Data.Migration;
using Orchard.Data.Migration.Interpreters;
using Orchard.Data.Providers;
using Orchard.Environment;
using Orchard.Environment.Extensions.Models;
using Orchard.Mvc;
using Orchard.Mvc.Filters;
using Orchard.Mvc.ModelBinders;
using Orchard.Mvc.Routes;
using Orchard.Mvc.ViewEngines;
using Orchard.Mvc.ViewEngines.Razor;
using Orchard.Mvc.ViewEngines.ThemeAwareness;
using Orchard.Settings;
using Orchard.Tasks;
using Orchard.UI;
using Orchard.UI.Notify;
using Orchard.UI.Resources;
using Orchard.Data;
using Orchard.Environment.Descriptor;

namespace Orchard.Setup
{
    public class SetupMode : Autofac.Module
    {
        public Feature Feature { get; set; }

        protected override void Load(ContainerBuilder builder)
        {

            // standard services needed in setup mode
            builder.RegisterModule(new MvcModule());
            //builder.RegisterModule(new CommandModule());
            builder.RegisterModule(new WorkContextModule());
            builder.RegisterModule(new CacheModule());

            builder.RegisterType<RoutePublisher>().As<IRoutePublisher>().InstancePerLifetimeScope();
            builder.RegisterType<SessionLocator>().As<ITransactionManager>().InstancePerLifetimeScope();

            builder.RegisterType<ModelBinderPublisher>().As<IModelBinderPublisher>().InstancePerLifetimeScope();
            builder.RegisterType<RazorViewEngineProvider>().As<IViewEngineProvider>().SingleInstance();
            

            //As<IShapeTemplateViewEngine>().SingleInstance();

            //builder.RegisterType<ThemedViewResultFilter>().As<IFilterProvider>().InstancePerLifetimeScope();
            //builder.RegisterType<ThemeFilter>().As<IFilterProvider>().InstancePerLifetimeScope();
            //builder.RegisterType<PageTitleBuilder>().As<IPageTitleBuilder>().InstancePerLifetimeScope();
            //builder.RegisterType<PageClassBuilder>().As<IPageClassBuilder>().InstancePerLifetimeScope();
            builder.RegisterType<Notifier>().As<INotifier>().InstancePerLifetimeScope();
            builder.RegisterType<NotifyFilter>().As<IFilterProvider>().InstancePerLifetimeScope();
            builder.RegisterType<DataServicesProviderFactory>().As<IDataServicesProviderFactory>().InstancePerLifetimeScope();
            //builder.RegisterType<DefaultCommandManager>().As<ICommandManager>().InstancePerLifetimeScope();
            //builder.RegisterType<HelpCommand>().As<ICommandHandler>().InstancePerLifetimeScope();
            //builder.RegisterType<WorkContextAccessor>().As<IWorkContextAccessor>().InstancePerMatchingLifetimeScope("shell");
            builder.RegisterType<ResourceManager>().As<IResourceManager>().InstancePerLifetimeScope();
            builder.RegisterType<ResourceFilter>().As<IFilterProvider>().InstancePerLifetimeScope();
            builder.RegisterType<DefaultOrchardShell>().As<IOrchardShell>().InstancePerMatchingLifetimeScope("shell");
            builder.RegisterType<SweepGenerator>().As<ISweepGenerator>().SingleInstance();

            // setup mode specific implementations of needed service interfaces
            //builder.RegisterType<SafeModeText>().As<IText>().InstancePerLifetimeScope();
            builder.RegisterType<SafeModeSiteService>().As<ISiteService>().InstancePerLifetimeScope();

            builder.RegisterType<DefaultDataMigrationInterpreter>().As<IDataMigrationInterpreter>().InstancePerLifetimeScope();
            builder.RegisterType<DataMigrationManager>().As<IDataMigrationManager>().InstancePerLifetimeScope();

            //builder.RegisterType<RecipeHarvester>().As<IRecipeHarvester>().InstancePerLifetimeScope();
            //builder.RegisterType<RecipeParser>().As<IRecipeParser>().InstancePerLifetimeScope();

            builder.RegisterType<DefaultCacheHolder>().As<ICacheHolder>().SingleInstance();

            builder.RegisterType<DisplayHelper>().As<IDisplayHelper>().InstancePerLifetimeScope();
            builder.RegisterType<ShellDescriptorCache>().As<IShellDescriptorCache>().SingleInstance();

            // in progress - adding services for display/shape support in setup
            //builder.RegisterType<DisplayHelperFactory>().As<IDisplayHelperFactory>().InstancePerLifetimeScope();
            //builder.RegisterType<DefaultDisplayManager>().As<IDisplayManager>().InstancePerLifetimeScope();
            //builder.RegisterType<DefaultShapeFactory>().As<IShapeFactory>().InstancePerLifetimeScope();
            //builder.RegisterType<DefaultShapeTableManager>().As<IShapeTableManager>().InstancePerLifetimeScope();
            //builder.RegisterType<ShapeTableLocator>().As<IShapeTableLocator>().InstancePerMatchingLifetimeScope("work");

            builder.RegisterType<ThemeAwareViewEngine>().As<IThemeAwareViewEngine>().InstancePerLifetimeScope();
            //builder.RegisterType<LayoutAwareViewEngine>().As<ILayoutAwareViewEngine>().InstancePerLifetimeScope();
            builder.RegisterType<ConfiguredEnginesCache>().As<IConfiguredEnginesCache>().SingleInstance();
            //builder.RegisterType<LayoutWorkContext>().As<IWorkContextStateProvider>().InstancePerLifetimeScope();
            builder.RegisterType<SafeModeSiteWorkContextProvider>().As<IWorkContextStateProvider>().InstancePerLifetimeScope();

            //builder.RegisterType<ShapeTemplateBindingStrategy>().As<IShapeTableProvider>().InstancePerLifetimeScope();
            //builder.RegisterType<BasicShapeTemplateHarvester>().As<IShapeTemplateHarvester>().InstancePerLifetimeScope();
            //builder.RegisterType<ShapeAttributeBindingStrategy>().As<IShapeTableProvider>().InstancePerMatchingLifetimeScope("shell");
            //builder.RegisterModule(new ShapeAttributeBindingModule());
        }


        //class SafeModeText : IText
        //{
        //    public LocalizedString Get(string textHint, params object[] args)
        //    {
        //        if (args == null || args.Length == 0)
        //        {
        //            return new LocalizedString(textHint);
        //        }
        //        return new LocalizedString(string.Format(textHint, args));
        //    }
        //}

        class SafeModeSiteWorkContextProvider : IWorkContextStateProvider
        {
            public Func<WorkContext, T> Get<T>(string name)
            {
                if (name == "CurrentSite")
                {
                    ISite safeModeSite = new SafeModeSite();
                    return ctx => (T)safeModeSite;
                }
                return null;
            }
        }

        class SafeModeSiteService : ISiteService
        {
            public ISite GetSiteSettings()
            {
                //var siteType = new ContentTypeDefinitionBuilder().Named("Site").Build();
                //var site = new ContentItemBuilder(siteType)
                //    .Weld<SafeModeSite>()
                //    .Build();  
                //return site.As<ISite>();


                return new SafeModeSite();
            }
        }

        class SafeModeSite : ContentItem, ISite
        {

            public string SiteName
            {
                get { return "Orchard Setup"; }
            }

            public string SiteSalt
            {
                get { return "42"; }
            }

            public string SiteUrl
            {
                get { return "/"; }
            }

            public string SuperUser
            {
                get { return ""; }
                set { throw new NotImplementedException(); }
            }

            public string HomePage
            {
                get { return ""; }
                set { throw new NotImplementedException(); }
            }


            public ResourceDebugMode ResourceDebugMode
            {
                get { return ResourceDebugMode.FromAppSetting; }
                set { throw new NotImplementedException(); }
            }


            public int PageSize
            {
                get { return 10; }
                set { throw new NotImplementedException(); }
            }

            public int MaxPageSize
            {
                get { return 100; }
                set { throw new NotImplementedException(); }
            }

            public int MaxPagedCount
            {
                get { return 0; }
                set { throw new NotImplementedException(); }
            }

            public string BaseUrl
            {
                get { return ""; }
            }

            public string SiteCulture
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }
        }

    }
}