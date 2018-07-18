using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;

namespace TrekStories.Tests
{
    class TestDbSet<T> : DbSet<T>, IQueryable<T>, IEnumerable<T>, IDbAsyncEnumerable<T>
        where T : class
    {
        ICollection<T> _data;
        IQueryable<T> _query;

        public TestDbSet()
        {
            _data = new List<T>();
            _query = _data.AsQueryable();
        }

        public override T Add(T item)
        {
            _data.Add(item);
            return item;
        }

        public override T Remove(T item)
        {
            _data.Remove(item);
            return item;
        }

        public override T Attach(T item)
        {
            _data.Add(item);
            return item;
        }

        public override T Create()
        {
            return Activator.CreateInstance<T>();
        }

        public override TDerivedEntity Create<TDerivedEntity>()
        {
            return Activator.CreateInstance<TDerivedEntity>();
        }

        //public override ObservableCollection<T> Local
        //{
        //    get { return new ObservableCollection<T>(_data); }
        //}

        public Type ElementType
        {
            get { return _query.ElementType; }
        }

        public Expression Expression
        {
            get { return _query.Expression; }
        }

        public IQueryProvider Provider
        {
            get { return _query.Provider; }
        }

        //Possible FIX when internet!!!
        //public IQueryProvider Provider
        //{
        //    get { return new TestDbAsyncQueryProvider<T>(_query.Provider); }
        //}

//https://msdn.microsoft.com/en-us/data/dn314431 ef testing eith your own test doubles


        //System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        //{
        //    return _data.GetEnumerator();
        //}

        //IEnumerator<T> IEnumerable<T>.GetEnumerator()
        //{
        //    return _data.GetEnumerator();
        //}

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _query.GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _query.GetEnumerator();
        }

        //IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        //{
        //    return ((IDbAsyncEnumerable)_query).GetAsyncEnumerator();
        //}

        //public IDbAsyncEnumerator<T> GetAsyncEnumerator()
        //{
        //    return ((IDbAsyncEnumerable<T>)_query).GetAsyncEnumerator();
        //}

        public IDbAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return new FakeDbAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        {
            return GetAsyncEnumerator();
        }
    }
}

//code copied from https://docs.microsoft.com/en-us/aspnet/web-api/overview/testing-and-debugging/mocking-entity-framework-when-unit-testing-aspnet-web-api-2#create-test-context
// and from https://gist.github.com/axelheer/bdbbd2f92600a45f22d6 for Async