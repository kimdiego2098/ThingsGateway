//------------------------------------------------------------------------------
//  此代码版权（除特别声明或在XREF结尾的命名空间的代码）归作者本人若汝棋茗所有
//  源代码使用协议遵循本仓库的开源协议及附加协议，若本仓库没有设置，则按MIT开源协议授权
//  CSDN博客：https://blog.csdn.net/qq_40374647
//  哔哩哔哩视频：https://space.bilibili.com/94253567
//  Gitee源代码仓库：https://gitee.com/RRQM_Home
//  Github源代码仓库：https://github.com/RRQM
//  API首页：https://www.yuque.com/rrqm/touchsocket/index
//  交流QQ群：234762506
//  感谢您的下载和使用
//------------------------------------------------------------------------------
//------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace ThingsGateway.Foundation.Http
{
    /// <summary>
    /// 多文件集合
    /// </summary>
    public class MultifileCollection : IEnumerable<IFormFile>
    {
        private readonly HttpRequest m_httpRequest;

        /// <summary>
        /// 多文件集合
        /// </summary>
        /// <param name="httpRequest"></param>
        public MultifileCollection(HttpRequest httpRequest)
        {
            m_httpRequest = httpRequest;
        }

        /// <summary>
        /// 获取一个迭代器。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IFormFile> GetEnumerator()
        {
            if (m_httpRequest.ContentComplated == null || m_httpRequest.ContentComplated == true)
            {
                if (m_httpRequest.TryGetContent(out byte[] context))
                {
                    byte[] boundary = $"--{m_httpRequest.GetBoundary()}".ToUTF8Bytes();
                    var indexs = context.IndexOfInclude(0, context.Length, boundary);
                    if (indexs.Count <= 0)
                    {
                        throw new Exception("没有发现由Boundary包裹的数据。");
                    }
                    List<IFormFile> files = new List<IFormFile>();
                    for (int i = 0; i < indexs.Count; i++)
                    {
                        if (i + 1 < indexs.Count)
                        {
                            InternalFormFile internalFormFile = new InternalFormFile();
                            files.Add(internalFormFile);
                            int index = context.IndexOfFirst(indexs[i] + 3, indexs[i + 1], Encoding.UTF8.GetBytes("\r\n\r\n"));
                            string line = Encoding.UTF8.GetString(context, indexs[i] + 3, index - indexs[i] - 6);
                            string[] lines = line.Split(new string[] { ";", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                            internalFormFile.DataPair = new NameValueCollection();
                            foreach (var item in lines)
                            {
                                string[] kv = item.Split(new char[] { ':', '=' });
                                if (kv.Length == 2)
                                {
                                    internalFormFile.DataPair.Add(kv[0].Trim(), kv[1].Replace("\"", String.Empty).Trim());
                                }
                            }

                            int length = indexs[i + 1] - (index + 2) - boundary.Length;
                            byte[] data = new byte[length];
                            Array.Copy(context, index + 1, data, 0, length);
                            //string ssss = Encoding.UTF8.GetString(data);
                            internalFormFile.Data = data;
                        }
                    }

                    return files.GetEnumerator();
                }
                throw new Exception("管道状态异常");
            }
            else
            {
                throw new Exception("管道状态异常");
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}