using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
//using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Dataverse
{
    public class DynamicEntity : DynamicObject
    {
        Dictionary<string, object> dictionary
        = new Dictionary<string, object>();

        public void AddProperty(string key, object value)
        {
            dictionary[key] = value;
        }


        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return dictionary.TryGetValue(binder.Name, out result);
        }
        public T GetValue<T>(string memberName)
        {
            var binder = Binder.GetMember(CSharpBinderFlags.None, memberName, this.GetType(), new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
            var callsite = CallSite<Func<CallSite, object, object>>.Create(binder);
            return (T)callsite.Target(callsite, this);

        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            // Converting the property name to lowercase
            // so that property names become case-insensitive.
            dictionary[binder.Name.ToLower()] = value;

            // You can always add a value to a dictionary,
            // so this method always returns true.
            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            return base.TryInvokeMember(binder, args, out result);
        }

        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            return base.TryInvoke(binder, args, out result);
        }

        public override bool TryCreateInstance(CreateInstanceBinder binder, object[] args, [NotNullWhen(true)] out object result)
        {
            return base.TryCreateInstance(binder, args, out result);
        }
    }
}
