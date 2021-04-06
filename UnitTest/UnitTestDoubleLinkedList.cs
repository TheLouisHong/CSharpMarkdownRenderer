using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Markdown2HTML.Core.Algorithms;
using Markdown2HTML.InlineRenderers;

namespace UnitTest
{
    [TestClass]
    public class UnitTestDoubleLinkedList
    {
        [TestMethod]
        public void Test0()
        {
            var linkedList = new DoubleLinkedList<string>();
            Assert.IsNull(linkedList.Tail);
            Assert.IsNull(linkedList.Head);
            Assert.AreEqual( 0, linkedList.Count());
        }

        [TestMethod]
        public void Test1()
        {
            var linkedList = new DoubleLinkedList<string>();
            linkedList.Append("number 1");
            Assert.AreEqual( "number 1", linkedList.Head.Value);
            Assert.AreEqual( "number 1", linkedList.Tail.Value);
            Assert.AreSame(linkedList.Head, linkedList.Tail);
            Assert.AreEqual( 1, linkedList.Count());
        }

        [TestMethod]
        public void Test2()
        {
            var linkedList = new DoubleLinkedList<string>();
            linkedList.Append("number 1");
            linkedList.Append("number 2");
            Assert.AreEqual( "number 1", linkedList.Head.Value);
            Assert.AreEqual( "number 2", linkedList.Tail.Value);
            Assert.AreSame(linkedList.Head.Next, linkedList.Tail);
            Assert.AreEqual( 2, linkedList.Count());
        }

        [TestMethod]
        public void Test3()
        {
            var linkedList = new DoubleLinkedList<string>();
            linkedList.Prepend("number 2");
            linkedList.Prepend("number 1");
            Assert.AreEqual( "number 1", linkedList.Head.Value);
            Assert.AreEqual( "number 2", linkedList.Tail.Value);
            Assert.AreSame(linkedList.Head.Next, linkedList.Tail);
            Assert.AreEqual( 2, linkedList.Count());
        }

        [TestMethod]
        public void Test4()
        {
            var linkedList = new DoubleLinkedList<string>();
            linkedList.Append("number 2");
            linkedList.Prepend("number 1");
            Assert.AreEqual( "number 1", linkedList.Head.Value);
            Assert.AreEqual( "number 2", linkedList.Tail.Value);
            Assert.AreEqual(linkedList.Head.Next, linkedList.Tail);
            Assert.AreEqual( 2, linkedList.Count());
        }

        [TestMethod]
        public void Test5()
        {
            var linkedList = new DoubleLinkedList<string>();
            linkedList.Append("number 2");
            linkedList.Append("number 3");
            linkedList.Prepend("number 1");
            Assert.AreEqual( "number 1", linkedList.Head.Value);
            Assert.AreEqual( "number 2", linkedList.Head.Next.Value);
            Assert.AreEqual( "number 3", linkedList.Tail.Value);
            Assert.AreSame(linkedList.Head.Next.Next, linkedList.Tail);
            Assert.AreEqual( 3, linkedList.Count());
        }


        [TestMethod]
        public void Test6()
        {
            var linkedList = new DoubleLinkedList<string>();
            linkedList.Append("number 2");
            linkedList.Append("number 3");
            linkedList.Prepend("number 1");
            Assert.AreEqual( "number 1", linkedList.Head.Value);
            Assert.AreEqual( "number 2", linkedList.Head.Next.Value);
            Assert.AreEqual( "number 3", linkedList.Tail.Value);
            Assert.AreSame( linkedList.Tail, linkedList.Head.Next.Next);
            Assert.AreEqual( 3, linkedList.Count());

            linkedList.RemoveLast();
            Assert.AreEqual( 2, linkedList.Count());
            Assert.AreEqual( "number 1", linkedList.Head.Value);
            Assert.AreEqual( "number 2", linkedList.Head.Next.Value);
            Assert.AreSame(linkedList.Head.Next, linkedList.Tail);
            Assert.IsNull(linkedList.Head.Prev);
            Assert.IsNull(linkedList.Tail.Next);

            linkedList.RemoveLast();
            Assert.AreEqual( 1, linkedList.Count());
            Assert.AreEqual( "number 1", linkedList.Head.Value);
            Assert.AreSame(  linkedList.Head, linkedList.Tail);
            Assert.IsNull(linkedList.Head.Prev);
            Assert.IsNull(linkedList.Tail.Next);

            linkedList.RemoveLast();
            Assert.AreEqual( 0, linkedList.Count());
            Assert.IsNull(linkedList.Head);
            Assert.IsNull(linkedList.Tail);
            Assert.AreSame(  linkedList.Head, linkedList.Tail);
        }
        
