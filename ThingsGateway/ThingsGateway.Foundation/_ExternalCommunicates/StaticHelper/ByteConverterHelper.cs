using System.Linq;
using System.Text;

namespace ThingsGateway.Foundation
{
    /// <summary>
    /// 所有数据转换类的静态辅助方法<br />
    /// </summary>
    public static class ByteConverterHelper
    {

        #region Public Methods

        /// <summary>
        ///设备地址可以带有的额外信息包含：
        /// DATA=XX;
        /// TEXT=XX;
        /// BCD=XX;
        /// LEN=XX;
        ///<br></br>
        /// 解析地址的附加<see cref="DataFormat"/> />参数方法，
        /// 并去掉address中的全部额外信息
        /// </summary>
        public static IThingsGatewayBitConverter GetTransByAddress(
          ref string address,
          IThingsGatewayBitConverter defaultTransform)
        {
            var strs = address.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            var format = strs.FirstOrDefault(m => !m.Trim().ToUpper().Contains("DATA="))?.ToUpper();
            DataFormat dataFormat = DataFormat.None;
            switch (format)
            {
                case "DATA=ABCD":
                    dataFormat = DataFormat.ABCD;
                    break;
                case "DATA=BADC":
                    dataFormat = DataFormat.BADC;
                    break;
                case "DATA=DCBA":
                    dataFormat = DataFormat.DCBA;
                    break;
                case "DATA=CDAB":
                    dataFormat = DataFormat.CDAB;
                    break;
            }

            //去除以上的额外信息
            address = String.Join(";", strs.Where(m =>
            (!m.Trim().ToUpper().Contains("DATA=")) &&
            (!m.Trim().ToUpper().Contains("TEXT=")) &&
            (!m.Trim().ToUpper().Contains("BCD=")) &&
            (!m.Trim().ToUpper().Contains("LEN="))
            ));

            return dataFormat != defaultTransform.DataFormat && dataFormat != DataFormat.None ?
                    defaultTransform.CreateByDateFormat(dataFormat) :
                    defaultTransform;
        }

        /// <summary>
        /// <inheritdoc cref="GetTransByAddress(ref string, IThingsGatewayBitConverter)"/>
        /// </summary>
        public static void GetTransByAddress(ref string address)
        {
            var strs = address.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            //去除以上的额外信息
            address = String.Join(";", strs.Where(m =>
            (!m.Trim().ToUpper().Contains("DATA=")) &&
            (!m.Trim().ToUpper().Contains("TEXT=")) &&
            (!m.Trim().ToUpper().Contains("BCD=")) &&
            (!m.Trim().ToUpper().Contains("LEN="))
            ));
        }

        /// <summary>
        /// <inheritdoc cref="GetTransByAddress(ref string, IThingsGatewayBitConverter)"/>
        /// </summary>
        public static IThingsGatewayBitConverter GetTransByAddress(
          ref string address,
          IThingsGatewayBitConverter defaultTransform,
          out Encoding encoding,
          out ushort length,
          out BCDFormat bCDFormat)
        {
            encoding = Encoding.Default;
            length = 0;
            bCDFormat = BCDFormat.C8421;
            if (address.IsNullOrEmpty()) return defaultTransform;
            var strs = address.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            var format = strs.FirstOrDefault(m => m.Trim().ToUpper().Contains("DATA="))?.ToUpper();
            DataFormat dataFormat = DataFormat.None;
            switch (format)
            {
                case "DATA=ABCD":
                    dataFormat = DataFormat.ABCD;
                    break;
                case "DATA=BADC":
                    dataFormat = DataFormat.BADC;
                    break;
                case "DATA=DCBA":
                    dataFormat = DataFormat.DCBA;
                    break;
                case "DATA=CDAB":
                    dataFormat = DataFormat.CDAB;
                    break;
            }

            var strencoding = strs.FirstOrDefault(m => m.Trim().ToUpper().Contains("TEXT="))?.ToUpper();
            encoding = Encoding.Default;
            switch (strencoding)
            {
                case "TEXT=UTF8":
                    encoding = Encoding.UTF8;
                    break;
                case "TEXT=ASCII":
                    encoding = Encoding.ASCII;
                    break;
                case "TEXT=Default":
                    encoding = Encoding.Default;
                    break;
                case "TEXT=Unicode":
                    encoding = Encoding.Unicode;
                    break;
            }

            var strlen = strs.FirstOrDefault(m => m.Trim().ToUpper().Contains("STRLEN="))?.ToUpper().Replace("STRLEN=", "");
            length = strlen.IsNullOrEmpty() ? (ushort)0 : Convert.ToUInt16(strlen);

            var strbCDFormat = strs.FirstOrDefault(m => m.Trim().ToUpper().Contains("BCD="))?.ToUpper();
            bCDFormat = BCDFormat.C8421;
            switch (strbCDFormat)
            {
                case "BCD=C8421":
                    bCDFormat = BCDFormat.C8421;
                    break;
                case "BCD=C2421":
                    bCDFormat = BCDFormat.C2421;
                    break;
                case "BCD=C3":
                    bCDFormat = BCDFormat.C3;
                    break;
                case "BCD=C5421":
                    bCDFormat = BCDFormat.C5421;
                    break;
                case "BCD=Gray":
                    bCDFormat = BCDFormat.Gray;
                    break;
            }

            //去除以上的额外信息
            address = String.Join(";", strs.Where(m =>
            (!m.Trim().ToUpper().Contains("DATA=")) &&
            (!m.Trim().ToUpper().Contains("TEXT=")) &&
            (!m.Trim().ToUpper().Contains("BCD=")) &&
            (!m.Trim().ToUpper().Contains("LEN="))
            ));

            return dataFormat != defaultTransform.DataFormat && dataFormat != DataFormat.None ?
                    defaultTransform.CreateByDateFormat(dataFormat) :
                    defaultTransform;

        }

        #endregion Public Methods

    }
}