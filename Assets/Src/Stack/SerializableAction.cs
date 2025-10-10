using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using FUI;

using UnityEngine;
#nullable enable


/*public static class SerializableActionExtensions {
    public static SerializableActionFilter<X, N> Filter<X, N>(this SerializableActionBase<N> action, Func<X, N> filter) {
        var result = new SerializableActionFilter<X, N>(action, filter);
        return result;
    }
}*/

[Serializable]
public abstract class SerializableActionBase {

    protected string _methodName;
    protected string _targetTypeName;
    protected string _argumentTypeName;
    protected UnityEngine.Object? _target;

    protected SerializableActionBase(string methodName, string targetTypeName, string argumentTypeName, object target) {
        _methodName = methodName;
        _targetTypeName = targetTypeName;
        _argumentTypeName = argumentTypeName;
        if (target is UnityEngine.Object uo) 
            _target = uo;
        else
            _target = null;
    }

    public abstract void Invoke(object arg);

    bool TypeHasNoFields(Type type, out string? firstFieldName) {
        firstFieldName = null;
        var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (fields.Length > 0) {
            firstFieldName = fields[0].Name;
            return false;
        }

        var baseType = type.BaseType;
        if (baseType != null && baseType != typeof(object)) {
            return TypeHasNoFields(baseType, out firstFieldName);
        }
        return true;
    }
    
    public bool GetMethod([NotNullWhen(true)] out MethodInfo? method, [NotNullWhen(true)] out object? target) {
        target = _target;
        method = null;
        if (target == null) {
            var targetType = Type.GetType(_targetTypeName!);
            if (targetType == null) {
                return false;
            }
            bool typeHasNoFields = TypeHasNoFields(targetType, out var firstFieldName);
            if (!typeHasNoFields) {
                return false;
            }
            target = Activator.CreateInstance(targetType);
        }

        var xType = Type.GetType(_argumentTypeName!);
        if (xType == null) {
            return false;
        }
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        method = target.GetType().GetMethod(_methodName, flags, null, new Type[] { xType }, null);
        if (method != null) {
            return true;
        }
        return false;
    }
}


/*[Serializable]
public class SerializableActionFilter<X,N> : SerializableActionBase<X> { 
    public SerializableActionBase<N> _nextAction = null!;
    public Func<X, N> _filter = null!;

     public SerializableActionFilter(SerializableActionBase<N> nextAction, Func<X, N> filter)
     : base(filter.Method.Name, filter.Target.GetType().AssemblyQualifiedName, filter.Target) {
        _nextAction = nextAction;
        _filter = filter;
    }

    private N InvokeFilter(X arg) {
        if (_filter != null) {
            return _filter(arg);
        }
        if (GetMethod(out var method, out var target)) {
            return (N)method.Invoke(target, new object[] { arg })!;
        } else {
            Debug.LogError($"Call to method {_methodName} on target {_targetTypeName} failed");
            return default(N)!;
        }

    }

    public override void Invoke(X arg) {
        N t = InvokeFilter(arg);
        _nextAction.Invoke(t);
    }
}*/


[Serializable]
public class SerializableAction : SerializableActionBase {
    private Delegate? _action;


    public static implicit operator SerializableAction(Action action) {
        return new SerializableAction(action);
    }

    public SerializableAction(Delegate action): base(
        action.Method.Name,
        action.Target!.GetType().AssemblyQualifiedName,
        action.Method.GetParameters()[0].ParameterType.AssemblyQualifiedName, action.Target!) {
        _action = action;        
    }

    public override void Invoke(object arg) {
        if (_action != null) {
            _action.DynamicInvoke(arg);
            return;
        }

        if (GetMethod(out var method, out var target)) {
            method.Invoke(target, new object[] { arg });
            return;
        } else { 
            Debug.LogError($"Call to method {_methodName} on target {_targetTypeName} failed");
            return;
        }
    }
}