        [TestMethod]
        public void Test7()
        {
            var linkedList = new DoubleLinkedList<string>();
            linkedList.Append("number 2");
            linkedList.Append("number 3");
            linkedList.Prepend("number 1");
            Assert.AreEqual( "number 1", linkedList.Head.Value);
            Assert.AreEqual( "number 2", linkedList.Head.Next.Value);
            Assert.AreEqual( "number 3", linkedList.Tail.Value);
            Assert.AreSame( linkedList.Tail, linkedList.Head.Next.Next);
            Assert.AreEqual( 3, linkedList.Count());

            linkedList.RemoveFirst();
            Assert.AreEqual( 2, linkedList.Count());
            Assert.AreEqual( "number 2", linkedList.Head.Value);
            Assert.AreEqual( "number 3", linkedList.Head.Next.Value);
            Assert.AreSame(linkedList.Head.Next, linkedList.Tail);
            Assert.IsNull(linkedList.Head.Prev);
            Assert.IsNull(linkedList.Tail.Next);

            linkedList.RemoveFirst();
            Assert.AreEqual( 1, linkedList.Count());
            Assert.AreEqual( "number 3", linkedList.Head.Value);
            Assert.AreSame(  linkedList.Head, linkedList.Tail);
            Assert.IsNull(linkedList.Head.Prev);
            Assert.IsNull(linkedList.Tail.Next);

            linkedList.RemoveFirst();
            Assert.AreEqual( 0, linkedList.Count());
            Assert.IsNull(linkedList.Head);
            Assert.IsNull(linkedList.Tail);
            Assert.AreSame(  linkedList.Head, linkedList.Tail);
        }
        
        [TestMethod]
        public void Test8()
        {
            var linkedList = new DoubleLinkedList<string>();
            linkedList.Append("number 2");
            linkedList.Append("number 3");
            linkedList.Prepend("number 1");
            Assert.AreEqual( "number 1", linkedList.Head.Value);
            Assert.AreEqual( "number 2", linkedList.Head.Next.Value);
            Assert.AreEqual( "number 3", linkedList.Tail.Value);
            Assert.AreSame( linkedList.Tail, linkedList.Head.Next.Next);
            Assert.AreEqual( 3, linkedList.Count());

            linkedList.RemoveFirst();
            Assert.AreEqual( 2, linkedList.Count());
            Assert.AreEqual( "number 2", linkedList.Head.Value);
            Assert.AreEqual( "number 3", linkedList.Head.Next.Value);
            Assert.AreSame(linkedList.Head.Next, linkedList.Tail);
            Assert.IsNull(linkedList.Head.Prev);
            Assert.IsNull(linkedList.Tail.Next);

            linkedList.RemoveLast();
            Assert.AreEqual( 1, linkedList.Count());
            Assert.AreEqual( "number 2", linkedList.Head.Value);
            Assert.AreSame(  linkedList.Head, linkedList.Tail);
            Assert.IsNull(linkedList.Head.Prev);
            Assert.IsNull(linkedList.Tail.Next);

            linkedList.Prepend("surprise 1");
            Assert.AreEqual( 2, linkedList.Count());
            Assert.AreEqual( "surprise 1", linkedList.Head.Value);
            Assert.AreEqual( "number 2", linkedList.Head.Next.Value);
            Assert.AreSame(linkedList.Head.Next, linkedList.Tail);
            Assert.IsNull(linkedList.Head.Prev);
            Assert.IsNull(linkedList.Tail.Next);

            linkedList.RemoveLast();
            Assert.AreEqual( 1, linkedList.Count());
            Assert.AreEqual( "surprise 1", linkedList.Head.Value);
            Assert.AreSame(  linkedList.Head, linkedList.Tail);
            Assert.IsNull(linkedList.Head.Prev);
            Assert.IsNull(linkedList.Tail.Next);

            linkedList.RemoveFirst();
            Assert.AreEqual( 0, linkedList.Count());
            Assert.IsNull(linkedList.Head);
            Assert.IsNull(linkedList.Tail);
            Assert.AreSame(  linkedList.Head, linkedList.Tail);
        }

