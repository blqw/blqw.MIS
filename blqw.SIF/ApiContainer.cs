using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using blqw.SIF.Descriptor;
using blqw.SIF.Services;
using System.Reflection;

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
            ApiGlobal = new ApiGlobalProxy();
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

        /// <summary>
        /// 全局操作
        /// </summary>
        public ApiGlobal ApiGlobal { get; private set; }


        /// <summary>
        /// 创建上下文
        /// </summary>
        /// <param name="apiDescriptor">api描述</param>
        /// <param name="dataProvider">数据提供程序</param>
        /// <param name="instance">api实例</param>
        /// <param name="args">api参数</param>
        /// <returns>如果编译失败,返回异常信息</returns>
        public ApiCallContext CreateContext(ApiDescriptor apiDescriptor, IApiDataProvider dataProvider, out ResultProvider resultProvider)
        {
            if (ApiCollection.Apis.Contains(apiDescriptor) == false)
            {
                throw new ArgumentException("Api描述无效", nameof(apiDescriptor));
            }
            var instance = (object)null;
            var parameters = new SafeStringDictionary();
            var properties = new SafeStringDictionary();

            if (apiDescriptor.Method.IsStatic == false)
            {
                instance = Activator.CreateInstance(apiDescriptor.ApiClass.Type);
                foreach (var p in apiDescriptor.Properties)
                {
                    var result = dataProvider.GetProperty(p);

                    if (result.Error != null && result.Exists)
                    {
                        properties.Add(p.Name, result.Error);
                        resultProvider = new ResultProvider(result.Error);
                        return new ApiCallContext(resultProvider, instance, null, properties).AppendData("$ResultProvider", resultProvider).AppendData("$ApiContainer", this);
                    }
                    else if (result.Exists)
                    {
                        p.Setter(instance, result.Value);
                        properties.Add(p.Name, result.Value);
                    }
                    else if (p.DefaultValue == null)
                    {
                        properties.Add(p.Name, null);
                    }
                    else
                    {
                        p.Setter(instance, p.DefaultValue);
                        properties.Add(p.Name, p.DefaultValue);
                    }
                }
            }
            else
            {
                instance = null;
            }

            parameters = new SafeStringDictionary();

            foreach (var p in apiDescriptor.Parameters)
            {
                var result = dataProvider.GetParameter(p);

                if (result.Error != null && result.Exists)
                {
                    parameters.Add(p.Name, result.Error);
                    resultProvider = new ResultProvider(result.Error);
                    return new ApiCallContext(resultProvider, instance, parameters, properties).AppendData("$ResultProvider", resultProvider).AppendData("$ApiContainer", this);
                }
                else if (result.Exists)
                {
                    parameters.Add(p.Name, result.Value);
                }
                else
                {
                    parameters.Add(p.Name, p.DefaultValue);
                }
            }

            resultProvider = new ResultProvider(null);
            return new ApiCallContext(resultProvider, instance, parameters, properties).AppendData("$ResultProvider", resultProvider).AppendData("$ApiContainer", this);
        }

    }
}

