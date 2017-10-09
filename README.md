# XComponent.Functions
XComponent Functions is used to take advantage of XComponent in any technologies (Node, Java, C++, ...)


[![Slack](http://slack.xcomponent.com/badge.svg)](http://slack.xcomponent.com/)

[![Build status](https://ci.appveyor.com/api/projects/status/dsj723fkcacptuoq?svg=true)](https://ci.appveyor.com/project/fredericcarre/xcomponent-functions)
[![XComponent.Functions Nuget](https://img.shields.io/nuget/v/XComponent.Functions.svg)]
(https://www.nuget.org/packages/XComponent.Functions/)


This library automatically exposes XComponent Triggered Methods to a Rest Endpoint. This way, Triggered methods can be implemented in any languages outside of Visual Studio. 
Moreover, with this library you can take advantage of XComponent Studio to design complex orchestrations and code in the technology that fits to your needs.

# How it works ?

1. Reference the following Nuget package in the TriggeredMethod projects: 
https://www.nuget.org/packages/XComponent.Functions/

2. Initialize the library in the Triggered Method project.
First of all, you need to create a **FunctionsManager** by type of state machine. The **FunctionsManager** will expose through a Rest Api the tasks to execute for a type of state machine. 
Initialize the **FunctionsManager** with the following lines of code :

```csharp
 FunctionsManager myFunctionManager = FunctionsFactory.CreateFunctionsManager(ComponentHelper.COMPONENT_NAME, "MyStateMAchineName", FunctionsFactory.DefaultUrl);
 ```
 > Note: The **FunctionsManager** can be stored on the TriggeredMethodContext.

 3. Implement the Triggered Methods

You only need to add a call to the **AddTask** method of the **FunctionsManager** .

```csharp
  public static void ExecuteOn_Calculating_Through_Calculate(Calculate calculate, Calculator calculator, object object_InternalMember, Context context, ICalculateCalculateOnCalculatingCalculatorSenderInterface sender)
{
    myFunctionManager.AddTask(calculate, calculator, object_InternalMember, context, sender);
} 
```

4. REST Api

By default, the REST Endpoint is: http://127.0.0.1:9676/swagger/ui/index

This REST Api exposes two methods:
+ Get method should be called from to retrieve the tasks to execute.
```Json
{
  "Event": {},
  "PublicMember": {},
  "InternalMember": {},
  "Context": {},
  "ComponentName": "string",
  "StateMachineName": "string",
  "FunctionName": "string",
  "RequestId": "string"
}
```
+ Post method is used to push the retrieve the result of the Triggered Method.

```Json
{
  "ComponentName": "string",
  "StateMachineName": "string",
  "PublicMember": {},
  "InternalMember": {},
  "Sender": {
    "SenderName": "string",
    "SenderParameter": "string",
    "UseContext": true
  },
  "RequestId": "string"
}
```

> Public Member and Internal Member can be modified this way. We can also call the **Senders**.



