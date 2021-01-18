using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TurboCollection;

namespace TurboCollectionTest
{
    /// <summary>
    /// Идентификатор ключа в виде пользовательского типа, с сравнением по значению.
    /// </summary>
    /// <remarks>
    /// Так как C# 9, то воспользуюсь готовой возможностью,
    /// что бы не переопределять Equals и GetHashCode, как в <see cref="CompositeKey{TKeyId,TKeyName}"/>
    /// </remarks>
    public record UserType(int Id, string Surname);

    public class Tests
    {
        [Test]
        public void MultiKeyCollectionTest()
        {
            var collection = new MultiKeyCollection<UserType, string, object>()
            {
                {(new UserType(1, "Некрасов"), "Павел"), "Некрасов Павел"},
                {(new UserType(2, "Денисов"), "Макар"), "Денисов Макар"},
                {(new UserType(3, "Авдеева"), "Стефания"), "Авдеева Стефания"},
                {(new UserType(4, "Смирнова"), "Софья"), "Смирнова Софья"},
                {(new UserType(5, "Киселева"), "Арина"), "Киселева Арина"},
                {(new UserType(5, "Киселева"), "Стефания"), "Киселева Стефания"},
                {(new UserType(2, "Денисов"), "Денис"), "Денисов Денис"},
                {(new UserType(4, "Смирнова"), "Арина"), "Смирнова Арина"},
                {(new UserType(2, "Денисов"), "Тимофей"), "Денисов Тимофей"},
                {(new UserType(2, "Денисов"), "Никита"), "Денисов Никита"},
            };

            Assert.AreEqual(1, collection.GetValueByName("Павел").Count());
            Assert.AreEqual(2, collection.GetValueByName("Стефания").Count());
            Assert.AreEqual(1, collection.GetValueById(new(1, "Некрасов")).Count());
            Assert.AreEqual(4, collection.GetValueById(new(2, "Денисов")).Count());
            Assert.Throws<KeyNotFoundException>(() => collection.GetValueById(new(0, "Киселева")));
        }

        [Test]
        public void MultiKeyCollectionEmptyTest()
        {
            var collection = new MultiKeyCollection<int, Guid, string>();
            Assert.Throws<KeyNotFoundException>(() => collection.GetValueById(0));
            Assert.Throws<KeyNotFoundException>(() => collection.GetValueByName(Guid.NewGuid()));
        }


        [Test]
        public void MultiKeyCollectionDictionaryTest()
        {
            var collection = new MultiKeyCollection<int, Guid, string>()
            {
                {(0, Guid.Empty), Guid.NewGuid().ToString()},
                {(0, Guid.NewGuid()), Guid.NewGuid().ToString()},
                {(1, Guid.NewGuid()), Guid.NewGuid().ToString()},
                {(2, Guid.NewGuid()), Guid.NewGuid().ToString()},
                {(3, Guid.Empty), Guid.NewGuid().ToString()},
                {(3, Guid.NewGuid()), Guid.NewGuid().ToString()},
                {(42, Guid.NewGuid()), "42"}
            };

            Assert.AreEqual(7, collection.Count);

            Assert.Throws<ArgumentException>(
                () => collection.Add((0, Guid.Empty), Guid.NewGuid().ToString()));
        }

        [Test]
        public void MultiKeyCollectionAddTest()
        {
            var collection = new MultiKeyCollection<int, string, int>()
            {
                {(0, "0"), 1},
                {(0, "1"), 2},
                {(1, "1"), 3},
                {(2, "2"), 4},
                {(3, "1"), 5},
                {(3, "4"), 6},
                {(42, "0"), 42}
            };

            CollectionAssert.AreEquivalent(new[] {1, 2}, collection.GetValueById(0));
            CollectionAssert.AreEquivalent(new[] {3}, collection.GetValueById(1));
            CollectionAssert.AreEquivalent(new[] {5, 6}, collection.GetValueById(3));
            Assert.Throws<KeyNotFoundException>(() => collection.GetValueById(-1));

            CollectionAssert.AreEquivalent(new[] {1, 42}, collection.GetValueByName("0"));
            CollectionAssert.AreEquivalent(new[] {2, 3, 5}, collection.GetValueByName("1"));
            CollectionAssert.AreEquivalent(new[] {4}, collection.GetValueByName("2"));
            Assert.Throws<ArgumentNullException>(() => collection.GetValueByName(null!));
            Assert.Throws<KeyNotFoundException>(() => collection.GetValueByName("key"));
        }

        [Test]
        public void MultiKeyCollectionIndexerTest()
        {
            var collection = new MultiKeyCollection<int, int, int>
            {
                [(0, 0)] = 1,
                [(0, 1)] = 2,
                [(1, 1)] = 3,
                [(1, 2)] = 4,
                [(2, 2)] = 5,
            };

            Assert.AreEqual(5, collection.Count);

            Assert.AreEqual(2, collection.GetValueById(0).Count());
            Assert.AreEqual(2, collection.GetValueById(1).Count());
            Assert.AreEqual(1, collection.GetValueById(2).Count());

            Assert.AreEqual(1, collection.GetValueByName(0).Count());
            Assert.AreEqual(2, collection.GetValueByName(1).Count());
            Assert.AreEqual(2, collection.GetValueByName(2).Count());
        }

        [Test]
        public void MultiKeyCollectionIndexerAddTest()
        {
            var collection = new MultiKeyCollection<int, int, int>
            {
                [(0, 0)] = 1,
                [(0, 1)] = 2,
                [(1, 1)] = 3,
                [(1, 2)] = 4,
                [(2, 2)] = 5,
                [(41, 43)] = 42,
            };

            Assert.AreEqual(42, collection[(41, 43)]);
            Assert.AreEqual(6, collection.Count);

            Assert.AreEqual(42, collection.GetValueById(41).Single());
            Assert.AreEqual(42, collection.GetValueByName(43).Single());
        }
        
        [Test]
        public void MultiKeyCollectionIndexerReplaceTest()
        {
            var collection = new MultiKeyCollection<int, int, int>
            {
                [(0, 0)] = 1,
                [(0, 1)] = 2,
                [(0, 0)] = 2,
            };
            
            Assert.AreEqual(2, collection[(0, 0)]);
            Assert.AreEqual(2, collection.Count);

            Assert.AreEqual(2, collection.GetValueById(0).Count());

            Assert.AreEqual(1, collection.GetValueByName(0).Count());
            Assert.AreEqual(1, collection.GetValueByName(1).Count());
        }
        
        [Test]
        public void MultiKeyCollectionRemoveTest()
        {
            var collection = new MultiKeyCollection<int, string, int>()
            {
                {(0, "0"), 1},
                {(0, "1"), 54},
                {(42, "0"), 42}
            };
            
            Assert.AreEqual(3, collection.Count);

            Assert.True(collection.Remove((0, "0")));
            
            Assert.AreEqual(54, collection.GetValueById(0).Single());
            Assert.AreEqual(42, collection.GetValueByName("0").Single());
        }
    }
}