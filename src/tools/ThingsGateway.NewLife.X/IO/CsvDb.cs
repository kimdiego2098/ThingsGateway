﻿//------------------------------------------------------------------------------
//  此代码版权声明为全文件覆盖，如有原作者特别声明，会在下方手动补充
//  此代码版权（除特别声明外的代码）归作者本人Diego所有
//  源代码使用协议遵循本仓库的开源协议及附加协议
//  Gitee源代码仓库：https://gitee.com/diego2098/ThingsGateway
//  Github源代码仓库：https://github.com/kimdiego2098/ThingsGateway
//  使用文档：https://kimdiego2098.github.io/
//  QQ群：605534569
//------------------------------------------------------------------------------

using NewLife.Reflection;
using NewLife.Serialization;

using System.Reflection;
using System.Text;

namespace NewLife.IO;

/// <summary>Csv文件轻量级数据库</summary>
/// <remarks>
/// 文档 https://newlifex.com/core/csv_db
/// 适用于大量数据需要快速存储、快速查找，很少修改和删除的场景。
/// 在桌面客户端中，关系型数据库SQLite很容易因非法关机而损坏，本数据库能跳过损坏行，自动恢复。
/// </remarks>
/// <typeparam name="T"></typeparam>
public class CsvDb<T> where T : new()
{
    #region 属性

    /// <summary>实体比较器</summary>
    public IEqualityComparer<T> Comparer { get; set; }

    /// <summary>文件编码，默认utf8</summary>
    public Encoding Encoding { get; set; } = Encoding.UTF8;

    /// <summary>文件名</summary>
    public String? FileName { get; set; }

    #endregion 属性

    #region 构造

    /// <summary>实例化Csv文件数据库</summary>
    public CsvDb() => Comparer = EqualityComparer<T>.Default;

    /// <summary>实例化Csv文件数据库</summary>
    /// <param name="comparer"></param>
    public CsvDb(Func<T?, T?, Boolean> comparer) => Comparer = new MyComparer(comparer);

    #endregion 构造

    #region 方法

    /// <summary>尾部插入数据，性能极好</summary>
    /// <param name="model"></param>
    public void Add(T model) => Add(new[] { model });

    /// <summary>尾部插入数据，性能极好</summary>
    /// <param name="models"></param>
    public void Add(IEnumerable<T> models) => Write(models, true);

    /// <summary>清空数据。只写头部</summary>
    public void Clear() => Write(Array.Empty<T>(), false);

    /// <summary>查找指定数据行</summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public T? Find(T model)
    {
        if (Comparer == null) throw new ArgumentNullException(nameof(Comparer));

        return FindAll(e => Comparer.Equals(model, e), 1).FirstOrDefault();
    }

    /// <summary>获取满足条件的第一行数据</summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public T? Find(Func<T, Boolean>? predicate) => FindAll(predicate, 1).FirstOrDefault();

    /// <summary>获取所有数据行</summary>
    /// <returns></returns>
    public IList<T> FindAll() => FindAll(null);

    /// <summary>获取满足条件的数据行，性能好，顺序查找</summary>
    /// <param name="predicate"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public IList<T> FindAll(Func<T, Boolean>? predicate, Int32 count = -1)
    {
        var file = GetFile();
        if (!File.Exists(file)) return new List<T>();

        lock (this)
        {
            using var csv = new CsvFile(file, false) { Encoding = Encoding };

            var list = new List<T>();
            var headers = new Dictionary<String, Int32>();
            var pis = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            while (true)
            {
                var ss = csv.ReadLine();
                if (ss == null) break;

                // 头部，名称与序号对应
                if (headers.Count == 0)
                {
                    for (var i = 0; i < ss.Length; i++)
                    {
                        if (!headers.ContainsKey(ss[i])) headers[ss[i]] = i;
                    }
                    // 无法识别所有字段
                    if (headers.Count == 0) break;
                }
                else
                {
                    try
                    {
                        // 反射构建对象
                        var model = new T();
                        foreach (var pi in pis)
                        {
                            var name = SerialHelper.GetName(pi);
                            if (pi.CanWrite && headers.TryGetValue(name, out var idx) && idx < ss.Length)
                            {
                                model.SetValue(pi, ss[idx]);
                            }
                        }

                        if (predicate == null || predicate(model))
                        {
                            list.Add(model);

                            if (--count == 0) break;
                        }
                    }
                    catch
                    {
                    }
                }
            }

            return list;
        }
    }

