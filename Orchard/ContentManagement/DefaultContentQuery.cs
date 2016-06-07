using NHibernate;
using NHibernate.Criterion;
using NHibernate.Impl;
using NHibernate.Linq;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using Orchard.Caching;
using Orchard.ContentManagement.Records;
using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Orchard.ContentManagement;
using Orchard.Utility.Extensions;

namespace Orchard.ContentManagement
{
    public class DefaultContentQuery : IContentQuery
    {
        private readonly ISessionLocator _sessionLocator;
        private ISession _session;
        private ICriteria _itemCriteria;
        private ICacheManager _cacheManager;
        private ISignals _signals;
        private IRepository<ContentTypeRecord> _contentTypeRepository;

        public DefaultContentQuery(IContentManager contentManager,
            ISessionLocator sessionLocator,
            ICacheManager cacheManager,
            ISignals signals,
            IRepository<ContentTypeRecord> contentTypeRepository)
        {
            _sessionLocator = sessionLocator;
            ContentManager = contentManager;
            _cacheManager = cacheManager;
            _signals = signals;
            _contentTypeRepository = contentTypeRepository;
        }

        public IContentManager ContentManager { get; private set; }

        ISession BindSession()
        {
            if (_session == null)
                _session = _sessionLocator.For(typeof(ContentItemRecord));
            return _session;
        }

        ICriteria BindCriteriaByPath(ICriteria criteria, string path)
        {
            return criteria.GetCriteriaByPath(path) ?? criteria.CreateCriteria(path);
        }

        ICriteria BindItemCriteria()
        {
            //// [ContentItemVersionRecord] >join> [ContentItemRecord]
            //return BindCriteriaByPath(BindItemVersionCriteria(), "ContentItemRecord");
            if (_itemCriteria == null)
            {
                _itemCriteria = BindSession().CreateCriteria<ContentItemRecord>();
                _itemCriteria.SetCacheable(true);
            }
            return _itemCriteria;
        }

        ICriteria BindPartCriteria<TRecord>() where TRecord : ContentPartRecord
        {
            //if (typeof(TRecord).IsSubclassOf(typeof(ContentPartRecord)))
            //{
            //    return BindCriteriaByPath(BindItemVersionCriteria(), typeof(TRecord).Name);
            //}
            return BindCriteriaByPath(BindItemCriteria(), typeof(TRecord).Name);
        }

        private int GetContentTypeRecordId(string contentType)
        {
            return _cacheManager.Get(contentType + "_Record", ctx =>
            {
                ctx.Monitor(_signals.When(contentType + "_Record"));

                var contentTypeRecord = _contentTypeRepository.Get(x => x.Name == contentType);

                if (contentTypeRecord == null)
                {
                    //TEMP: this is not safe... ContentItem types could be created concurrently?
                    contentTypeRecord = new ContentTypeRecord { Name = contentType };
                    _contentTypeRepository.Create(contentTypeRecord);
                }

                return contentTypeRecord.Id;
            });
        }

        private void ForType(params string[] contentTypeNames)
        {
            if (contentTypeNames != null && contentTypeNames.Length != 0)
            {
                var contentTypeIds = contentTypeNames.Select(GetContentTypeRecordId).ToArray();
                // don't use the IN operator if not needed for performance reasons
                if (contentTypeNames.Length == 1)
                {
                    BindItemCriteria().Add(Restrictions.Eq("ContentType.Id", contentTypeIds[0]));
                }
                else
                {
                    BindItemCriteria().Add(Restrictions.InG("ContentType.Id", contentTypeIds));
                }
            }
        }

        private void ForContentItems(IEnumerable<int> ids)
        {
            if (ids == null) throw new ArgumentNullException("ids");

            // Converting to array as otherwise an exception "Expression argument must be of type ICollection." is thrown.
            Where<ContentItemRecord>(record => ids.ToArray().Contains(record.Id), BindCriteriaByPath(BindItemCriteria(), typeof(ContentItemRecord).Name));
        }

        private void Where<TRecord>() where TRecord : ContentPartRecord
        {
            // this simply demands an inner join
            BindPartCriteria<TRecord>();
        }

        private void Where<TRecord>(Expression<Func<TRecord, bool>> predicate) where TRecord : ContentPartRecord
        {
            Where<TRecord>(predicate, BindPartCriteria<TRecord>());
        }

