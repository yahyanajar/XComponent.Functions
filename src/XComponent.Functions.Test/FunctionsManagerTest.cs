using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using XComponent.Functions.Core;
using Newtonsoft.Json.Linq;

namespace XComponent.Functions.Test
{

    [TestFixture]
    public class FunctionsManagerTest
    {

        [SetUp]
        public void SetUp() {
           Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
        }

        [Test]
        public void AddTaskShouldPutTaskOnQueue() {
            var functionsManager = new FunctionsManager("component", "statemachine");

            var xcEvent = new object();
            var publicMember = new object();
            var internalMember = new object();
            var context = new object();
            var sender = new object();

            functionsManager.AddTask(xcEvent, publicMember, internalMember, context, sender, "function");

            var enqueuedParameter = functionsManager.GetTask();

            Assert.AreEqual(xcEvent, enqueuedParameter.Event);
            Assert.AreEqual(publicMember, enqueuedParameter.PublicMember);
            Assert.AreEqual(internalMember, enqueuedParameter.InternalMember);
            Assert.AreEqual(context, enqueuedParameter.Context);
            Assert.AreEqual("component", enqueuedParameter.ComponentName);
            Assert.AreEqual("statemachine", enqueuedParameter.StateMachineName);
            Assert.AreEqual("function", enqueuedParameter.FunctionName);
            Assert.IsNotNull(enqueuedParameter.RequestId);
        }

        [Test]
        public async Task AddTaskAsyncShouldReturnPostedResult() {
            var functionsManager = new FunctionsManager("component", "statemachine");

            var xcEvent = new object();
            var publicMember = new object();
            var internalMember = new object();
            var context = new object();
            var sender = new object();

            var task = functionsManager.AddTaskAsync(xcEvent, publicMember, internalMember, context, sender, "function");

            var enqueuedParameter = functionsManager.GetTask();

            var functionResult = new FunctionResult() {
                ComponentName = "component",
                StateMachineName = "statemachine",
                PublicMember = "{}",
                InternalMember = "{}",
                Sender = null,
                RequestId = enqueuedParameter.RequestId,
            };

            functionsManager.AddTaskResult(functionResult);

            var publishedResult = await task;

            Assert.AreEqual(functionResult, publishedResult);
        }

        private class PublicMember {
            public string State { get; set; }
        }
        
        [Test]
        public async Task AddTaskShouldApplyPostedResult() {
            var functionsManager = new FunctionsManager("component", "statemachine");

            var xcEvent = new object();
            var publicMember = new PublicMember() { State = "before" };
            var internalMember = new object();
            var context = new object();
            var sender = new object();

            var task = functionsManager.AddTask(xcEvent, publicMember, internalMember, context, sender, "function");

            Task.Run(() => {
                while(true)
                {
                    var postedTask = functionsManager.GetTask();
                    if (postedTask != null)
                    {
                        var functionResult = new FunctionResult() {
                            ComponentName = "component",
                            StateMachineName = "statemachine",
                            PublicMember = "{ \"State\": \"after\" }",
                            InternalMember = "{}",
                            Sender = null,
                            RequestId = postedTask.RequestId,
                        };
                        functionsManager.AddTaskResult(functionResult);

                        break;
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            });

            await task;

            Assert.AreEqual("after", publicMember.State);
        }

        [Test]
        public void ApplyPostedResultShouldWorkWithJObjects() {
            var publicMemberBefore = new PublicMember() { State = "before" };

            var publicMemberAfter = new JObject();
            publicMemberAfter.Add("State", new JValue("after"));

            var functionResult = new FunctionResult() {
                PublicMember = publicMemberAfter,
            };

            var functionsManager = new FunctionsManager("component", "statemachine");
            functionsManager.ApplyFunctionResult(functionResult,
                    publicMemberBefore,
                    null,
                    new object(),
                    new object());

            Assert.AreEqual("after", publicMemberBefore.State);
        }
    }
}