    /// <summary>获取数据行数，性能极好，文件行数（除头部）</summary>
    /// <returns></returns>
    public Int32 FindCount()
    {
        lock (this)
        {
            var file = GetFile();
            if (!File.Exists(file)) return 0;

            var lines = File.ReadAllLines(file, Encoding);
            if (lines == null || lines.Length <= 1) return 0;

            // 除了头部以外的所有数据行
            return lines.Length - 1;
        }
    }

    /// <summary>删除数据，性能很差，全部读取剔除后保存</summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public Int32 Remove(T model) => Remove(new[] { model });

    /// <summary>删除数据，性能很差，全部读取剔除后保存</summary>
    /// <param name="models"></param>
    /// <returns></returns>
    public Int32 Remove(IEnumerable<T> models)
    {
        if (models == null || !models.Any()) return 0;
        if (Comparer == null) throw new ArgumentNullException(nameof(Comparer));

        return Remove(x => models.Any(y => Comparer.Equals(x, y)));
    }

    /// <summary>删除满足条件的数据，性能很差，全部读取剔除后保存</summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public Int32 Remove(Func<T, Boolean> predicate)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        lock (this)
        {
            var list = FindAll();
            if (list.Count == 0) return 0;

            var count = list.Count;
            list = list.Where(x => !predicate(x)).ToList();

            // 删除文件，重新写回去
            if (list.Count < count)
            {
                // 如果没有了数据，只写头部
                Write(list, false);
            }

            return count - list.Count;
        }
    }

    /// <summary>设置（添加或更新）指定数据行，性能很差，全部读取替换后保存</summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public Boolean Set(T model) => Set(model, true);

    /// <summary>更新指定数据行，性能很差，全部读取替换后保存</summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public Boolean Update(T model) => Set(model, false);

    /// <summary>强行写入数据</summary>
    /// <param name="models">要写入的数据</param>
    /// <param name="append">是否附加在尾部</param>
    public void Write(IEnumerable<T> models, Boolean append)
    {
        var file = GetFile();
        file.EnsureDirectory(true);

        using var fs = new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        if (append) fs.Position = fs.Length;

        using var csv = new CsvFile(fs, true) { Encoding = Encoding };

        var pis = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        // 首次写入文件头。需要正确处理协变逆变问题，兼容.NET2.0
        if (fs.Position == 0) csv.WriteLine(pis.Select(e => e.Name as Object));

        // 写入数据
        foreach (var item in models)
        {
            if (item != null)
                csv.WriteLine(pis.Select(e => item.GetValue(e)));
        }

        csv.TryDispose();
        fs.SetLength(fs.Position);
    }

    private Boolean Set(T model, Boolean add)
    {
        if (Comparer == null) throw new ArgumentNullException(nameof(Comparer));

        lock (this)
        {
            var list = FindAll();
            if (list.Count == 0) return false;

            // 找到目标数据行，并替换
            var flag = false;
            for (var i = 0; i < list.Count; i++)
            {
                if (Comparer.Equals(model, list[i]))
                {
                    list[i] = model;
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                if (!add) return false;

                list.Add(model);
            }

            // 重新写回去
            Write(list, false);

            return true;
        }
    }

    #endregion 方法

    #region 辅助

    private String GetFile()
    {
        if (FileName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(FileName));

        return FileName.GetFullPath();
    }

    private class MyComparer : IEqualityComparer<T>
    {
        public Func<T?, T?, Boolean> Comparer;

        public MyComparer(Func<T?, T?, Boolean> comparer) => Comparer = comparer;

        public Boolean Equals(T? x, T? y) => Comparer(x, y);

        public Int32 GetHashCode(T? obj) => obj?.GetHashCode() ?? 0;
    }

    #endregion 辅助
}