        private void Where<TRecord>(Expression<Func<TRecord, bool>> predicate, ICriteria bindCriteria)
        {
            // build a linq to nhibernate expression
            var options = new QueryOptions();
            var queryProvider = new NHibernateQueryProvider(BindSession(), options);
            var queryable = new Query<TRecord>(queryProvider, options).Where(predicate);

            // translate it into the nhibernate ICriteria implementation
            var criteria = (CriteriaImpl)queryProvider.TranslateExpression(queryable.Expression);

            // attach the criterion from the predicate to this query's criteria for the record
            var recordCriteria = bindCriteria;
            foreach (var expressionEntry in criteria.IterateExpressionEntries())
            {
                recordCriteria.Add(expressionEntry.Criterion);
            }
        }



        private void OrderBy<TRecord, TKey>(Expression<Func<TRecord, TKey>> keySelector) where TRecord : ContentPartRecord
        {
            // build a linq to nhibernate expression
            var options = new QueryOptions();
            var queryProvider = new NHibernateQueryProvider(BindSession(), options);
            var queryable = new Query<TRecord>(queryProvider, options).OrderBy(keySelector);

            // translate it into the nhibernate ordering
            var criteria = (CriteriaImpl)queryProvider.TranslateExpression(queryable.Expression);

            // attaching orderings to the query's criteria
            var recordCriteria = BindPartCriteria<TRecord>();
            foreach (var ordering in criteria.IterateOrderings())
            {
                recordCriteria.AddOrder(ordering.Order);
            }
        }

        private void OrderByDescending<TRecord, TKey>(Expression<Func<TRecord, TKey>> keySelector) where TRecord : ContentPartRecord
        {
            // build a linq to nhibernate expression
            var options = new QueryOptions();
            var queryProvider = new NHibernateQueryProvider(BindSession(), options);
            var queryable = new Query<TRecord>(queryProvider, options).OrderByDescending(keySelector);

            // translate it into the nhibernate ICriteria implementation
            var criteria = (CriteriaImpl)queryProvider.TranslateExpression(queryable.Expression);

            // attaching orderings to the query's criteria
            var recordCriteria = BindPartCriteria<TRecord>();
            foreach (var ordering in criteria.IterateOrderings())
            {
                recordCriteria.AddOrder(ordering.Order);
            }
        }

        private IEnumerable<ContentItem> Slice(int skip, int count)
        {
            var criteria = BindItemCriteria();


            criteria.SetFetchMode("ContentItemRecord", FetchMode.Eager);
            criteria.SetFetchMode("ContentItemRecord.ContentType", FetchMode.Eager);

            // TODO: put 'removed false' filter in place
            if (skip != 0)
            {
                criteria = criteria.SetFirstResult(skip);
            }
            if (count != 0)
            {
                criteria = criteria.SetMaxResults(count);
            }

            return criteria
                .List<ContentItemRecord>()
                .Select(x => ContentManager.Get(x.Id))
                .ToReadOnlyCollection();
        }


        int Count()
        {
            var criteria = (ICriteria)BindItemCriteria().Clone();
            criteria.ClearOrders();


            return criteria.SetProjection(Projections.RowCount()).UniqueResult<Int32>();
        }

        void WithQueryHints(QueryHints hints)
        {
            if (hints == QueryHints.Empty)
            {
                return;
            }

            //var contentItemVersionCriteria = BindItemVersionCriteria();
            var contentItemCriteria = BindItemCriteria();

            var contentItemMetadata = _session.SessionFactory.GetClassMetadata(typeof(ContentItemRecord));

            // break apart and group hints by their first segment
            var hintDictionary = hints.Records
                .Select(hint => new { Hint = hint, Segments = hint.Split('.') })
                .GroupBy(item => item.Segments.FirstOrDefault())
                .ToDictionary(grouping => grouping.Key, StringComparer.InvariantCultureIgnoreCase);

            // locate hints that match properties in the ContentItemVersionRecord
            //foreach (var hit in contentItemVersionMetadata.PropertyNames.Where(hintDictionary.ContainsKey).SelectMany(key => hintDictionary[key]))
            //{
            //    contentItemVersionCriteria.SetFetchMode(hit.Hint, FetchMode.Eager);
            //    hit.Segments.Take(hit.Segments.Count() - 1).Aggregate(contentItemVersionCriteria, ExtendCriteria);
            //}

            // locate hints that match properties in the ContentItemRecord
            //foreach (var hit in contentItemMetadata.PropertyNames.Where(hintDictionary.ContainsKey).SelectMany(key => hintDictionary[key]))
            //{
            //    contentItemVersionCriteria.SetFetchMode("ContentItemRecord." + hit.Hint, FetchMode.Eager);
            //    hit.Segments.Take(hit.Segments.Count() - 1).Aggregate(contentItemCriteria, ExtendCriteria);
            //}

            //if (hintDictionary.SelectMany(x => x.Value).Any(x => x.Segments.Count() > 1))
            //    contentItemVersionCriteria.SetResultTransformer(new DistinctRootEntityResultTransformer());
        }

