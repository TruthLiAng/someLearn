using Autofac;
using Service.Core.Interface;
using System;
using System.Linq;
using System.Reflection;

namespace WechatDataService
{
    public class AutofacManage
    {
        public AutofacManage()
        {
            ContainerBuilder builder = new ContainerBuilder();
            //builder.RegisterGeneric(typeof(Repository<,>)).As(typeof(IRepository<,>));
            Type baseType = typeof(IDependency);

            // 获取所有相关类库的程序集
            //DLL所在的绝对路径
            Assembly assembly = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "L0-Core.dll");

            builder.RegisterAssemblyTypes(assembly)
                .Where(type => baseType.IsAssignableFrom(type) && !type.IsAbstract)
                .AsImplementedInterfaces().InstancePerLifetimeScope();//InstancePerLifetimeScope 保证对象生命周期基于请求

            IContainer container = builder.Build();
        }
    }
}