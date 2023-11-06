
using System.Text;
namespace QWeb
{
    /// <summary>
    /// 封装一下转换
    /// </summary>
    public static class QByteExpand
    {
        public static string ToDefault(this byte[] target)
        {
            return Encoding.Default.GetString(target);
        }
        public static string ToUTF8(this byte[] target)
        {
            return Encoding.UTF8.GetString(target);
        }
        public static string ToUTF7(this byte[] target)
        {
            return Encoding.UTF7.GetString(target);
        }
        public static string ToUTF32(this byte[] target)
        {
            return Encoding.UTF32.GetString(target);
        }
        public static string ToUnicode(this byte[] target)
        {
            return Encoding.Unicode.GetString(target);
        }
        public static string ToBigEndianUnicode(this byte[] target)
        {
            return Encoding.BigEndianUnicode.GetString(target);
        }
        public static string ToAscii(this byte[] target)
        {
            return Encoding.ASCII.GetString(target);
        }
    }
}