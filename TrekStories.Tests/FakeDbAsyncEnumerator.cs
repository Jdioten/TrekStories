using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TrekStories.Tests
{
    public class FakeDbAsyncEnumerator<T> : IDbAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _localEnumerator;

        public FakeDbAsyncEnumerator(IEnumerator<T> localEnumerator)
        {
            _localEnumerator = localEnumerator;
        }

        public void Dispose()
        {
            _localEnumerator.Dispose();
        }

        public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_localEnumerator.MoveNext());
        }

        public T Current
        {
            get { return _localEnumerator.Current; }
        }

        object IDbAsyncEnumerator.Current
        {
            get { return Current; }
        }
    }
}

//copied from http://mvenkatraman.co.uk/blog/2015/08/13/unit-testing-entity-framework-async-queries/
