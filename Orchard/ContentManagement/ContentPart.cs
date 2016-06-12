using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.Records;
using Orchard.ContentManagement.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Orchard.ContentManagement
{
    public class ContentPart : IContent
    {
        public virtual ContentItem ContentItem { get; set; }

        /// <summary>
        /// The ContentItem's identifier.
        /// </summary>
        [HiddenInput(DisplayValue = false)]
        public int Id
        {
            get { return ContentItem.Id; }
        }

        public ContentTypeDefinition TypeDefinition { get { return ContentItem.TypeDefinition; } }
        public ContentTypePartDefinition TypePartDefinition { get; set; }

        public string PartName { get { return TypePartDefinition.PartName; } }

        public SettingsDictionary Settings { get { return TypePartDefinition.Settings; } }
    }

    public class ContentPart<TRecord> : ContentPart
    {
        //static protected bool IsVersionableRecord { get; private set; }

        //static ContentPart()
        //{
        //    IsVersionableRecord = typeof(TRecord).IsAssignableTo<ContentItemVersionRecord>();
        //}

        //protected TProperty Retrieve<TProperty>(Expression<Func<TRecord, TProperty>> targetExpression)
        //{
        //    return InfosetHelper.Retrieve(this, targetExpression);
        //}

        //protected TProperty Retrieve<TProperty>(
        //    Expression<Func<TRecord, TProperty>> targetExpression,
        //    Func<TRecord, TProperty> defaultExpression)
        //{

        //    return InfosetHelper.Retrieve(this, targetExpression, defaultExpression);
        //}
        //protected TProperty Retrieve<TProperty>(
        //            Expression<Func<TRecord, TProperty>> targetExpression,
        //            TProperty defaultValue)
        //{

        //    return InfosetHelper.Retrieve(this, targetExpression, (Func<TRecord, TProperty>)(x => defaultValue));
        //}



        public readonly LazyField<TRecord> _record = new LazyField<TRecord>();
        public TRecord Record { get { return _record.Value; } set { _record.Value = value; } }
    }
}
