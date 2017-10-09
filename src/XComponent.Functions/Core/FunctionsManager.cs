using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using XComponent.Functions.Core.Clone;
using XComponent.Functions.Core.Owin;
using XComponent.Functions.Core.Senders;
using XComponent.Functions.Utilities;

namespace XComponent.Functions.Core
{
    public class FunctionsManager: IDisposable
    {
        private OwinServer _owinServerRef = null;
        private readonly ConcurrentQueue<FunctionParameter> _taskQueue = new ConcurrentQueue<FunctionParameter>();

        internal event Action<FunctionResult> NewTaskFunctionResult;

        private readonly Dictionary<object, SenderWrapper> _senderWrapperBySender = new Dictionary<object, SenderWrapper>();

        internal FunctionsManager(string componentName, string stateMachineName)
        {
            ComponentName = componentName;
            StateMachineName = stateMachineName;
        }

        public string ComponentName { get;  }
        public string StateMachineName { get;  }

        internal void InitManager(Uri url)
        {
            _owinServerRef = OwinServerFactory.CreateOwinServer(url);
        }

        public void AddTask(object xcEvent, object publicMember, object internalMember,
            object context, object sender, [CallerMemberName] string functionName = null)
        {
            RegisterSender(sender);

            var functionParameter =  FunctionParameterFactory.CreateFunctionParameter(xcEvent,
                publicMember,
                internalMember,
                context, ComponentName,
                StateMachineName, functionName);

            string requestId = functionParameter.RequestId;
            _taskQueue.Enqueue(functionParameter);

            Action<FunctionResult> resultHandler = null;
            resultHandler = delegate(FunctionResult result)
            {
                if (result.RequestId == requestId)
                {
                    NewTaskFunctionResult -= resultHandler;

                    try
                    {
                       
                        if (publicMember != null && result.PublicMember != null)
                        {
                           var newPublicMember = SerializationHelper.DeserializeObjectFromType(publicMember.GetType(), result.PublicMember);

                            XCClone.Clone(newPublicMember, publicMember);
                        }
                        if (internalMember != null && result.InternalMember != null)
                        {
                            var newInternalMember = SerializationHelper.DeserializeObjectFromType(internalMember.GetType(), result.InternalMember);
                            XCClone.Clone(newInternalMember, internalMember);
                        }
                    }
                    catch (Exception e)
                    {

                    }

                    lock (_senderWrapperBySender)
                    {
                        _senderWrapperBySender[sender].TriggerSender(result, context);
                    }
                }
            };

            NewTaskFunctionResult += resultHandler;

        }

        internal void AddTaskResult(FunctionResult functionResult)
        {
            NewTaskFunctionResult?.Invoke(functionResult);
        }

        internal FunctionParameter GetTask()
        {
            FunctionParameter functionParameter = null;
            if (_taskQueue.TryDequeue(out functionParameter))
            {
                return functionParameter;
            }

            return null;
        }

        private void RegisterSender(object sender)
        {
            lock (_senderWrapperBySender)
            {
                if (!_senderWrapperBySender.ContainsKey(sender))
                {
                    _senderWrapperBySender.Add(sender, new SenderWrapper(sender));
                }
            }
        }


        public void Dispose()
        {
            OwinServerFactory.UnRegisterOwinServer(_owinServerRef);
        }
    }
}
