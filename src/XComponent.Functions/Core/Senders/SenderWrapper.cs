using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using XComponent.Functions.Utilities;


namespace XComponent.Functions.Core.Senders
{
    internal class SenderWrapper
    {
        private readonly object _sender;
        private List<SenderMethod> _sendersList;
        private Dictionary<string, Type> _senderTypeBySenderName;

        private const string SendEvent = "SendEvent";
        private const string TransitionEvent = "transitionEvent";


        public SenderWrapper(object sender)
        {
            _sender = sender;
            Init();
        }

        private void Init()
        {
            Type myType = (_sender.GetType());
            MethodInfo[] myArrayMethodInfo = myType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            _sendersList = myArrayMethodInfo.Where(e => e.Name == SendEvent || e.GetParameters().Length > 1).Select(e => new SenderMethod
            {
                Name = e.Name,
                SenderParameterCollection = e.GetParameters().Select(p => new SenderParameter()
                {
                    Name = p.Name,
                    Type = p.ParameterType
                }).ToArray(),
                MethodInfo = e
            }).ToList();

            _senderTypeBySenderName =
                myArrayMethodInfo.Where(v => v.GetParameters().FirstOrDefault(e => e.Name == TransitionEvent) != null)
                    .ToDictionary(e => e.Name,
                        v => v.GetParameters().Where(e => e.Name == TransitionEvent).Select(e => e.ParameterType).FirstOrDefault());


        }

        public void TriggerSender(FunctionResult functionResult, object context)
        {
            if (!string.IsNullOrEmpty(functionResult.Sender?.SenderName))
            {
                var obj = !string.IsNullOrEmpty(functionResult.Sender.SenderParameter?.ToString())
                    ? SerializationHelper.DeserializeObjectFromType(_senderTypeBySenderName[functionResult.Sender.SenderName], functionResult.Sender.SenderParameter)
                    : Activator.CreateInstance(_senderTypeBySenderName[functionResult.Sender.SenderName]);

                if (functionResult.Sender.UseContext)
                {
                    var method = _sendersList.FirstOrDefault(e => e.SenderParameterCollection.Count() == 3 
                                                                && e.Name == functionResult.Sender.SenderName);
                    method?.MethodInfo.Invoke(_sender, new[] { context, obj, null });
                }
                else
                {
                    var method = _sendersList.FirstOrDefault(e => e.Name == SendEvent &&
                                                           e.SenderParameterCollection.Count() == 1
                                                           && e.SenderParameterCollection.Any(p => p.Type.IsAssignableFrom(_senderTypeBySenderName[functionResult.Sender.SenderName])) && e.Name == "SendEvent");
                    method?.MethodInfo.Invoke(_sender, new[] { obj });
                }
            }
        }
    }
}