        [TestMethod]
        public void TestInsertAfter01()
        {
            var list = new DoubleLinkedList<string>();
            var number1 = list.Append("number 1");
            list.InsertAfter(number1, "number 2");
            list.InsertBefore(number1, "number 0");

            string[] correct = {"number 0", "number 1", "number 2"};

            int i = 0;
            foreach (var item in list)
            {
                Console.WriteLine(item);
                Assert.AreEqual(correct[i++], item);
            }

        }

        [TestMethod]
        public void TestInsertAfter02()
        {
            var list = new DoubleLinkedList<string>();
            var root = list.Append("root");

            root.InsertBefore("before 1")
                .InsertBefore("before 2")
                .InsertBefore("before 3")
                .InsertAfter("surprise");

            root.InsertAfter("after 1")
                .InsertAfter("after 2")
                .InsertAfter("after 3")
                .InsertBefore("surprise");


            string[] correct = {"before 3", "surprise", "before 2", "before 1", "root", "after 1", "after 2", "surprise", "after 3"};

            foreach (var item in list)
            {
                Console.WriteLine(item);
            }

            int i = 0;
            foreach (var item in list)
            {
                Assert.AreEqual(correct[i++], item);
            }

            Assert.AreEqual(correct.Length, list.Count());

        }

        [TestMethod]
        public void TestRemove01()
        {
            var list = new DoubleLinkedList<string>();
            var root = list.Append("root");

            root.InsertBefore("before 1")
                .InsertBefore("before 2")
                .InsertBefore("before 3")
                .InsertAfter("surprise");

            root.InsertAfter("after 1")
                .InsertAfter("after 2")
                .InsertAfter("after 3")
                .InsertBefore("surprise");

            {
                string[] correct =
                {
                    "before 3",
                    "surprise",
                    "before 2",
                    "before 1",
                    "root",
                    "after 1",
                    "after 2",
                    "surprise",
                    "after 3"
                };

                int i = 0;
                foreach (var item in list)
                {
                    Assert.AreEqual(correct[i++], item);
                }
                Assert.AreEqual(correct.Length, list.Count());
            }

            list.Remove(root);
            {
                string[] correct =
                {
                    "before 3",
                    "surprise",
                    "before 2",
                    "before 1",
                    "after 1",
                    "after 2",
                    "surprise",
                    "after 3"
                };
                int i = 0;
                foreach (var item in list)
                {
                    Assert.AreEqual(correct[i++], item);
                }
                Assert.AreEqual(correct.Length, list.Count());
            }

            list.RemoveFirst();
            {
                string[] correct =
                {
                    "surprise",
                    "before 2",
                    "before 1",
                    "after 1",
                    "after 2",
                    "surprise",
                    "after 3"
                };

                int i = 0;
                foreach (var item in list)
                {
                    Assert.AreEqual(correct[i++], item);
                }
                Assert.AreEqual(correct.Length, list.Count());
            }

            list.RemoveLast();
            {
                string[] correct =
                {
                    "surprise",
                    "before 2",
                    "before 1",
                    "after 1",
                    "after 2",
                    "surprise",
                };

                int i = 0;
                foreach (var item in list)
                {
                    Assert.AreEqual(correct[i++], item);
                }
                Assert.AreEqual(correct.Length, list.Count());
            }

            while (list.Count() != 0)
            {
                if (list.Count() >= 2)
                {
                    list.Remove(list.Head.Next);
                }
                else
                {
                    list.Remove(list.Head);
                }
            }
        }
    }
}
