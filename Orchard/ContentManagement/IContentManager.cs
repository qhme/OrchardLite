using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.ContentManagement
{
    public interface IContentManager : IDependency
    {
        IEnumerable<ContentTypeDefinition> GetContentTypeDefinitions();

        /// <summary>
        /// Instantiates a new content item with the specified type
        /// </summary>
        /// <remarks>
        /// The content item is not yet persisted!
        /// </remarks>
        /// <param name="contentType">The name of the content type</param>
        ContentItem New(string contentType);


        /// <summary>
        /// Creates (persists) a new content item
        /// </summary>
        /// <param name="contentItem">The content instance filled with all necessary data</param>
        void Create(ContentItem contentItem);


        /// <summary>
        /// Gets the content item with the specified id
        /// </summary>
        /// <param name="id">Numeric id of the content item</param>
        ContentItem Get(int id);

        //IEnumerable<T> GetMany<T>(IEnumerable<int> ids, VersionOptions options, QueryHints hints) where T : class, IContent;
        //IEnumerable<T> GetManyByVersionId<T>(IEnumerable<int> versionRecordIds, QueryHints hints) where T : class, IContent;
        //IEnumerable<ContentItem> GetManyByVersionId(IEnumerable<int> versionRecordIds, QueryHints hints);

        void Remove(ContentItem contentItem);

        /// <summary>
        /// Permanently deletes the specified content item, including all of its content part records.
        /// </summary>
        void Destroy(ContentItem contentItem);


        /// <summary>
        /// Clears the current referenced content items
        /// </summary>
        void Clear();

        /// <summary>
        /// Query for arbitrary content items
        /// </summary>
        IContentQuery<ContentItem> Query();

        //IHqlQuery HqlQuery();

        //ContentItemMetadata GetItemMetadata(IContent contentItem);
        //IEnumerable<GroupInfo> GetEditorGroupInfos(IContent contentItem);
        //IEnumerable<GroupInfo> GetDisplayGroupInfos(IContent contentItem);
        //GroupInfo GetEditorGroupInfo(IContent contentItem, string groupInfoId);
        //GroupInfo GetDisplayGroupInfo(IContent contentItem, string groupInfoId);

        //ContentItem ResolveIdentity(ContentIdentity contentIdentity);

        /// <summary>
        /// Builds the display shape of the specified content item
        /// </summary>
        /// <param name="content">The content item to use</param>
        /// <param name="displayType">The display type (e.g. Summary, Detail) to use</param>
        /// <param name="groupId">Id of the display group (stored in the content item's metadata)</param>
        /// <returns>The display shape</returns>
        string BuildDisplay(IContent content, string displayType = "");

        /// <summary>
        /// Builds the editor shape of the specified content item
        /// </summary>
        /// <param name="content">The content item to use</param>
        /// <param name="groupId">Id of the editor group (stored in the content item's metadata)</param>
        /// <returns>The editor shape</returns>
        string BuildEditor(IContent content);

        /// <summary>
        /// Updates the content item and its editor shape with new data through an IUpdateModel
        /// </summary>
        /// <param name="content">The content item to update</param>
        /// <param name="updater">The updater to use for updating</param>
        /// <param name="groupId">Id of the editor group (stored in the content item's metadata)</param>
        /// <returns>The updated editor shape</returns>
        string UpdateEditor(IContent content, IUpdateModel updater);
    }

    public interface IContentDisplay : IDependency
    {
        string BuildDisplay(IContent content, string displayType = "");
        string BuildEditor(IContent content);
        string UpdateEditor(IContent content, IUpdateModel updater);
    }
}
