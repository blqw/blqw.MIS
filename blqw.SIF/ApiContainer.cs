using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using blqw.SIF.Descriptor;
using blqw.SIF.Services;
using System.Reflection;
using blqw.SIF.DataModification;
using blqw.SIF.Validation;
using blqw.SIF.Filters;

namespace blqw.SIF
{
    /// <summary>
    /// 容器
    /// </summary>
    public sealed class ApiContainer
    {
        /// <summary>
        /// 初始化容器
        /// </summary>
        /// <param name="id">容器名称,唯一标识</param>
        public ApiContainer(string id, IApiContainerServices serviceProvider)
        {
            ID = id;
            Services = serviceProvider;
            var filters = new List<ApiFilterAttribute>();
            var validations = new List<DataValidationAttribute>();
            var modifications = new List<DataModificationAttribute>();
            if(serviceProvider.GlobalFilters!=null) filters.AddRange(serviceProvider.GlobalFilters);
            if(serviceProvider.GlobalValidations != null) validations.AddRange(serviceProvider.GlobalValidations);
            if(serviceProvider.GlobalModifications != null) modifications.AddRange(serviceProvider.GlobalModifications);
            
            var apiGlobalType = typeof(ApiGlobal).GetTypeInfo();
            var apiGlobals = serviceProvider.DefinedTypes
                                .Select(t => t.GetTypeInfo())
                                .Where(t => t.IsAbstract == false)
                                .Where(t => apiGlobalType.IsAssignableFrom(t))
                                .Where(t => t.IsGenericType == false || t.IsGenericTypeDefinition == false)
                                .Select(t => t.DeclaredConstructors.FirstOrDefault(c => c.GetParameters().Length == 0))
                                .Where(c => c != null)
                                .Select(c => (ApiGlobal)c.Invoke(null));

            foreach (var apiGlobal in apiGlobals)
            {
                apiGlobal.Initialization();
                if (apiGlobal.Filters != null) filters.AddRange(apiGlobal.Filters);
                if (apiGlobal.Validations != null) validations.AddRange(apiGlobal.Validations);
                if (apiGlobal.Modifications != null) modifications.AddRange(apiGlobal.Modifications);
            }
            Filters = filters.AsReadOnly();
            Validations = validations.AsReadOnly();
            Modifications = modifications.AsReadOnly();
            ApiCollection = new ApiCollection(this);
        }

        public string ID { get; }

        /// <summary>
        /// 接口集合
        /// </summary>
        public ApiCollection ApiCollection { get; }

        /// <summary>
        /// 服务集合
        /// </summary>
        public IApiContainerServices Services { get; }


        public IReadOnlyCollection<ApiFilterAttribute> Filters { get; }

        public IReadOnlyCollection<DataValidationAttribute> Validations { get; }

        public IReadOnlyCollection<DataModificationAttribute> Modifications { get; }


        /// <summary>
        /// 创建上下文
        /// </summary>
        /// <param name="apiDescriptor">api描述</param>
        /// <param name="dataProvider">数据提供程序</param>
        /// <param name="instance">api实例</param>
        /// <param name="args">api参数</param>
        /// <returns>如果编译失败,返回异常信息</returns>
        public ApiCallContext CreateContext(ApiDescriptor apiDescriptor, IApiDataProvider dataProvider, out IResultProvider resultProvider)
        {
            if (ApiCollection.Apis.Contains(apiDescriptor) == false)
            {
                throw new ArgumentException("Api描述无效", nameof(apiDescriptor));
            }
            var instance = apiDescriptor.Method.IsStatic ? null : Activator.CreateInstance(apiDescriptor.ApiClass.Type);
            var parameters = new NameDictionary();
            var properties = new NameDictionary();
            resultProvider = new ResultProvider();
            var context = new ApiCallContext(resultProvider, instance, parameters, properties);
            context.Data["$ResultProvider"] = resultProvider;
            context.Data["$ApiContainer"] = this;
            context.Data["$ApiDescriptor"] = apiDescriptor;
            if (apiDescriptor.Method.IsStatic == false)
            {
                //属性
                foreach (var p in apiDescriptor.Properties)
                {
                    var result = dataProvider.GetProperty(p);

                    if (result.Error != null && result.Exists)
                    {
                        properties.Add(p.Name, result.Error);
                        resultProvider.Exception = result.Error;
                        return context;
                    }

                    if (result.Exists || p.HasDefaultValue == false)
                    {
                        var value = result.Exists ? result.Value : null;
                        if (p.DataModifications.Count > 0) p.DataModifications.ForEach(it => it.Modifies(ref value, context)); //变更数据
                        Modifier.Modifies(value, context);
                        properties.Add(p.Name, value);
                        p.Setter(instance, value);
                        if (p.DataValidations.Count > 0)
                        {
                            var ex = p.DataValidations.FirstOrDefault(it => it.IsValid(value, context) == false)?.GetException(p.Name, value, context)
                                        ?? Validator.IsValid(value, context, true); //数据验证
                            if (ex != null)
                            {
                                resultProvider.Exception = ex;
                                return context;
                            }
                        }
                    }
                    else
                    {
                        properties.Add(p.Name, p.DefaultValue);
                        p.Setter(instance, p.DefaultValue);
                    }

                }
            }

            //参数
            foreach (var p in apiDescriptor.Parameters)
            {
                var result = dataProvider.GetParameter(p);

                if (result.Error != null && result.Exists)
                {
                    parameters.Add(p.Name, result.Error);
                    resultProvider.Exception = result.Error;
                    return context;
                }

                if (result.Exists)
                {
                    var value = result.Value;
                    if (p.DataModifications.Count > 0) p.DataModifications.ForEach(it => it.Modifies(ref value, context)); //变更数据
                    Modifier.Modifies(value, context);
                    parameters.Add(p.Name, value);
                    if (p.DataValidations.Count > 0)
                    {
                        var ex = p.DataValidations.FirstOrDefault(it => it.IsValid(value, context) == false)?.GetException(p.Name, value, context)
                                    ?? Validator.IsValid(value, context, true); //数据验证
                        if (ex != null)
                        {
                            resultProvider.Exception = ex;
                            return context;
                        }
                    }
                }
                else if (p.HasDefaultValue)
                {
                    parameters.Add(p.Name, p.DefaultValue);
                }
                else
                {
                    var ex = ApiException.ArgumentMissing(p.Name);
                    parameters.Add(p.Name, ex);
                    resultProvider.Exception = ex;
                    return context;
                }
            }

            return context;
        }

    }
}

