﻿using LCL.Core.Domain.Services;
using LCL.Core.Infrastructure;
using LCL.Web.Framework.Mvc;
namespace LCL.Web.Framework
{
    public class LResourceDisplayName : System.ComponentModel.DisplayNameAttribute, IModelAttribute
    {
        private string _resourceValue = string.Empty;

        public LResourceDisplayName(string resourceKey)
            : base(resourceKey)
        {
            ResourceKey = resourceKey;
        }

        public string ResourceKey { get; set; }

        public override string DisplayName
        {
            get
            {
                var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
                return localizationService.GetResource(ResourceKey);
            }
        }

        public string Name
        {
            get { return "LResourceDisplayName"; }
        }
    }
}
