using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class ButtonAttribute : PropertyAttribute {
    public string MethodName { get; }
    public ButtonAttribute(string methodName = "")
    {
        MethodName = methodName;
    }
}