        void WithQueryHintsFor(string contentType)
        {
            var contentItem = ContentManager.New(contentType);
            var contentPartRecords = new List<string>();
            foreach (var part in contentItem.Parts)
            {
                var partType = part.GetType().BaseType;
                if (partType.IsGenericType && partType.GetGenericTypeDefinition() == typeof(ContentPart<>))
                {
                    var recordType = partType.GetGenericArguments().Single();
                    contentPartRecords.Add(recordType.Name);
                }
            }

            WithQueryHints(new QueryHints().ExpandRecords(contentPartRecords));
        }

        private static ICriteria ExtendCriteria(ICriteria criteria, string segment)
        {
            return criteria.GetCriteriaByPath(segment) ?? criteria.CreateCriteria(segment, JoinType.LeftOuterJoin);
        }

        IContentQuery<TPart> IContentQuery.ForPart<TPart>()
        {
            return new ContentQuery<TPart>(this);
        }

        public IContentQuery<TPart> ForPart<TPart>() where TPart : IContent
        {
            throw new NotImplementedException();
        }

        class ContentQuery<T> : IContentQuery<T> where T : IContent
        {
            protected readonly DefaultContentQuery _query;

            public ContentQuery(DefaultContentQuery query)
            {
                _query = query;
            }

            public IContentManager ContentManager
            {
                get { return _query.ContentManager; }
            }

            IContentQuery<TPart> IContentQuery.ForPart<TPart>()
            {
                return new ContentQuery<TPart>(_query);
            }

            IContentQuery<T> IContentQuery<T>.ForType(params string[] contentTypes)
            {
                _query.ForType(contentTypes);
                return this;
            }


            IContentQuery<T> IContentQuery<T>.ForContentItems(IEnumerable<int> ids)
            {
                _query.ForContentItems(ids);
                return this;
            }

            IEnumerable<T> IContentQuery<T>.List()
            {
                return _query.Slice(0, 0).AsPart<T>();
            }

            IEnumerable<T> IContentQuery<T>.Slice(int skip, int count)
            {
                return _query.Slice(skip, count).AsPart<T>();
            }

            int IContentQuery<T>.Count()
            {
                return _query.Count();
            }

            IContentQuery<T, TRecord> IContentQuery<T>.Join<TRecord>()
            {
                _query.Where<TRecord>();
                return new ContentQuery<T, TRecord>(_query);
            }

            IContentQuery<T, TRecord> IContentQuery<T>.Where<TRecord>(Expression<Func<TRecord, bool>> predicate)
            {
                _query.Where(predicate);
                return new ContentQuery<T, TRecord>(_query);
            }

            IContentQuery<T, TRecord> IContentQuery<T>.OrderBy<TRecord>(Expression<Func<TRecord, object>> keySelector)
            {
                _query.OrderBy(keySelector);
                return new ContentQuery<T, TRecord>(_query);
            }

            IContentQuery<T, TRecord> IContentQuery<T>.OrderByDescending<TRecord>(Expression<Func<TRecord, object>> keySelector)
            {
                _query.OrderByDescending(keySelector);
                return new ContentQuery<T, TRecord>(_query);
            }

            IContentQuery<T> IContentQuery<T>.WithQueryHints(QueryHints hints)
            {
                _query.WithQueryHints(hints);
                return this;
            }

            IContentQuery<T> IContentQuery<T>.WithQueryHintsFor(string contentType)
            {
                _query.WithQueryHintsFor(contentType);
                return this;
            }
        }


        class ContentQuery<T, TR> : ContentQuery<T>, IContentQuery<T, TR>
            where T : IContent
            where TR : ContentPartRecord
        {
            public ContentQuery(DefaultContentQuery query)
                : base(query)
            {
            }

            IContentQuery<T, TR> IContentQuery<T, TR>.Where(Expression<Func<TR, bool>> predicate)
            {
                _query.Where(predicate);
                return this;
            }

            IContentQuery<T, TR> IContentQuery<T, TR>.OrderBy<TKey>(Expression<Func<TR, TKey>> keySelector)
            {
                _query.OrderBy(keySelector);
                return this;
            }

            IContentQuery<T, TR> IContentQuery<T, TR>.OrderByDescending<TKey>(Expression<Func<TR, TKey>> keySelector)
            {
                _query.OrderByDescending(keySelector);
                return this;
            }

            IContentQuery<T, TR> IContentQuery<T, TR>.WithQueryHints(QueryHints hints)
            {
                _query.WithQueryHints(hints);
                return this;
            }

            IContentQuery<T, TR> IContentQuery<T, TR>.WithQueryHintsFor(string contentType)
            {
                _query.WithQueryHintsFor(contentType);
                return this;
            }
        }
    }


}
