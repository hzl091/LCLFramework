﻿

using LCL.Core.Plugins;
namespace LCL.Web.Framework.ViewEngines
{
    public interface IPluginViewEngineFactory
    {
        IPluginViewEngine CreateViewEngine(IPlugin bundle);
    }

    public class PluginRazorViewEngineFactory : IPluginViewEngineFactory
    {
        public IPluginViewEngine CreateViewEngine(IPlugin bundle)
        {
            return new PluginRazorViewEngine(bundle);
        }
    }

    public class PluginWebFormViewEngineFactory : IPluginViewEngineFactory
    {
        public IPluginViewEngine CreateViewEngine(IPlugin bundle)
        {
            return new PluginWebFormViewEngine(bundle);
        }
    }
}
